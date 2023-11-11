using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SKCell
{
    public class SKConsoleTip : MonoBehaviour
    {
        [SerializeField] TMP_Text cmdText, descripText, argText;

        public void UpdateInfo(string name, string arg, string descrip)
        {
            cmdText.text = name;
            descripText.text = descrip;
            argText.text = arg;
        }
    }
}