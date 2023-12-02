using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [AddComponentMenu("SKCell/CSV/SKCSVLoader")]
    public class SKCSVLoader : SKMonoSingleton<SKCSVLoader>
    {
        protected override void Awake()
        {
            base.Awake();
            LoadTable();
        }

        private const string CSVPATH = "CSV";

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
            SKUtils.EditorLogNormal("CSV Table count:" + tableList.Count);
            SKCSVReader.instance.Add(tableList);
        }
    }
}
