using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Base level flow control
/// </summary>
namespace SKCell
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SKCommonTimer))]
    [RequireComponent(typeof(SKPoolManager))]
    [AddComponentMenu("SKCell/SKCore")]
    public sealed class SKCore : MonoSingleton<SKCore>
    {
        public const string SKCELL_VERSION = "v0.13.4";

        public static Action Awake000 = new Action(EmptyAction), Awake100 = new Action(EmptyAction),
                              Start000 = new Action(EmptyAction), Start100 = new Action(EmptyAction), Start200 = new Action(EmptyAction),
                              Tick000 = new Action(EmptyAction), Tick100 = new Action(EmptyAction), Tick200 = new Action(EmptyAction), Tick300 = new Action(EmptyAction), Tick400 = new Action(EmptyAction), Tick500 = new Action(EmptyAction),
                              FixedTick000 = new Action(EmptyAction), FixedTick100 = new Action(EmptyAction), FixedTick200 = new Action(EmptyAction),
                              LateTick000 = new Action(EmptyAction), LateTick100 = new Action(EmptyAction), LateTick200 = new Action(EmptyAction),
                              OnSceneLoaded000 = new Action(EmptyAction), OnSceneLoaded100 = new Action(EmptyAction);

        public static Dictionary<SKModule, Type> moduleTypeDict = new Dictionary<SKModule, Type>()
        {
            {SKModule.CoreModule, null},
            {SKModule.LocalizationModule, typeof(SKLocalizationManager)},
        };
        private List<SKModuleBase> modules = new List<SKModuleBase>();

        private GameObject SKRoot;

        /// <summary>
        /// Add an SK Module to the active pool.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        internal SKModuleBase AddModule(SKModule module)
        {
            SKModuleBase m = Activator.CreateInstance(moduleTypeDict[module]) as SKModuleBase;
            CommonUtils.InsertToList(modules, m, false);
            modules.Sort(new SKModuleComparer());

            m.Initialize();

            return m;
        }

        /// <summary>
        /// Remove an SK Module from the active pool.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        internal SKModuleBase RemoveModule(SKModule module)
        {
            SKModuleBase m = GetModule(module);
            CommonUtils.RemoveFromList(modules, m);
            modules.Sort(new SKModuleComparer());

            m.Dispose();

            return m;
        }

        /// <summary>
        /// Get an SK Module from the active pool.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        internal SKModuleBase GetModule(SKModule module)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                Type t = modules[i].GetType();
                if (t == moduleTypeDict[module])
                {
                    return modules[i];
                }
            };

            CommonUtils.EditorLogError("SKCore.GetModule(): The target module is not active. Try AddModule() first.");
            return null;
        }

        private void InitializeSKCell()
        {
            foreach (var item in modules)
            {
                item.Start();
            }

            CommonUtils.EditorLogNormal("SKCell Initialized!");
        }

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            CommonUtils.ClearAllCustomMeshes();

            Awake000();
            Awake100();
        }
        private void Start()
        {
            InitializeSKCell();
            Start000();
            Start100();
            Start200();
        }
        private void Update()
        {
            Tick000();

            foreach (var item in modules)
            {
                item.Tick();
            }

            Tick100();
            Tick200();
            Tick300();
            Tick400();
            Tick500();
        }
        private void FixedUpdate()
        {
            FixedTick000();

            foreach (var item in modules)
            {
                item.FixedTick();
            }

            FixedTick100();
            FixedTick200();
        }

        private void LateUpdate()
        {
            LateTick000();
            LateTick100();
            LateTick200();
        }
        //private void OnLevelWasLoaded(int level)
        //{
        //    OnSceneLoaded000();
        //    OnSceneLoaded100();
        //}

        #endregion
        private static void EmptyAction() { }
    }

    /// <summary>
    /// Represents an SK module manager class.
    /// </summary>
    public enum SKModule
    {
        CoreModule,
        InputModule,
        MediaModule,
        EnvironmentModule,
        LocalizationModule,
        CSVModule,
        FSMModule,
        GridModule,
        ObjectPoolModule,
        TimeModule,
        PhysicsModule,
        UIModule,
        StructureModule
    }
}
