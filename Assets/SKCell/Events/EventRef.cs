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
        public static readonly int CM_ON_GAME_PAUSE = 1102;

        //File
        public static readonly int CM_ON_SAVE = 1201;
        public static readonly int CM_ON_LOAD = 1202;
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
        public static readonly int PLAYER_ON_HEALTH_DEPLETED = 2300;
        public static readonly int PLAYER_ON_SANITY_DEPLETED = 2301;
        public static readonly int PLAYER_ON_ATTACK_FINISH = 2302;
        public static readonly int PLAYER_ON_ATTACK_MOVEMENT_START = 2303;


        public static readonly int PLAYER_ON_ATTACK_MOVEMENT_END = 2310;



        #endregion

        //~3000
        #region Item
        public static readonly int PLAYER_ON_USE_ITEM = 3000;
        public static readonly int FEATHER_ON_EQUIP = 3100;
        public static readonly int FEATHER_ON_UNEQUIP = 3101;
        public static readonly int FEATHER_MAIN_ON_REPLENISH = 3102;

        public static readonly int SANITY_CARD_ON_ACTIVATE = 3200;
        public static readonly int SANITY_CARD_ON_ACQUIRE = 3201;
        public static readonly int SANITY_CARD_ON_EQUIP = 3202;
        public static readonly int SANITY_CARD_ON_UNEQUIP = 3203;
        public static readonly int SANITY_CARD_ON_PROGRESS_CHANGE = 3205;
        #endregion

        //~4000
        #region Combat
        public static readonly int COMBAT_PLAYER_INJURED_NORMAL = 4001;
        public static readonly int COMBAT_PLAYER_INJURED_SEVERE = 4002;
        public static readonly int COMBAT_PLAYER_HEALED = 4003;
        public static readonly int COMBAT_PLAYER_SAN_INJURED_NORMAL = 4004;
        public static readonly int COMBAT_PLAYER_SAN_HEALED = 4006;


        public static readonly int COMBAT_FEATHER_SKILL_USED = 4100;
        public static readonly int COMBAT_FEATHER_SKILL_ON_FAIL = 4101;

        public static readonly int COMBAT_ON_ENEMY_DEFEATED = 4200;
        public static readonly int COMBAT_ON_ENEMY_SAN_ZERO = 4201;


        public static readonly int ON_ENTER_MEDITATION = 4501;
        public static readonly int ON_EXIT_MEDITATION = 4502;
        public static readonly int ON_ENTER_SINGING = 4503;
        public static readonly int ON_EXIT_SINGING = 4504;
        #endregion

        //~5000
        #region UI
        public static readonly int UI_ON_MAIN_PANEL_OPEN = 5000;
        public static readonly int UI_ON_MAIN_PANEL_CLOSE = 5001;


        public static readonly int UI_CONV_ON_NEXT_SENTENCE = 5100;
        public static readonly int UI_CONV_ON_SELECT_OPTION = 5101;

        public static readonly int UI_ON_RESTPOINT_OPEN = 5110;
        public static readonly int UI_ON_RESTPOINT_CLOSE = 5111;
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

