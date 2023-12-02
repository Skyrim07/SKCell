using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for SKUI Panels. Inherit from this class to connect with the SKUI system.
/// </summary>

namespace SKCell
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SKUIAnimation))]
    [AddComponentMenu("SKCell/UI/SKUIPanel")]
    public class SKUIPanel : MonoBehaviour
    {
        [Header("General")]
        public SKUIPanel rootPanel;
        [HideInInspector] public List<SKUIPanel> leafPanels = new List<SKUIPanel>();
        [HideInInspector] public int lastActivatedLeaf=0;

        public int panelID = -1;
        public SKUIPanelState initialState = SKUIPanelState.Inactive;
        [HideInInspector] public bool active = false;

        [Header("Hierachy Management")]
        public SKUIPanelLeafManagementMethod leafPanelMethod = SKUIPanelLeafManagementMethod.OneLeafPanelActive;
        [Range(0f,2f)]
        public float leafActivationLag = 0f;

        public Action<bool> onStateChanged;

        private Dictionary<int, SKUIPanel> leafPanelDict = new Dictionary<int, SKUIPanel>();
        private SKUIAnimation anim;

        private void Awake()
        {
            onStateChanged += (b) => { };
            anim = SKUtils.GetComponentNonAlloc<SKUIAnimation>(gameObject);
            if (rootPanel != null)
            {
                rootPanel.leafPanels.Add(this);
            }
            InitializeDictionary();
            InitializeState();
        }
       
        #region Methods
        /// <summary>
        /// Set the state of the current panel. Will affect its children.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(SKUIPanelState state)
        {
            if (state == SKUIPanelState.Active)
            {
                OnPanelActivated();
            }
            else
            {
                OnPanelDeactivated();
            }
        }
        public void SetState(bool appear)
        {
            if (appear)
            {
                OnPanelActivated();
            }
            else
            {
                OnPanelDeactivated();
            }
        }
        /// <summary>
        /// Activate the given leaf panel.
        /// </summary>
        public void ActivateLeaf(SKUIPanel leaf)
        {
            if (leaf == null)
                return;
            lastActivatedLeaf = leaf.panelID;
            StartCoroutine(ActivateLeafCR(leaf));
        }
        public void ActivateLeaf(int leafPanelID)
        {
            lastActivatedLeaf = leafPanelID;
            SKUIPanel leaf = leafPanelDict[leafPanelID];
            if (leaf == null)
                return;
            StartCoroutine(ActivateLeafCR(leaf));
        }
        private IEnumerator ActivateLeafCR(SKUIPanel leaf)
        {
            //Activate the given panel and deactivate the rest.
            if (leafPanelMethod == SKUIPanelLeafManagementMethod.OneLeafPanelActive)
            {
                for (int i = 0; i < leafPanels.Count; i++)
                {
                        leafPanels[i].SetState(SKUIPanelState.Inactive);
                }
                yield return new WaitForSecondsRealtime(leafActivationLag);
                leaf.SetState(SKUIPanelState.Active);
            }
        }
        /// <summary>
        /// Deactivate the given leaf panel.
        /// </summary>
        public void DeactivateLeaf(int leafPanelID)
        {

        }

        private void InitializeDictionary()
        {
            //Build leaf dictionary
            foreach(var item in leafPanels)
            {
                SKUtils.InsertOrUpdateKeyValueInDictionary(leafPanelDict, item.panelID, item);
            }
            //Build master dictionary
            SKUtils.InsertOrUpdateKeyValueInDictionary(SKUIPanelManager.panelIDDict, panelID, this);
            SKUIPanelManager.existingPanelList.Add(this);
        }
        private void InitializeState()
        {
            SetState(initialState);
        }
        #endregion
        #region Virtual Events
        public virtual void OnPanelActivated()
        {
            if (active)
                return;

            active = true;
            if(anim)
            anim.SetState(true);
            onStateChanged(true);
        }
        public virtual void OnPanelDeactivated()
        {
            if (!active)
                return;

            active = false;
            if (anim)
                anim.SetState(false);
            onStateChanged(false);
        }
        /// <summary>
        /// Called by SKUIPanelManager when instantiated
        /// </summary>
        public virtual void OnPanelInstantiated()
        {

        }
        #endregion
    }
    public enum SKUIPanelLeafManagementMethod
    {
        OneLeafPanelActive
    }
    public enum SKUIPanelState
    {
        Active,
        Inactive
    }
}
