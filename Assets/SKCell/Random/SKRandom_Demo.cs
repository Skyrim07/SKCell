using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell.Test
{
    public class SKRandom_Demo : MonoBehaviour
    {
        [SKFolder("Settings")]
        public int count = 100;

        [SKFolder("References")]
        public GameObject boxPF;
        public Transform boxContainer;
        void Start()
        {
            float mean = 1.0f, stdDev = .1f;
            List<float> nums = new List<float>();
            for (int i = 0; i < count; i++)
            {
                nums.Add(SKRandom.NormalDistribution(mean, stdDev));
            }
            nums.Sort();
            for (int i = 0; i < nums.Count; i++)
            {
                RectTransform rect = Instantiate(boxPF, boxContainer).GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y *nums[i]);
            }
        }

    }
}
