using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
