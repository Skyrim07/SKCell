using UnityEngine;

namespace SKCell.Test
{
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
}
