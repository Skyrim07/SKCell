using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



[AddComponentMenu("1Enwer/Input/_KeyBoard")]
public class _InputKeyBoard : MonoBehaviour
{
    public PressType pressType = PressType.GetKeyDown;
    public KeyCodeEvent[] keyBoardGetKey;

    public enum PressType
    {
        GetKey,
        GetKeyDown,
        GetKeyUp
    }


    [System.Serializable]
    public class KeyCodeEvent
    {
        public KeyCode keyCode;
        public UnityEvent function;
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < keyBoardGetKey.Length; i++)
        {
            if (KeyBoardIsPressed(pressType, keyBoardGetKey[i].keyCode))
            {
                keyBoardGetKey[i].function.Invoke();
            }
        }
    }

    public bool KeyBoardIsPressed(PressType _pressType, KeyCode keycode)
    {
        switch (_pressType)
        {
            case PressType.GetKey:
                return Input.GetKey(keycode);
            case PressType.GetKeyDown:
                return Input.GetKeyDown(keycode);
            case PressType.GetKeyUp:
                return Input.GetKeyUp(keycode);
            default:
                return false;
        }
    }
}
