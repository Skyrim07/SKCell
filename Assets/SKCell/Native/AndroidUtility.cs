using System;
using UnityEngine;

namespace SKCell
{
    class AndroidUtility
    {
        private const string SDK_PACKAGE = "com.netease.tools.Utility";
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaClass jc = new AndroidJavaClass(SDK_PACKAGE);
#endif

        public static int GetNetworkInfo_Android()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return jc.CallStatic<int>("GetNetworkInfo");
#else
            return 0;
#endif
        }
        public static float GetBatteryLevel_Android()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return jc.CallStatic<float>("GetBatteryLevel");
#else
            return 0.0f;
#endif
        }

        public static string GetMacAddress_Android()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return jc.CallStatic<string>("GetMacAddress");
#else
            return "";
#endif
        }

        public static string GetCarrierName_Android()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return jc.CallStatic<string>("GetCarrierName");
#else
            return "";
#endif
        }

        public static int GetDetailNetworkInfo_Android()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return jc.CallStatic<int>("GetDetailNetworkInfo");
#else
            return 0;
#endif
        }

        public static string GetPushToken_Android()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return jc.CallStatic<string>("GetPushToken");
#else
            return "";
#endif
        }
    }
}
