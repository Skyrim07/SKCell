using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SKCell
{
    public enum EnumNetworkType
    {
        None = 0,
        Wifi = 1,
        Mobile = 2,
        MobileEdge = 3,
        Mobile3G = 4,
        Mobile4G = 5
    }
    public class NativeUtility
    {
        //只返回简单的网络状态，None,Wifi,Mobile
        public static EnumNetworkType GetNetworkInfo()
        {
            int netInfo = 1;
#if UNITY_ANDROID && !UNITY_EDITOR
        netInfo = AndroidUtility.GetNetworkInfo_Android();
#elif UNITY_IOS && !UNITY_EDITOR
        netInfo = IphoneUtility.GetNetworkInfo_IOS();
#endif
            return (EnumNetworkType)netInfo;
        }
        public static float GetBatteryLevel()
        {
            /*#if UNITY_ANDROID && !UNITY_EDITOR
                    return AndroidUtility.GetBatteryLevel_Android();
            #elif UNITY_IOS && !UNITY_EDITOR
                    return IphoneUtility.GetBatteryLevel_IOS();
            #endif*/
            return UnityEngine.SystemInfo.batteryLevel;
        }

        public static UnityEngine.BatteryStatus GetBatteryStatus()
        {
            return UnityEngine.SystemInfo.batteryStatus;
        }

        public static string GetMacAddress()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return AndroidUtility.GetMacAddress_Android();
#elif UNITY_IOS && !UNITY_EDITOR
        return IphoneUtility.GetMacAddress_IOS();
#endif
            return "";
        }
        public static string GetCarrierName()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return AndroidUtility.GetCarrierName_Android();
#elif UNITY_IOS && !UNITY_EDITOR
        return IphoneUtility.GetCarrierName_IOS();
#endif
            return "";
        }
        //返回详细的网络状态，None,Wifi,Mobile_Edge, Mobile_3G, Mobile_4G
        public static EnumNetworkType GetDetailNetworkInfo()
        {
            int netInfo = 1;
#if UNITY_ANDROID && !UNITY_EDITOR
        netInfo = AndroidUtility.GetDetailNetworkInfo_Android();
#elif UNITY_IOS && !UNITY_EDITOR
        netInfo = IphoneUtility.GetDetailNetworkInfo_IOS();
#endif
            return (EnumNetworkType)netInfo;
        }

        public static string GetPushToken()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return AndroidUtility.GetPushToken_Android();
#elif UNITY_IOS && !UNITY_EDITOR
        return IphoneUtility.GetPushToken_IOS();
#endif
            return "";
        }

        public static string GetSystemTime()
        {
            DateTime time = DateTime.Now;
            return time.ToShortTimeString();
        }

        /// <summary>
        /// IP地址转换
        /// </summary>
#if UNITY_IOS && !UNITY_EDITOR
	public static string ConvertIPAddress(string ipv4, string port)
	{
		return IphoneUtility.ConvertIPAddress_IOS(ipv4, port);
	}
#endif
    }
}
