using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace SKCell
{
    /// <summary>
    /// Manages all the ui panels in the game
    /// </summary>
    [AddComponentMenu("SKCell/UI/SKUIPanelManager")]
    public class SKUIPanelManager : SKMonoSingleton<SKUIPanelManager>
    {
        public static string[] panelHierarcyName = { "UIRoot","UILowermost -- 1", "UILow -- 2", "UIMain -- 3", "UIHigh -- 4", "UIHigher -- 5", "UITopmost -- 6", "UIConstant -- 7" };
        public static int[] panelHierarcyValue = { 0, 100,200,300,400,500,600,700};

        public static Dictionary<int, SKUIPanel> panelIDDict = new Dictionary<int, SKUIPanel>();
        public static List<SKUIPanel> existingPanelList = new List<SKUIPanel>();

        public static Dictionary<SKUIPanelHierarchy, GameObject> panelHierDict = new Dictionary<SKUIPanelHierarchy, GameObject>();
        public static Dictionary<int, GameObject> panelPrefabDict = new Dictionary<int, GameObject>();

        protected override void Awake()
        {
            base.Awake();
            InitializePrefabReference();
            InitializeHierarchyReference();
           // SKCore.OnSceneLoaded000 += () => Invoke("InitializeHierarchyReference", 0.1f);
        }
        private void InitializePrefabReference()
        {
            string path = SKAssetLibrary.PANEL_PREFAB_PATH;
            string resPath = path.Substring(path.IndexOf("SKCell"));
            string[] files = Directory.GetFiles(Application.dataPath + path);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".prefab"))
                {
                    files[i] = files[i].Replace(".prefab", "");
                    string suffix = files[i].Substring(files[i].IndexOf("_") + 1);
                    int panelID = int.Parse(suffix.TrimStart());
                    SKUtils.InsertOrUpdateKeyValueInDictionary(panelPrefabDict, panelID, Resources.Load(files[i].Substring(files[i].IndexOf("SKCell"))) as GameObject);
                }
            }
        }
        private void InitializeHierarchyReference()
        {
            Transform rootTF = GameObject.Find(panelHierarcyName[0])?.transform;
            if (rootTF == null)
            {
                SKUtils.EditorLogWarning("UI Panel Hierarchy not Initialized!");
                return;
            }
            for (int i =0; i < panelHierarcyName.Length-1; i++)
            {
                SKUtils.InsertOrUpdateKeyValueInDictionary(panelHierDict, (SKUIPanelHierarchy)(i+1), rootTF.GetChild(i).gameObject);
            }
        }
        #region Public Methods
        public SKUIPanel GetPanelByID(int id)
        {
            return SKUtils.GetValueInDictionary(panelIDDict, id);
        }
        public bool? IsActive(int panelID)
        {
            return panelIDDict[panelID]?.active;
        }

        public int GetActivePanelCount()
        {
            return existingPanelList.Count;
        }
        /// <summary>
        ///  Instantiate a new panel.
        /// </summary>
        /// <param name="panelID"></param>
        /// <param name="active">The initial state of the panel.</param>
        /// <param name="hierarchy">The hierarchy canvas.</param>
        /// <param name="predecessor">The id of the panel behind it. If not given, it will be instantiated as a direct child of the given parent.</param>
        ///  <param name="parent">The parent of the instantiated panel. Will only be used if predecessor is not given.</param>
        public void InstantiatePanel(int panelID, SKUIPanelState state, int predecessor = -1, Transform parent = null)
        {
            GameObject toInst = panelPrefabDict[panelID];
            if (toInst == null)
            {
                SKUtils.EditorLogError($"UI panel prefab with the given ID: {panelID} not found. Check Resources/SKCell/UI/Panels/xxx_{panelID} for the corresponding prefab.");
                return;
            }
            if (predecessor < 0 && parent == null)
                return;
            GameObject panelGO = SKPoolManager.SpawnObject(toInst);
            if (predecessor >= 0)
            {
                SKUIPanel pre = panelIDDict[predecessor];
                if (pre == null)
                {
                    SKUtils.EditorLogError($"UI panel with the given ID: {predecessor} not found when trying to instantiate a panel.");
                    return;
                }
                Transform _parent =pre.transform.parent;
                panelGO.transform.SetParent(_parent);
                panelGO.transform.SetSiblingIndex(pre.transform.GetSiblingIndex() + 1);
            }
            else
            {
                panelGO.transform.SetParent(parent);
            }
            SKUIPanel panel = SKUtils.GetComponentNonAlloc<SKUIPanel>(panelGO);
            if (panel == null)
            {
                SKUtils.EditorLogError($"UI panel with the given ID: {panelID} does not have the component SKUIPanel.");
                return;
            }
            panel.SetState(state);
            panel.OnPanelInstantiated();
        }
        /// <summary>
        ///  Instantiate a new panel as a direct child of the given hierarchy canvas.
        /// </summary>
        /// <param name="panelID"></param>
        /// <param name="active">The initial state of the panel.</param>
        /// <param name="hierarchy">The hierarchy canvas.</param>
        public void InstantiatePanel(int panelID, SKUIPanelState state, SKUIPanelHierarchy hierarchy)
        {
            GameObject toInst = panelPrefabDict[panelID];
            if (toInst == null)
            {
                SKUtils.EditorLogError($"UI panel prefab with the given ID: {panelID} not found. Check Resources/SKCell/UI/Panels/xxx_{panelID} for the corresponding prefab.");
                return;
            }
            GameObject panelGO = SKPoolManager.SpawnObject(toInst);
            if (panelHierDict[hierarchy] == null)
            {
                SKUtils.EditorLogWarning("UI Panel Hierarchy not Initialized!");
                return;
            }
            panelGO.transform.SetParent(panelHierDict[hierarchy].transform);
            SKUIPanel panel = SKUtils.GetComponentNonAlloc<SKUIPanel>(panelGO);
            if (panel == null)
            {
                SKUtils.EditorLogError($"UI panel with the given ID: {panelID} does not have the component SKUIPanel.");
                return;
            }
            panel.SetState(state);
            panel.OnPanelInstantiated();
        }
        /// <summary>
        /// Destroy a UI Panel.
        /// </summary>
        /// <param name="panelID"></param>
        public void DisposePanel(int panelID)
        {
            if (!panelIDDict.ContainsKey(panelID))
            {
                SKUtils.EditorLogError($"UI panel with the given ID: {panelID} is not present in scene.");
                return;
            }
            SKUtils.RemoveKeyInDictionary(panelIDDict,panelID);
            SKUtils.RemoveFromList(existingPanelList,GetPanelByID(panelID));
            SKUtils.SafeDestroy(GetPanelByID(panelID).gameObject);
        }
        /// <summary>
        /// Activate an existing panel.
        /// </summary>
        public void ActivatePanel(int panelID)
        {
            if (!existingPanelList.Contains(GetPanelByID(panelID)))
            {
                SKUtils.EditorLogWarning("Cannot activate an non-existing panel.");
                return;
            }
            GetPanelByID(panelID).SetState(SKUIPanelState.Active);
        }
        /// <summary>
        /// Activate an existing panel.
        /// </summary>
        public void ActivatePanel(SKUIPanel panel)
        {
            panel.SetState(SKUIPanelState.Active);
        }
        /// <summary>
        ///  Deactivate an existing panel.
        /// </summary>
        public void DeactivatePanel(int panelID)
        {
            if (!existingPanelList.Contains(GetPanelByID(panelID)))
            {
                SKUtils.EditorLogWarning("Cannot deactivate an non-existing panel.");
                return;
            }
            GetPanelByID(panelID).SetState(SKUIPanelState.Inactive);
        }
        /// <summary>
        ///  Deactivate an existing panel.
        /// </summary>
        public void DeactivatePanel(SKUIPanel panel)
        {
           panel.SetState(SKUIPanelState.Inactive);
        }

        #endregion
    }

    public enum SKUIPanelHierarchy
    {
        UIRoot,
        UILowermost,
        UILow,
        UIMain,
        UIHigh,
        UIHIgher,
        UITopmost,
        UIConstant
    }
}
