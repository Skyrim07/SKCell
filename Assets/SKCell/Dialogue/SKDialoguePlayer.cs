using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SKCell
{
    [AddComponentMenu("SKCell/Dialogue/SKDialoguePlayer")]
    public class SKDialoguePlayer : MonoBehaviour
    {
        public SKDialogueAsset asset;
        public bool playOnStart = false;
        [Header("Scene Components")]
        public SKUIPanel panel;
        public SKText contentText;
        public SKText speakerText;
        public SKUIPanel choicePanel;
        public GameObject[] choiceObjects;
        public SKText[] choiceTexts;
        public SKImage speakerImage;
        [Header("Buttons")]
        public Button contentButton;
        public SKButton[] choiceButtons;

        [Header("Events")]
        public UnityEvent onDialogueEnded;
        public UnityEvent onDialogueStart;

        private SKTextAnimator textAnimator;
        private SKDialogueEditorNode curNode, curSentence;
        private List<SKDialogueEditorNode> curChoices;
        private bool typewriterFinishExecuted = false, curSentenceLinkExecuted=false;

        private void Start()
        {
            textAnimator = contentText.GetComponent<SKTextAnimator>();
            textAnimator.onTypeWriterFinished += OnTypewriterFinished;
            contentButton.onClick.AddListener(OnContentButtonPressed);
            if (playOnStart)
                Play();
        }
        private void OnApplicationQuit()
        {
            if (asset != null)
            {
                asset.event_properties = new List<SKDE_EventProperty>();
                asset.int_properties = new List<SKDE_IntProperty> { };
            }
        }

        #region Public Methods

        /// <summary>
        /// Play the current dialogue asset.
        /// </summary>
        public void Play()
        {
            if (asset == null) return;
            SKDialogueManager.curEditorAsset = asset;
            curNode = asset.editorNodes[0];
            choicePanel.SetState(SKUIPanelState.Inactive);

            if(curChoices != null)
                curChoices.Clear(); 
            if (curNode.type!= SKDialogueEditorNodeType.Start)
            {
                SKUtils.EditorLogError("SKDialoguePlayer: First node is not start. Please make sure there is a start node to begin with.");
            }
            panel.SetState(SKUIPanelState.Active);
            StartPlay();
        }
        /// <summary>
        /// Play the given dialogue asset.
        /// </summary>
        public void Play(SKDialogueAsset asset)
        {
            this.asset = asset;
            Play();
        }

        /// <summary>
        /// Continue the dialogue flow by one step. (e.g. clicking on the sentence text -> fast forward / go to next sentence)
        /// </summary>
        public void SentenceNextStep()
        {
            OnContentButtonPressed();
        }

        /// <summary>
        /// Add listener to an event node according to its name.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public void AddListenerToEvent(string eventName, System.Action<float,float> callback)
        {
            bool isNew = true;
            foreach (var item in asset.event_properties)
            {
                if (item.name.Equals(eventName))
                {
                    isNew = false;
                    item.action += callback;
                }
            }
            if (isNew)
            {
                SKDE_EventProperty e = new SKDE_EventProperty()
                {
                    name = eventName,
                    action = new System.Action<float, float>((a, b) => { }),
                };
                asset.event_properties.Add(e);
                e.action += callback;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// When reached the last node
        /// </summary>
        private void OnEndSequence()
        {
            panel.SetState(SKUIPanelState.Inactive);
            onDialogueEnded.Invoke();
        }
        private void ExecuteNode(SKDialogueEditorNode node)
        {
            curNode = node;
            switch (node.type)
            {
                case SKDialogueEditorNodeType.Sentence:
                    if (node != curSentence)
                        curSentenceLinkExecuted = false;

                    curSentence = node;
                    typewriterFinishExecuted = false;
                    if (node.textType == SKDialogueEditorNodeTextType.Text)
                    {
                        contentText.UpdateTextDirectly(node.info.content);
                        speakerText.UpdateTextDirectly(node.info.speaker);
                    }
                    else
                    {
                        contentText.UpdateLocalID(node.info.content_localID);
                        speakerText.UpdateLocalID(node.info.speaker_localID);
                    }
                    if (node.info.avatar != null)
                    {
                        speakerImage.sprite = Sprite.Create(node.info.avatar, new Rect(0, 0, node.info.avatar.width, node.info.avatar.height), Vector2.zero);
                    }
                    break;
                case SKDialogueEditorNodeType.Choice:
                    break;
                case SKDialogueEditorNodeType.Random:
                    int count = node.linkedNodesID.Count;
                    ExecuteNode(node.linkedNodes[Random.Range(0, count)]);
                    break;
                case SKDialogueEditorNodeType.Event:
                    bool isNew = true;
                    foreach (var item in asset.event_properties)
                    {
                        if (item.name.Equals(node.event_name))
                        {
                            isNew = false;
                            item.action.Invoke(node.event_arg0, node.event_arg1);
                        }
                    }
                    if (isNew)
                    {
                        SKDE_EventProperty e = new SKDE_EventProperty()
                        {
                            name = node.event_name,
                            action = new System.Action<float, float>((a, b) => { }),
                        };
                        asset.event_properties.Add(e);
                        e.action.Invoke(node.event_arg0, node.event_arg1);
                    }
                    break;
                case SKDialogueEditorNodeType.Start:
                    ExecuteLinkedNodes(curNode);
                    break;
                case SKDialogueEditorNodeType.Set:
                    isNew = true;
                    foreach (var item in asset.int_properties)
                    {
                        if (item.name.Equals(node.int_property_name))
                        {
                            isNew = false;
                            item.value = node.int_property_value;
                        }
                    }
                    if (isNew)
                    {
                        asset.int_properties.Add(new SKDE_IntProperty()
                        {
                            name = node.int_property_name,
                            value = node.int_property_value,
                        });
                    }
                    break;
                case SKDialogueEditorNodeType.If:
                    bool success = true;
                    foreach (var item in asset.int_properties)
                    {
                        if (item.name.Equals(node.if_property_name))
                        {
                            switch (node.if_comparator)
                            {
                                case SKDE_Comparator.Equals:
                                    if (item.value != node.if_property_value)
                                        success = false;
                                    break;
                                case SKDE_Comparator.GreaterThan:
                                    if (item.value <= node.if_property_value)
                                        success = false;
                                    break;
                                case SKDE_Comparator.LessThan:
                                    if (item.value >= node.if_property_value)
                                        success = false;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (success)
                    {
                        ExecuteLinkedNodes(curNode);
                    }
                    break;
                case SKDialogueEditorNodeType.End:
                    OnEndSequence();
                    break;
                default:
                    break;
            }
        }
        private void StartPlay()
        {
            onDialogueStart.Invoke();
            ExecuteNode(curNode);
        }
        private void OnTypewriterFinished()
        {
            if (!typewriterFinishExecuted)
            {
                typewriterFinishExecuted = true;
                GotoChoices(curNode);
                ExecuteInstantNodes(curNode);
            }
        }
        private void OnContentButtonPressed()
        {
            if (curSentence.linkedNodesID.Count == 0)
            {
                OnEndSequence();
            }
            if (!typewriterFinishExecuted)
            {
                textAnimator.TypewriterFastForward();
                OnTypewriterFinished();
            }
            else
            {
                if (!curSentenceLinkExecuted)
                {
                    curSentenceLinkExecuted = true;
                    ExecuteLinkedNodesExclueInstant(curSentence);
                }
            }
        }
        private void OnChoiceButtonPressed(int id)
        {
            choicePanel.SetState(SKUIPanelState.Inactive);
            ExecuteLinkedNodes(curChoices[id]);
        }
        private void ExecuteChoices(List<SKDialogueEditorNode> choices)
        {
            curChoices = choices;
            choicePanel.SetState(choices.Count==0?SKUIPanelState.Inactive:SKUIPanelState.Active);
            foreach (var go in choiceObjects)
                SKUtils.SetActiveEfficiently(go, false);
            for (int i = 0; i < choices.Count; i++)
            {
                SKUtils.SetActiveEfficiently(choiceObjects[i], true);
                if(choices[i].textType == SKDialogueEditorNodeTextType.Text)
                {
                    choiceTexts[i].UpdateTextDirectly(choices[i].info.content);
                }
                else
                {
                    choiceTexts[i].UpdateLocalID(choices[i].info.content_localID);
                }
                int j = i;
                choiceButtons[i].RemoveAllListeners(SKButtonEventType.OnPressed);
                choiceButtons[i].AddListener( SKButtonEventType.OnPressed,() =>
                {
                    OnChoiceButtonPressed(j);
                });
            }
        }
        /// <summary>
        /// Display choices after a sentence
        /// </summary>
        /// <param name="node"></param>
        private void GotoChoices(SKDialogueEditorNode node)
        {
            if (node.linkedNodesID.Count == 0) return;

            List<SKDialogueEditorNode> choices = new List<SKDialogueEditorNode>();
            foreach (var link in node.linkedNodes)
            {
                if (link.type == SKDialogueEditorNodeType.Choice)
                {
                    choices.Add(link);
                }
            }
            ExecuteChoices(choices);
        }
        private void ExecuteInstantNodes(SKDialogueEditorNode node)
        {
            if (node.type != SKDialogueEditorNodeType.Sentence) return;
            if (node.linkedNodesID.Count == 0) return;
            List<SKDialogueEditorNode> list = new List<SKDialogueEditorNode>();
            foreach (var link in node.linkedNodes)
            {
                if (link.type == SKDialogueEditorNodeType.Set || link.type == SKDialogueEditorNodeType.Event)
                {
                    list.Add(link);
                }
            }
            foreach (var link in list)
            {
                ExecuteNode(link);
            }
        }
        private void ExecuteLinkedNodes(SKDialogueEditorNode node)
        {
            List<SKDialogueEditorNode> list = new List<SKDialogueEditorNode>();
            foreach (var link in node.linkedNodes)
            {
                if(link.type == SKDialogueEditorNodeType.Set|| link.type == SKDialogueEditorNodeType.Event)
                {
                    list.Insert(0, link);
                }
                else if(link.type != SKDialogueEditorNodeType.Choice)
                {
                    list.Add(link);
                }
            }
            foreach (var link in list)
            {
                ExecuteNode(link);
            }
        }
        private void ExecuteLinkedNodesExclueInstant(SKDialogueEditorNode node)
        {
            List<SKDialogueEditorNode> list = new List<SKDialogueEditorNode>();
            foreach (var link in node.linkedNodes)
            {
                if (link.type == SKDialogueEditorNodeType.Set || link.type == SKDialogueEditorNodeType.Event)
                {
                    
                }
                else if (link.type != SKDialogueEditorNodeType.Choice)
                {
                    list.Add(link);
                }
            }
            foreach (var link in list)
            {
                ExecuteNode(link);
            }
        }
        #endregion
    }
}
