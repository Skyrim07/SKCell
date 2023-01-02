using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Sveta
{

    [System.Serializable]
    public class UnityEventString : UnityEvent<string>
    {

    }

    public class ObjectsSwitcher : MonoBehaviour
    {
        public UnityEventString outputName;

        public List<GameObject> list;
        private int index = 0;

        public void Switch(int delta)
        {
            index += delta;

            if (index > list.Count - 1)
            {
                index = 0;
            }

            if (index < 0)
            {
                index = list.Count - 1;
            }


            SwitchTo(index);
        }

        private void SwitchTo(int _index) {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetActive(i == _index);
            }

            outputName?.Invoke(list[_index].name);

        }

        public void Awake()
        {
            /*
            for (int i = 0; i < list.Count; i++)
            {
                list[i].gameObject.SetActive(false);
            }
            list[0].SetActive(true);
            */

            SwitchTo(0);
        }


    }
}
