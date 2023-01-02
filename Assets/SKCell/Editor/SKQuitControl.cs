using UnityEngine;
using UnityEditor;

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
        float t = Time.realtimeSinceStartup;
        if (EditorUtility.DisplayDialog("Are you sure to exit?", $"Take a break!\nCurrent session time: {(int)t / 3600}h {(int)(t%3600)/60}m {(int)t%60}s", "Exit", "Cancel")){
            return true;
        }
        return false;
    }
}
