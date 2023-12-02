using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SKCell
{
    [AddComponentMenu("SKCell/UI/SKDragSpawner")]
    public class SKDragSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public int spawnerID = 0;

        [Tooltip("The object you want to spawn")]
        public GameObject spawnObject;

        [Tooltip("Spawn position. Use this transform as default.")]
        public Transform spawnPos;

        [Tooltip("Parent of spawned object. Use this transform as default.")]
        public Transform spawnParent;

        public RectTransform constraint;

        [Header("Events")]
        public SKDragSpawnerEvent onBeginDrag;
        private void Start()
        {
            if (spawnPos == null)
                spawnPos = GetComponent<Transform>();
            if (spawnParent == null)
                spawnParent = GetComponent<Transform>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag.Invoke(eventData);
        }

        public void SpawnAndAttach(PointerEventData eventData)
        {
            GameObject inst = Instantiate(spawnObject, spawnParent);
            inst.transform.position = spawnPos.position;
            SKDragger drag = inst.GetComponentInChildren<SKDragger>();
            if (drag == null)
            {
                SKUtils.EditorLogError("SKDragSpawner: spawnObject must have SKDragger component on itself or its children.");
                Destroy(inst);
                return;
            }
            drag.onSpawn.Invoke();
            drag.constraint = constraint;
            eventData.pointerDrag = inst;
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        [Serializable]
        public class SKDragSpawnerEvent : UnityEvent<PointerEventData> { }
    }
}