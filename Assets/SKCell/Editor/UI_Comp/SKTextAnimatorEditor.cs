using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SKCell
{
    [CustomEditor(typeof(SKTextAnimator))]
    [CanEditMultipleObjects]
    public class SKTextAnimatorEditor : Editor
    {
        public SKTextAnimator anim;
        bool expanded = false;

        GUIStyle bold = new GUIStyle(EditorStyles.boldLabel);
        private void OnEnable()
        {
            anim = (SKTextAnimator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!expanded)
            {
                GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
                if (GUILayout.Button("< View effect commands >"))
                {
                    expanded = true;
                }
                GUI.contentColor = Color.white;
            }
            else
            {
                GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
                if (GUILayout.Button("< Hide effect commands >"))
                {
                    expanded = false;
                }
                GUI.contentColor = Color.white;
                GUILayout.Label("To use command: <effectName(arg0,arg1...)> your text </>", bold);
                GUILayout.Label("1. Shake: <shake>        <shake(time)>");
                GUILayout.Label("2. Banner: <banner(time,r,g,b)>            ");
                GUILayout.Label("3. Fade: <fade>          <fade(speed)>");
                GUILayout.Label("4. Twinkle: <twinkle>          <twinkle(speed)>");
                GUILayout.Label("5. Dangle: <dangle>          <dangle(speed)>");
                GUILayout.Label("6. Exclaim: <excl>          <excl(speed,r,g,b)>");
                GUILayout.Label("7. Timed Exclaim: <exclt>          <exclt(speed,time,r,g,b)>");
                GUILayout.Label("8. Wave: <wave>          <wave(speed)>");
                GUILayout.Label("9. Scale Up: <scaleup>          <scaleup(speed)>");
                GUILayout.Label("10. Scale Down: <scaledn>          <scaledn(speed)>");
                GUILayout.Label("11. Rotate: <rot(speed,angle)>");
                GUILayout.Label("12. Color: <col(speed,time,r,g,b)>");
            }

        }
    }
}
