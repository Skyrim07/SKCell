using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;
public class Input_DialogueScene : MonoBehaviour
{
    [SerializeField] SKDialoguePlayer player;
    void Start()
    {
        CommonUtils.InvokeAction(.2f, () =>
        {
            player.Play();
        });
    }
}
