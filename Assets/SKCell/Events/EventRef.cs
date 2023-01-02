using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Store event identifiers. Used in EventDispatcher.
    /// </summary>
    public sealed class EventRef
    {
        //~1000
        #region Common
        public const int CM_EVENT_0 = 1001;

        public static readonly int CM_ON_SCENE_LOADED = 1100;
        public static readonly int CM_ON_SCENE_EXIT = 1101;
        #endregion

        //~2000
        #region Player
        // Main module
        public static readonly int PLAYER_MAIN_MODULE_START = 2001;

        // Movement module
        public static readonly int PLAYER_MOVEMENT_MODULE_START = 2101;

        public static readonly int PLAYER_ON_INPUT_JUMP = 2104;
        public static readonly int PLAYER_ON_TOUCH_GROUND = 2105;
        public static readonly int PLAYER_ON_REACH_MAX_JUMP = 2106;

        public static readonly int PLAYER_ON_START_DASH = 2107;
        public static readonly int PLAYER_ON_END_DASH = 2108;

        //Lifecycle Module
        public static readonly int PLAYER_ON_SPAWN = 2201;
        public static readonly int PLAYER_ON_DIE = 2202;

        //Combat Module
        public static readonly int PLAYER_ON_ATTACK_FINISH = 2301;
        public static readonly int PLAYER_ON_ATTACK_MOVEMENT_START = 2302;
        public static readonly int PLAYER_ON_ATTACK_MOVEMENT_END = 2303;
        #endregion

        //~3000
        #region Item

        #endregion

        //~4000
        #region Combat

        #endregion

        //~5000
        #region UI
        public static readonly int UI_CONV_ON_NEXT_SENTENCE = 5100;
        public static readonly int UI_CONV_ON_SELECT_OPTION = 5101;
        #endregion

        //~6000
        #region Env

        #endregion

        //~7000
        #region FX

        #endregion

        //~9000
        #region Console
        public static readonly int CONSOLE_ON_OPEN = 9001;
        public static readonly int CONSOLE_ON_CLOSE = 9002;

        #endregion
    }
}

