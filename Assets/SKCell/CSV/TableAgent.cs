using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SKCell
{
    public class SKCSVReader : SKSingleton<SKCSVReader>
    {
        private Dictionary<string, Dictionary<TableKey, string>> _tableAgent;

        private Dictionary<string, List<string>> _keys;

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
        /// Get a string value from csv file
        /// </summary>
        /// <param name="name">The index. (upper-left corner)</param>
        /// <param name="key1">The row key.</param>
        /// <param name="key2">The column key.</param>
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
        /// Get a float value from csv file
        /// </summary>
        /// <param name="name">The index. (upper-left corner)</param>
        /// <param name="key1">The row key.</param>
        /// <param name="key2">The column key.</param>
        /// <returns></returns>
        public float GetFloat(string name, string key1, string key2)
        {
            return float.Parse(_tableAgent[name][new TableKey(key1, key2)]);
        }
        /// <summary>
        /// Get floats separated by '|'
        /// </summary>
        /// <param name="name">The index. (upper-left corner)</param>
        /// <param name="key1">The row key.</param>
        /// <param name="key2">The column key.</param>
        /// <returns></returns>
        public float[] GetFloats(string name, string key1, string key2)
        {
            string[] strs = _tableAgent[name][new TableKey(key1, key2)].Split('|');
            float[] result = new float[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                result[i] = float.Parse(strs[i]);
            }
            return result;
        }

        /// <summary>
        /// Get a int value from csv file
        /// </summary>
        /// <param name="name">The index. (upper-left corner)</param>
        /// <param name="key1">The row key.</param>
        /// <param name="key2">The column key.</param>
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
        /// Get ints separated by '|'
        /// </summary>
        /// <param name="name">The index. (upper-left corner)</param>
        /// <param name="key1">The row key.</param>
        /// <param name="key2">The column key.</param>
        /// <returns></returns>
        public int[] GetInts(string name, string key1, string key2)
        {
            string[] strs = _tableAgent[name][new TableKey(key1, key2)].Split('|');
            int[] result = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                result[i] = int.Parse(strs[i]);
            }
            return result;
        }

        /// <summary>
        /// Get string values splitted by '|' from csv file
        /// </summary>
        /// <param name="name">The index. (upper-left corner)</param>
        /// <param name="key1">The row key.</param>
        /// <param name="key2">The column key.</param>
        /// <returns></returns>
        public string[] GetStrings(string name, string key1, string key2)
        {
            return (_tableAgent[name][new TableKey(key1, key2)]).Split('|');
        }

        /// <summary>
        /// Get all row keys from csv file
        /// </summary>
        /// <param name="name">The index. (upper-left corner)</param>
        /// <returns></returns>
        public List<string> CollectKey1(string name)
        {
            var list = _keys[name];
            list.Remove(name);
            return list;
        }

        public static string TransformString(string des, Func<string, string> trs)
        {
            List<string> secList = SecStrings(des);
            if (secList == null)
            {
                return des;
            }
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

        private static List<string> SecStrings(string des)
        {
            List<string> secList = new List<string>();
            Match match = Regex.Match(des, "<<([^>])+>>");
            if (!match.Success)
            {
                return null;
            }

            string str1 = des.Substring(0, match.Index);
            string str3 = des.Substring(match.Index + match.Length);
            string str2 = des.Substring(match.Index + 2, match.Length - 4); 
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
    }

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