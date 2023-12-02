using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [AddComponentMenu("SKCell/Physics & GO/SKMeasurer")]
    public class SKMeasurer : MonoBehaviour
    {
        public SKMeasurerMode mode;
        public List<Transform> measuredObjects = new List<Transform>();

    }

    public enum SKMeasurerMode
    {
        EveryObject,
        NextObject
    }
}