using UnityEngine;
using UnityEditor;
using System;

public class SKQuitControl
{
    [InitializeOnLoadMethod]
    static void InitializeOnLoadMethod()
    {
        EditorApplication.wantsToQuit -= Quit;
        EditorApplication.wantsToQuit += Quit;
    }

    static bool Quit()
    {
        DateTime now = DateTime.Now;
        if (EditorUtility.DisplayDialog("Are you sure to exit?", $"Take a break!\n" +
            $"Current time: {now.Hour.ToString("d2")} : {now.Minute.ToString("d2")} : {now.Second.ToString("d2")}",
            "Quit Unity", "Cancel")){
            return true;
        }
        return false;
    }
}
