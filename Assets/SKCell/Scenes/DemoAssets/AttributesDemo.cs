using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell.Test
{
    public class AttributesDemo : MonoBehaviour
    {
        [SKInspectorText("Set the value of the magic number below,\nand use the button above to print it out!")]
        [SerializeField]
        private byte a;

        public int magicNumber = 42;
        [SKFolder("Integer Fields")]
        public int number1;
        public int number2, number3;

        [SKFolder("Lists")]
        public List<int> list1;
        public List<int> list2, list3;

        [SKFolder("Conditional Fields")]
        public bool useGameObject1;
        [SKConditionalField("useGameObject1", true)]
        public GameObject gameObject1;
        public bool useGameObject2;
        [SKConditionalField("useGameObject2", true)]
        public GameObject gameObject2;
        public bool useGameObject3;
        [SKConditionalField("useGameObject3", true)]
        public GameObject gameObject3;

        [SKEndFolder]
        [SKSeparator]
        public string name1;
        [SKSeparator]
        public string name2;

        [SKInspectorButton("Print Magic Number")]
        public void PrintMagicNumber()
        {
            print(magicNumber);
        }
    }

}
