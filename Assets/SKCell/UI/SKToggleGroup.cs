using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SKCell
{
    [DisallowMultipleComponent]
    [AddComponentMenu("SKCell/UI/SKToggleGroup")]
    public class SKToggleGroup : MonoBehaviour
    {
        [HideInInspector] public List<SKToggle> toggles = new List<SKToggle>();

        public SKToggleGroupMode mode = SKToggleGroupMode.Passive;

        [Header("Passive Mode Settings")]
        [Tooltip("Set all off toggles uninteractable if the number of on toggles reached this amount.")]
        public int maxActiveToggleCount = 1;
        [Tooltip("Set all on toggles uninteractable if the number of off toggles reached this amount.")]
        public int minActiveToggleCount = 0;

        [Header("Passive Mode Events")]
        [SerializeField] UnityEvent OnMaxReached;
        [SerializeField] UnityEvent OnMinReached;

        private int currentActiveToggleCount = 0;
        private bool initializing = false;
        private void Start()
        {
            if (maxActiveToggleCount < minActiveToggleCount)
            {
                SKUtils.EditorLogWarning($"SKToggleGroup: Max count cannot be smaller than min count! Gamobject:{name}");
            }
            Invoke("ApplyInitialConstraint", 0.2f);
        }
        private void ApplyConstraints()
        {
            if (initializing)
                return;
            if (currentActiveToggleCount >= maxActiveToggleCount)
            {
                OnMaxReached.Invoke();
                foreach(var item in toggles)
                {
                    item.Interactable = item.isOn;
                }
            }
            else if (currentActiveToggleCount <= minActiveToggleCount)
            {
                OnMinReached.Invoke();
                foreach (var item in toggles)
                {
                  item.Interactable = !item.isOn;
                }
            }
            else{
                foreach (var item in toggles)
                {
                   item.Interactable = true;
                }
            }
        }
        private void ApplyInitialConstraint()
        {
            if (mode == SKToggleGroupMode.ActiveOneOnly)
            {
                return;
            }
            initializing = true;
            if (currentActiveToggleCount > maxActiveToggleCount)
            {
                int diff = currentActiveToggleCount - maxActiveToggleCount;
                int count = 0, i = 0 ;
                while (count < diff)
                {
                    if (toggles[i].isOn)
                    {
                        toggles[i].Toggle();
                        toggles[i].Interactable = false;
                        count++;
                        i++;
                    }
                }
            }
            if (currentActiveToggleCount < minActiveToggleCount)
            {
                int diff = minActiveToggleCount - currentActiveToggleCount;
                int count = 0, i = 0;
                while (count < diff)
                {
                    if (!toggles[i].isOn)
                    {
                        toggles[i].Toggle();
                        toggles[i].Interactable = false;
                        count++;
                        i++;
                    }
                }
            }
            initializing = false;
        }
        public void OnMemberStateChanged(SKToggle toggle)
        {
            if (mode == SKToggleGroupMode.ActiveOneOnly)
            {
                if (toggle.isOn)
                {
                    foreach(var item in toggles)
                    {
                        if (item != toggle)
                            item.ToggleOff();
                    }
                }
                return;
            }
            currentActiveToggleCount += toggle.isOn ? 1 : -1;
            if (currentActiveToggleCount < 0)
            {
                ReCount();
            }
            ApplyConstraints();
        }
        public void AddMember(SKToggle toggle)
        {
            toggles.Add(toggle);
            if (toggle.isOn)
                currentActiveToggleCount++;
        }
        public void RemoveMember(SKToggle toggle)
        {
            toggles.Remove(toggle);
            if (toggle.isOn)
                currentActiveToggleCount--;
        }
        private void ReCount()
        {
            currentActiveToggleCount = 0;
            foreach(var item in toggles)
            {
                if (item.isOn)
                    currentActiveToggleCount++;
            }
        }

        public enum SKToggleGroupMode
        {
            Passive,
            ActiveOneOnly
        }
    }
}
