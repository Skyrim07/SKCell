using NChardet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SKCell
{
    public class CSVParser : Editor
    {
        private static readonly string SplitRow = "###";

        private static readonly string CommentStr = "//";

        private static readonly string ConstomPath = "Assets/Resources/CSV/";
        private static readonly string SourcePath = "Table/";

        private static List<Action> _tablePass = new List<Action>();

        private static Dictionary<string, List<List<string>>> _tables = new Dictionary<string, List<List<string>>>();

        private static DirectoryInfo TheFolder = new DirectoryInfo("Table/");
        private static Regex _tableNameRegex = new Regex(@"\(([^>])+\)");

        [MenuItem("SKCell/Build CSV Files")]
        public static void BuildCSVFile()
        {
            //For Test
            LoadConfig();

            MakeCsv(SourcePath);

            DirectoryInfo TheFolder = new DirectoryInfo(SourcePath);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                MakeCsv(SourcePath + NextFolder.Name + "/");
            }
        }

        /// <summary>
        /// 包装起来了，用来多改一个添加里文件夹里面东西的表格的东西
        /// </summary>
        /// <param name="soucepath"></param>
        private static void MakeCsv(string soucepath)
        {
            _tablePass = new List<Action>()
        {
            TableSplit,
            CommentRowLine,
            CommentClear,
        };

            string tableName = "";
            try
            {
                DirectoryInfo directory = new DirectoryInfo(soucepath);
                var files = directory.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".csv"))
                    {
                        ReadFile(files[i].FullName);
                    }
                }

                for (int i = 0; i < _tablePass.Count; i++)
                {
                    _tablePass[i]();
                }
                foreach (var pair in _tables)
                {
                    string path = pair.Key;
                    tableName = pair.Key;
                    //查看有没有括号
                    Match match = _tableNameRegex.Match(path);
                    if (match.Success)
                    {
                        //分离括号里真的表名
                        pair.Value[0][0] = match.Value.Substring(1, match.Length - 2);
                        //分离掉括号内容
                        path = path.Substring(0, match.Index);
                    }
                    else
                    {
                        if (path.Contains("/"))
                        {
                            pair.Value[0][0] = path.Substring(path.LastIndexOf("/") + 1);
                        }
                    }

                    //替换变量
                    ReplaceParameter(pair);

                    path = path.Replace("//", "/");
                    CSV.WriteCSV(ConstomPath + path + ".csv", pair.Value);
                    CommonUtils.EditorLogNormal( "[CSVParser] CSV Generated：" + ConstomPath + path);
                }

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                CommonUtils.EditorLogError("[CSVParser] CSV Parse Error：" + tableName + "  " + e);
            }
            finally
            {
                _tablePass.Clear();
                _tables.Clear();
            }
        }

        private static void ReadFile(string filePath)
        {
            var str = ReadFileAndEncoding(filePath);
            var table = CSV.Decode(str);
            _tables.Add(table[0][0], table);
        }

        /// <summary>
        /// 自动识别编码并读取文件
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static string ReadFileAndEncoding(string filePath)
        {
            using (FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                int lang = 2;//
                             //用指定的语参数实例化Detector
                Detector det = new Detector(lang);
                //初始化
                CharsetDetectionObserver cdo = new CharsetDetectionObserver();
                det.Init(cdo);

                Stream stream = fs;
                byte[] buf = new byte[1024];
                int len;
                bool done = false;
                bool isAscii = true;

                while ((len = stream.Read(buf, 0, buf.Length)) != 0)
                {
                    // 探测是否为Ascii编码
                    if (isAscii)
                        isAscii = det.isAscii(buf, len);

                    // 如果不是Ascii编码，并且编码未确定，则继续探测
                    if (!isAscii && !done)
                        done = det.DoIt(buf, len, false);

                }
                //stream.Close();
                //stream.Dispose();
                //调用DatEnd方法，
                //如果引擎认为已经探测出了正确的编码，
                //则会在此时调用ICharsetDetectionObserver的Notify方法
                det.DataEnd();

                StreamReader sr = null;

                fs.Position = 0;
                if (isAscii)
                {
                    sr = new StreamReader(fs, Encoding.ASCII);
                }
                else if (cdo.Charset != null)
                {
                    sr = new StreamReader(fs, Encoding.GetEncoding(cdo.Charset));
                }
                else
                {
                    sr = new StreamReader(fs, Encoding.UTF8);
                }

                try
                {
                    string str = sr.ReadToEnd();
                    return str;
                }
                finally
                {
                    sr.Dispose();
                }
            }
        }

        public class CharsetDetectionObserver :
         NChardet.ICharsetDetectionObserver
        {
            public string Charset = null;

            public void Notify(string charset)
            {
                Charset = charset;
            }
        }

        /// <summary>
        /// 表格拆分
        /// </summary>
        private static void TableSplit()
        {
            var newTable = new Dictionary<string, List<List<string>>>();
            foreach (var pair in _tables)
            {
                int end = pair.Value.Count - 1;
                for (int i = pair.Value.Count - 1; i >= 0; i--)
                {
                    if (pair.Value[i][0].Equals(SplitRow))
                    {
                        int row = i + 1;

                        string name = pair.Value[row][0];
                        var nt = new List<List<string>>();
                        newTable[name] = nt;
                        //添加到新表
                        for (int j = i + 1; j <= end; j++)
                        {
                            row = j;
                            nt.Add(pair.Value[row]);
                        }
                        end = row;
                        //删除旧的行
                        for (int j = end; j >= i; j--)
                        {
                            row = j;
                            pair.Value.RemoveAt(row);
                        }
                        end = pair.Value.Count - 1;
                    }
                }
            }
            foreach (var pair in newTable)
            {
                _tables[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// 注释行 列删除
        /// </summary>
        private static void CommentRowLine()
        {
            foreach (var pair in _tables)
            {
                var table = pair.Value;
                //寻找注释行
                for (int i = table.Count - 1; i >= 0; i--)
                {
                    if (table[i][0].StartsWith(CommentStr) || string.IsNullOrEmpty(table[i][0]))
                    {
                        table.RemoveAt(i);
                    }
                }
                for (int i = table[0].Count - 1; i >= 0; i--)
                {
                    if (table[0][i].StartsWith(CommentStr) || string.IsNullOrEmpty(table[0][i]))
                    {
                        for (int j = table.Count - 1; j >= 0; j--)
                        {
                            if (table[j].Count > i)
                            {
                                table[j].RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注释删除
        /// </summary>
        private static void CommentClear()
        {
            foreach (var pair in _tables)
            {
                var table = pair.Value;
                for (int r = 0; r < table.Count; r++)
                {
                    for (int l = 0; l < table[r].Count; l++)
                    {
                        int index = table[r][l].IndexOf(CommentStr);
                        if (index >= 0)
                        {
                            table[r][l] = table[r][l].Remove(index, table[r][l].Length - index);
                        }
                    }
                }
            }
        }

        //

        private static readonly string _configPath = SourcePath + "CSVMaker.conf";
        private static List<KeyValuePair<string, string>> _replaceParamTableName;

        /// <summary>
        /// 加载设置 主要是替换表格的配置 以后可能会有其他吧
        /// </summary>
        private static void LoadConfig()
        {
            _replaceParamTableName = new List<KeyValuePair<string, string>>();
            string confText = ReadFileAndEncoding(_configPath);
            List<List<string>> config = CSV.Decode(confText);
            //删除注释 #
            config.RemoveAll((item) =>
            {
                return item[0][0].Equals('#');
            });
            //分割两个表名
            config.ForEach((item) =>
            {
                string[] tableNames = item[0].Split(':');
                _replaceParamTableName.Add(new KeyValuePair<string, string>(tableNames[0], tableNames[1]));
            });
        }

        /// <summary>
        /// 变量替换
        /// </summary>
        private static void ReplaceParameter(KeyValuePair<string, List<List<string>>> pair)
        {
            var matchList = _replaceParamTableName.FindAll((i) => { return i.Value == pair.Value[0][0]; });
            if (matchList.Count <= 0)
            {
                return;
            }

            List<string> source = new List<string>();
            matchList.ForEach((i) => { source.Add(i.Key); });


            //解析pair条目
            for (int i = 0; i < pair.Value.Count; i++)
            {
                for (int j = 0; j < pair.Value[i].Count; j++)
                {
                    string tableName = source[0];
                    string text = pair.Value[i][j];

                    string pattern = "<table[^>]*?{0}=(['\"\"]?)(?<{1}>[^'\"\"\\s>]+)\\1[^>]*>";
                    string columnName = Regex.Match(text, string.Format(pattern, "column", "columnInfo")).Groups["columnInfo"].Value.Trim('"');

                    if (columnName.Length == 0)
                    {
                        continue;
                    }
                    string specialTable = Regex.Match(text, string.Format(pattern, "source", "tableInfo")).Groups["tableInfo"].Value.Trim('"');
                    string key1 = Regex.Match(text, string.Format(pattern, "key", "keyInfo")).Groups["keyInfo"].Value.Trim('"');
                    if (specialTable.Length != 0)
                    {
                        //自定义表
                        tableName = specialTable;
                        if (key1.Length == 0)
                        {
                            Debug.LogError("指定了表源但未指定key");
                            return;
                        }
                    }
                    string functionName = Regex.Match(text, string.Format(pattern, "function", "functionInfo")).Groups["functionInfo"].Value.Trim('"');
                    int parameterIndex = int.Parse(Regex.Match(text, string.Format(pattern, "parameter", "paramInfo")).Groups["paramInfo"].Value.Trim('"'));
                    int rowIndex = i;

                    //找到替换源
                    KeyValuePair<string, List<List<string>>> sourceTable = default(KeyValuePair<string, List<List<string>>>);
                    foreach (KeyValuePair<string, List<List<string>>> kvPair in _tables)
                    {
                        if (kvPair.Value[0][0].Equals(tableName))
                        {
                            sourceTable = kvPair;
                            break;
                        }
                    }

                    if (sourceTable.Equals(default(KeyValuePair<string, List<List<string>>>)))
                    {
                        Debug.LogError("无法找到数据来源表");
                        return;
                    }

                    int columnIndex = sourceTable.Value[0].FindIndex((co) => { return co == columnName; });
                    if (columnIndex == 0)
                    {
                        Debug.LogError("无法找到数据来源列");
                        return;
                    }

                    if (key1.Length != 0)
                    {
                        for (int row = 0; row < sourceTable.Value.Count; row++)
                        {
                            if (sourceTable.Value[row][0] == key1)
                            {
                                rowIndex = row;
                                break;
                            }
                        }
                    }

                    List<string> paramStrings = new List<string>();
                    if (functionName.Length == 0)
                    {
                        //只是替换成表里的文本
                        paramStrings.AddRange(sourceTable.Value[rowIndex][columnIndex].Split('|'));
                    }
                    else
                    {
                        string funcExpression = "";
                        //替换为函数里的参数
                        funcExpression = sourceTable.Value[rowIndex][columnIndex];


                        pattern = "(?<=" + functionName + @"\:)([.-]|[.+]|[0-9]|[._])+";
                        paramStrings.AddRange(Regex.Match(funcExpression, pattern).Value.Split('_'));

                        //
                        //for (int k = 0; k < paramStrings.Count; k++)
                        //{
                        //    if (int.Parse(paramStrings[k]) > 0 && paramStrings[k][0] != '+')
                        //    {
                        //        paramStrings[k] = "+" + paramStrings[k];
                        //    }
                        //}
                    }

                    //判断越界
                    if (paramStrings.Count < parameterIndex)
                    {
                        Debug.LogError("无法找到参数");
                        return;
                    }

                    //替换文本域
                    pattern = "<table[^>]*?>";
                    pair.Value[i][j] = Regex.Replace(text, pattern, paramStrings[parameterIndex]);
                }
            }
        }


    }
}