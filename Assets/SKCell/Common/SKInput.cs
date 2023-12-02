using System;
using System.Collections.Generic;
using UnityEngine;


namespace SKCell
{
    public class SKInput : SKMonoSingleton<SKInput>
    {
        private List<KeyCode> activeKeycodes = new List<KeyCode>();
        private Dictionary<KeyCode, List<Action>> keyDownActionDict = new Dictionary<KeyCode, List<Action>>();
        private Dictionary<int, List<Action>> mouseDownActionDict = new Dictionary<int, List<Action>>();
        private Dictionary<KeyCode, List<Action>> keyUpActionDict = new Dictionary<KeyCode, List<Action>>();
        private Dictionary<int, List<Action>> mouseUpActionDict = new Dictionary<int, List<Action>>();

        private KeyCode[] allKeyCodes;
        protected override void Awake()
        {
            base.Awake();
            allKeyCodes = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
        }
        public void Update()
        {
            KeyInputUpdate();
            MouseInputUpdate();
        }

        #region Key Input
        private void KeyInputUpdate()
        {
            foreach (var item in allKeyCodes)
            {
                if (Input.GetKeyDown(item))
                {
                    SKUtils.InsertToList(activeKeycodes, item, false);

                    if (keyDownActionDict.ContainsKey(item))
                    {
                        for (int i = 0; i < keyDownActionDict[item].Count; i++)
                        {
                            keyDownActionDict[item][i].Invoke();
                        }
                    }
                }
                if (Input.GetKeyUp(item))
                {
                    SKUtils.RemoveFromList(activeKeycodes, item);

                    if (keyUpActionDict.ContainsKey(item))
                    {
                        for (int i = 0; i < keyUpActionDict[item].Count; i++)
                        {
                            keyUpActionDict[item][i].Invoke();
                        }
                    }
                }
            }
        }

        public void RegisterKeyDownAction(KeyCode key, Action action)
        {
            if (!keyDownActionDict.ContainsKey(key))
            {
                keyDownActionDict.Add(key, new List<Action>());
            }

            keyDownActionDict[key].Add(action);
        }

        public void RemoveKeyDownAction(KeyCode key, Action action)
        {
            if (keyDownActionDict.ContainsKey(key))
            {
                keyDownActionDict[key].Remove(action);
            }
        }
        public void RegisterKeyUpAction(KeyCode key, Action action)
        {
            if (!keyUpActionDict.ContainsKey(key))
            {
                keyUpActionDict.Add(key, new List<Action>());
            }

            keyUpActionDict[key].Add(action);
        }
        public void RemoveKeyUpAction(KeyCode key, Action action)
        {
            if (keyUpActionDict.ContainsKey(key))
            {
                keyUpActionDict[key].Remove(action);
            }
        }

        public List<KeyCode> GetActiveKeycodes()
        {
            return activeKeycodes;
        }
        #endregion

        #region Mouse Input

        private void MouseInputUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!mouseDownActionDict.ContainsKey(0))
                    return;

                int keyCount = mouseDownActionDict[0].Count;
                for (int i = 0; i < keyCount; i++)
                {
                    mouseDownActionDict[0][i].Invoke();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (!mouseDownActionDict.ContainsKey(1))
                    return;

                int keyCount = mouseDownActionDict[1].Count;
                for (int i = 0; i < keyCount; i++)
                {
                    mouseDownActionDict[1][i].Invoke();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (!mouseUpActionDict.ContainsKey(0))
                    return;

                int keyCount = mouseUpActionDict[0].Count;
                for (int i = 0; i < keyCount; i++)
                {
                    mouseUpActionDict[0][i].Invoke();
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (!mouseUpActionDict.ContainsKey(1))
                    return;

                int keyCount = mouseUpActionDict[1].Count;
                for (int i = 0; i < keyCount; i++)
                {
                    mouseUpActionDict[1][i].Invoke();
                }
            }
        }

        public void RegisterMouseDownAction(int mouseID, Action action)
        {
            if (mouseID != 0 && mouseID != 1)
            {
                SKUtils.EditorLogWarning("SKInput.RegisterMouseDownAction: unidentified mouse id.");
                return;
            }

            if (!mouseDownActionDict.ContainsKey(mouseID))
            {
                mouseDownActionDict.Add(mouseID, new List<Action>());
            }
            mouseDownActionDict[mouseID].Add(action);
        }

        public void RemoveMouseDownAction(int mouseID, Action action)
        {
            if (mouseDownActionDict.ContainsKey(mouseID))
            {
                mouseDownActionDict[mouseID].Remove(action);
            }
        }
        public void RegisterMouseUpAction(int mouseID, Action action)
        {
            if (mouseID != 0 && mouseID != 1)
            {
                SKUtils.EditorLogWarning("SKInput.RegisterMouseUpAction: unidentified mouse id.");
                return;
            }

            if (!mouseUpActionDict.ContainsKey(mouseID))
            {
                mouseUpActionDict.Add(mouseID, new List<Action>());
            }
            mouseUpActionDict[mouseID].Add(action);
        }

        public void RemoveMouseUpAction(int mouseID, Action action)
        {
            if (mouseUpActionDict.ContainsKey(mouseID))
            {
                mouseUpActionDict[mouseID].Remove(action);
            }
        }
        #endregion
    }
}
