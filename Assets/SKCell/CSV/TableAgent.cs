using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace SKCell
{
    /// <summary>
    /// 配置表代理
    /// </summary>
    public class TableAgent : Singleton<TableAgent>
    {
        private Dictionary<string, Dictionary<TableKey, string>> _tableAgent;

        private Dictionary<string, List<string>> _keys;

        /// <summary>
        /// 加载多个CSV文件
        /// </summary>
        /// <param name="tables">The tables.</param>
        public void Add(List<string> tables)
        {
            if (_tableAgent == null)
            {
                _tableAgent = new Dictionary<string, Dictionary<TableKey, string>>();
                _keys = new Dictionary<string, List<string>>();
            }

            for (int i = 0; i < tables.Count; i++)
            {
                var table = CSV.Decode(tables[i]);
                string name = table[0][0];
                _tableAgent[name] = new Dictionary<TableKey, string>();
                _keys[name] = new List<string>();

                for (int j = 0; j < table.Count; j++)
                {
                    _keys[name].Add(table[j][0]);
                    for (int k = 0; k < table[j].Count; k++)
                    {
                        _tableAgent[name][new TableKey(table[j][0], table[0][k])] = table[j][k];
                    }
                }
            }
        }

        /// <summary>
        /// 添加单个条目
        /// </summary>
        /// <typeparam name="T">添加的值类型</typeparam>
        /// <param name="name">表名称.</param>
        /// <param name="key1">键1.</param>
        /// <param name="key2">键2</param>
        /// <param name="value">值</param>
        public void Add<T>(string name, string key1, string key2, T value)
        {
            if (_tableAgent == null)
            {
                _tableAgent = new Dictionary<string, Dictionary<TableKey, string>>();
                _keys = new Dictionary<string, List<string>>();
            }

            if (!_tableAgent.ContainsKey(name))
            {
                _tableAgent[name] = new Dictionary<TableKey, string>();
                _keys[name] = new List<string>();
            }

            if (!_keys[name].Contains(key1))
            {
                _keys[name].Add(key1);
            }

            _tableAgent[name][new TableKey(key1, key2)] = value.ToString();
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="name">The index.</param>
        /// <param name="key1">The horizontal key.</param>
        /// <param name="key2">The vertical key.</param>
        /// <returns></returns>
        public string GetString(string name, string key1, string key2)
        {
            if (_tableAgent.ContainsKey(name)&&_tableAgent[name].ContainsKey(new TableKey(key1, key2)))
            {
                return _tableAgent[name][new TableKey(key1, key2)];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <param name="name">The index.</param>
        /// <param name="key1">The horizontal key.</param>
        /// <param name="key2">The vertical key.</param>
        /// <returns></returns>
        public float GetFloat(string name, string key1, string key2)
        {
            return float.Parse(_tableAgent[name][new TableKey(key1, key2)]);
        }

        /// <summary>
        /// 获取整数值
        /// </summary>
        /// <param name="name">The index.</param>
        /// <param name="key1">The key1.</param>
        /// <param name="key2">The key2.</param>
        /// <returns></returns>
        public int GetInt(string name, string key1, string key2)
        {
            int x = 0;
            bool isOK = int.TryParse(_tableAgent[name][new TableKey(key1, key2)], out x);
            if (isOK)
            {
                return x;
            }
            else
            {
                // Debug.LogError(string.Format("name{0}-key1{1}-key2{2}",name,key1,key2));
                return x;
            }
            //return int.Parse();
        }

        /// <summary>
        /// 获取分隔字符串
        /// </summary>
        /// <param name="name">The index.</param>
        /// <param name="key1">The key1.</param>
        /// <param name="key2">The key2.</param>
        /// <returns></returns>
        public string[] GetStrings(string name, string key1, string key2)
        {
            return (_tableAgent[name][new TableKey(key1, key2)]).Split('|');
        }

        /// <summary>
        /// 获取所有Key1值
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public List<string> CollectKey1(string name)
        {
            var list = _keys[name];
            list.Remove(name);
            return list;
        }

        /// <summary>
        /// 改变带<<>>的字符.
        /// </summary>
        /// <param name="des">原字符串</param>
        /// <param name="trs">转换函数</param>
        /// <returns>转换后的字符串</returns>
        public static string TransformString(string des, Func<string, string> trs)
        {
            //第一个不是<<>>
            List<string> secList = SecStrings(des);
            if (secList == null)
            {
                return des;
            }

            //替换所有下标为单数的.
            for (int i = 1; i < secList.Count; i += 2)
            {
                secList[i] = trs(secList[i]);
            }

            string value = "";
            for (int i = 0; i < secList.Count; i++)
            {
                value += secList[i];
            }

            return value;
        }

        /// <summary>
        /// 将带<<>>的分段.
        /// </summary>
        /// <param name="des"></param>
        /// <returns>分段</returns>
        private static List<string> SecStrings(string des)
        {
            List<string> secList = new List<string>();
            Match match = Regex.Match(des, "<<([^>])+>>");
            if (!match.Success)
            {
                return null;
            }

            string str1 = des.Substring(0, match.Index); //第一段.
            string str3 = des.Substring(match.Index + match.Length); //第三段
            string str2 = des.Substring(match.Index + 2, match.Length - 4); //第二段
            secList.Add(str1);
            secList.Add(str2);
            List<string> subList = SecStrings(str3);
            if (subList != null)
            {
                for (int i = 0; i < subList.Count; i++)
                {
                    secList.Add(subList[i]);
                }
            }
            else
            {
                if (str3 != "")
                {
                    secList.Add(str3);
                }
            }

            return secList;
        }

        /// <summary>
        /// 清除操作，用于测试
        /// </summary>
        public void Clear()
        {
            _tableAgent.Clear();
        }
    }

    /// <summary>
    /// 表格键值对结构
    /// </summary>
    public struct TableKey
    {
        public string Key1;
        public string Key2;

        public TableKey(string key1, string key2)
        {
            Key1 = key1;
            Key2 = key2;
        }
    }
}