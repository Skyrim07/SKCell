using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace SKCell
{
    public class IphoneUtility
    {
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern int GetNetworkInfo();
    [DllImport("__Internal")]
    private static extern float GetBatteryLevel();
    [DllImport("__Internal")]
    private static extern string GetMacAddress();
    [DllImport("__Internal")]
    private static extern string GetCarrierName();
    [DllImport("__Internal")]
    private static extern int GetDetailNetworkInfo();
	[DllImport("__Internal")]
	private static extern string ConvertIPAddress (string ipv4, string port);
	[DllImport("__Internal")]
	private static extern void SaveImage (IntPtr imageData, int length);
    [DllImport("__Internal")]
    private static extern bool IsJailBreak();
    [DllImport("__Internal")]
    private static extern string GetAppleId();
#endif
        public static int GetNetworkInfo_IOS()
        {
#if UNITY_IOS
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            return GetNetworkInfo();
        }
#endif
            return 0;
        }
        public static float GetBatteryLevel_IOS()
        {
#if UNITY_IOS
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            return GetBatteryLevel();
        }
#endif
            return 0;
        }

        public static string GetMacAddress_IOS()
        {
#if UNITY_IOS
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            return GetMacAddress();
        }
#endif
            return "";
        }

        public static string GetCarrierName_IOS()
        {
#if UNITY_IOS
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            return GetCarrierName();
        }
#endif
            return "";
        }

        public static int GetDetailNetworkInfo_IOS()
        {
#if UNITY_IOS
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            return GetDetailNetworkInfo();
        }
#endif
            return 0;
        }

        public static string GetPushToken_IOS()
        {
#if UNITY_IOS
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            byte[] tokenbytes = UnityEngine.iOS.NotificationServices.deviceToken;
            if (tokenbytes != null)
            {
                string token = System.BitConverter.ToString(tokenbytes).Replace("-", "").ToLower();
                return token;
            }
        }
#endif

            return "";
        }

        public static string ConvertIPAddress_IOS(string ipv4, string port)
        {
#if UNITY_IOS
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			return ConvertIPAddress(ipv4, port);
		}
#endif
            return ipv4;
        }

        public static void SaveImage_IOS(IntPtr imageData, int length)
        {
#if UNITY_IOS
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			SaveImage(imageData, length);
		}
#endif
        }

        public static bool IsJailBreak_IOS()
        {
#if UNITY_IOS && !UNITY_EDITOR
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			return IsJailBreak();
		}
#endif
            return false;
        }
        public static string GetAppleId_IOS()
        {
#if UNITY_IOS
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			return GetAppleId();
		}
#endif
            return "";
        }
    }
}
