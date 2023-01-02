using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [AddComponentMenu("SKCell/CSVLoader")]
    public class SKCSVLoader : MonoSingleton<SKCSVLoader>
    {
        protected override void Awake()
        {
            base.Awake();
            LoadTable();
        }

        private const string CSVPATH = "CSV";

        /// <summary>
        /// 添加CSV表.
        /// </summary>
        public void LoadTable()
        {
            List<string> tableList = new List<string>();
            var tables = Resources.LoadAll(CSVPATH);
            if (tables != null)
            {
                foreach (var table in tables)
                {
                    TextAsset textAsset = table as TextAsset;
                    if (textAsset != null)
                    {
                        tableList.Add(textAsset.text);
                    }
                }
            }
            CommonUtils.EditorLogNormal("CSV Table count:" + tableList.Count);
            TableAgent.instance.Add(tableList);
        }
    }
}
