using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace SKCell
{
    public class CSV
    {
        //write a new file, existed file will be overwritten
        public static void WriteCSV(string filePathName, List<String[]> ls)
        {
            WriteCSV(filePathName, false, ls);
        }

        //write a file, existed file will be overwritten if append = false
        public static void WriteCSV(string filePathName, bool append, List<String[]> ls)
        {
            StreamWriter fileWriter = new StreamWriter(filePathName, append, Encoding.Default);
            foreach (String[] strArr in ls)
            {
                fileWriter.WriteLine(String.Join(",", strArr));
            }
            fileWriter.Flush();
            fileWriter.Close();
        }

        public static void WriteCSV(string filePathName, List<List<string>> ls)
        {
            WriteCSV(filePathName, false, ls);
        }

        //write a file, existed file will be overwritten if append = false
        public static void WriteCSV(string filePathName, bool append, List<List<string>> ls)
        {
            StreamWriter fileWriter = new StreamWriter(filePathName, append, Encoding.UTF8);
            foreach (List<string> strArr in ls)
            {
                fileWriter.WriteLine(String.Join(",", strArr.ToArray()));
            }
            fileWriter.Flush();
            fileWriter.Close();
        }

        public static List<List<string>> Decode(string text)
        {
            if (text == null)
                return null;

            List<List<string>> result = new List<List<string>>();
            List<string> line = new List<string>();
            string field = "";
            bool isInQuotation = false;//字符串模式  
            bool isInField = true;//是否在读取Field，用来表示空Field  
            int i = 0;
            while (i < text.Length)
            {
                char ch = text[i];
                if (isInQuotation)
                {
                    if (ch == '"')
                    {
                        if (i < text.Length - 1 && text[i + 1] == '"')//重复"只算一个，切不结束字符串模式  
                        {
                            field += '"';
                            i++;
                        }
                        else
                        {
                            isInQuotation = false;
                        }
                    }
                    else
                    {
                        field += ch;//字符串模式中所有字符都要加入  
                    }
                }
                else
                {
                    switch (ch)
                    {
                        case ',':
                            line.Add(field);
                            field = "";
                            isInField = true;
                            break;
                        case '"':
                            if (isInField)
                                isInQuotation = true;//进入字符串模式  
                            else
                                field += ch;
                            break;
                        case '\n':
                        case '\r':
                            if (field.Length > 0 || isInField)
                            {
                                line.Add(field);
                                field = "";
                            }
                            result.Add(line);
                            line = new List<string>();
                            isInField = true;//下一行首先应该是数据  
                            if (i < text.Length - 1 && text[i + 1] == '\n')//跳过\r\n  
                                i++;
                            break;
                        default:
                            isInField = false;
                            field += ch;
                            break;
                    }
                }
                i++;
            }
            //收尾工作  
            if (field.Length > 0 || isInField && line.Count > 0)//如果是isInField标记的单元格，则要保证这行有其他数据，否则单独一个空单元格的行是没有意义的  
                line.Add(field);

            if (line.Count > 0)
                result.Add(line);

            return result;
        }
    }
}