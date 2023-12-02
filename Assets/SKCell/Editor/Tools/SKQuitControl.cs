using UnityEditor;
using System;
namespace SKCell
{
    public class SKQuitControl
    {
        private static DateTime? editorStartTime = null;

        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            if (editorStartTime == null)
            {
                if (EditorPrefs.GetBool("IsRecompile", false))
                {
                    string storedTime = EditorPrefs.GetString("EditorRecompileStartTime", DateTime.Now.ToString());
                    editorStartTime = DateTime.Parse(storedTime);
                }
                else
                {
                    editorStartTime = DateTime.Now;
                    EditorPrefs.SetString("EditorRecompileStartTime", editorStartTime.Value.ToString());
                }
            }
            EditorPrefs.SetBool("IsRecompile", true);

            EditorApplication.wantsToQuit -= Quit;
            EditorApplication.wantsToQuit += Quit;
            EditorApplication.quitting += OnEditorQuit;
        }

        static void OnEditorQuit()
        {
            EditorPrefs.SetBool("IsRecompile", false);
        }

        static bool Quit()
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsedTime = now - editorStartTime.Value;

            string elapsedTimeString = $"{elapsedTime.Hours.ToString("d2")}:{elapsedTime.Minutes.ToString("d2")}:{elapsedTime.Seconds.ToString("d2")}";

            if (EditorUtility.DisplayDialog("Take a break !", $"You've been working for {elapsedTimeString}.\n",
                "Quit Unity", "Cancel"))
            {
                return true;
            }
            return false;
        }
    }
}