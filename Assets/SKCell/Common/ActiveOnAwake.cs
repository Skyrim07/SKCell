using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveOnAwake : MonoBehaviour
{
    public GameObject target;
    public bool active = false;
    private void Awake()
    {
        target.SetActive(active);
    }

}
