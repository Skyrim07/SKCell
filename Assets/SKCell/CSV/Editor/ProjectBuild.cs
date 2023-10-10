using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor.Build.Content;

public class ProjectBuild : Editor {
    private static string[] s;
    static void BuildPackage()
    {
        s = System.Environment.GetCommandLineArgs();
        var target = s[s.Length - 3];
       
        if (target.Equals("1")) //android
        {
            BuildForAndroid();
        }
        else if (target.Equals("2"))  //ios
        {
            BuildForIOS();
        }
        s = null;
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

    static void BuildForAndroid()
    {
        string[] levels = GetBuildScenes();
        string path= GetExportPath(BuildTarget.Android);
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        BuildPipeline.BuildPlayer(levels, path, BuildTarget.Android, BuildOptions.None);
    }
    static void BuildForIOS()
    {
        string[] levels = GetBuildScenes();
        string path = GetExportPath(BuildTarget.iOS);
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        BuildPipeline.BuildPlayer(levels, path, BuildTarget.iOS, BuildOptions.None);
    }
       
    private static string GetExportPath(BuildTarget target)
    {
        string path = s[s.Length - 2];
        string name = string.Empty;
        if (target == BuildTarget.Android)
        {
            name = "/" + s[s.Length - 1] + ".apk";
        }
        else if (target == BuildTarget.iOS)
        {
            name = "/" + s[s.Length - 1];
        }
        else if (target == BuildTarget.StandaloneWindows)
        {
            name = "/" + s[s.Length - 1] + ".exe";
        }
        UnityEngine.Debug.Log(path);
        UnityEngine.Debug.Log(name);
        string exepath = @path + name;
        UnityEngine.Debug.Log(exepath);
        return exepath;
    }
    private static string companyName{
        get {
            return s[s.Length - 5];
        }
    }
    private static string productName {
        get {
            return s[s.Length - 4];
        }
    }
    }