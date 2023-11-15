using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;

#endif
namespace SKCell
{
    [CustomEditor(typeof(SKToggle))]
    [CanEditMultipleObjects]
    public class SKToggleEditor : Editor
    {
        static SKToggle skToggle;
        public override void OnInspectorGUI()
        {
            skToggle = (SKToggle)target;
            if ((!skToggle.initialized) && UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null)
                if (GUILayout.Button("<Generate structure>", GUILayout.Height(30)))
                {
                    skToggle.GenerateStructure();
                }
            if (skToggle.initialized || UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                if (skToggle.transitionMode == SKToggle.SKToggleTransitionMode.Animation)
                {
                    GUILayout.Label("The animation template is automatically generated.");
                    if (!skToggle.hasAnimator && GUILayout.Button("<Generate animator>", GUILayout.Height(30)))
                    {
                        CreateController();
                    }
                }
                else if (skToggle.hasAnimator)
                {
                    if (GUILayout.Button("<Delete animation>", GUILayout.Height(30)))
                    {
                        DetachController();
                    }
                }
                base.OnInspectorGUI();
                if (!Application.isPlaying)
                    skToggle.DrawEditorPreview();
            }
        }
        private static void CreateController()
        {
            string path = SKAssetLibrary.UI_ANIM_DIR_PATH + $"/{skToggle.name}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            // Creates the controller
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path + $"/{skToggle.name}.controller");
            AssetDatabase.CreateAsset(new AnimationClip(), path + $"/ToggleOn.anim");
            AssetDatabase.CreateAsset(new AnimationClip(), path + $"/ToggleOff.anim");
            AssetDatabase.CreateAsset(new AnimationClip(), path + $"/Pressed.anim");
            AssetDatabase.CreateAsset(new AnimationClip(), path + $"/OverOn.anim");
            AssetDatabase.CreateAsset(new AnimationClip(), path + $"/OverOff.anim");
            AssetDatabase.CreateAsset(new AnimationClip(), path + $"/Disabled.anim");
            skToggle.AttachController(controller);



            // Add parameters
            controller.AddParameter("isOn", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Pressed", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("OverOn", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("OverOff", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Disabled", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("anyToOn", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("anyToOff", AnimatorControllerParameterType.Trigger);

            // Add StateMachines
            var rootStateMachine = controller.layers[0].stateMachine;

            // Add States
            var onState = rootStateMachine.AddState("ToggleOn");
            onState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/ToggleOn.anim");
            var offState = rootStateMachine.AddState("ToggleOff");
            offState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/ToggleOff.anim");
            var pressedState = rootStateMachine.AddState("Pressed");
            pressedState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Pressed.anim");
            var disabledState = rootStateMachine.AddState("Disabled");
            disabledState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Disabled.anim");
            var overonState = rootStateMachine.AddState("OverOn");
            overonState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/OverOn.anim");
            var overoffState = rootStateMachine.AddState("OverOff");
            overoffState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/OverOff.anim");

            rootStateMachine.defaultState = onState;

            // Add Transitions
            var onToOffTransition = onState.AddTransition(offState);
            onToOffTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "isOn");
            onToOffTransition.duration = 0.1f;

            var offToOnTransition = offState.AddTransition(onState);
            offToOnTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "isOn");
            offToOnTransition.duration = 0.1f;

            var anyToOnTransition = rootStateMachine.AddAnyStateTransition(onState);
            anyToOnTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "anyToOn");
            anyToOnTransition.duration = 0.1f;

            var anyToOffTransition = rootStateMachine.AddAnyStateTransition(offState);
            anyToOffTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "anyToOff");
            anyToOffTransition.duration = 0.1f;

            var pressedTransition = rootStateMachine.AddAnyStateTransition(pressedState);
            pressedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Pressed");
            pressedTransition.duration = 0.1f;

            var disabledTransition = rootStateMachine.AddAnyStateTransition(disabledState);
            disabledTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Disabled");
            disabledTransition.duration = 0.1f;

            var overonTransition = rootStateMachine.AddAnyStateTransition(overonState);
            overonTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "OverOn");
            overonTransition.duration = 0.1f;

            var overoffTransition = rootStateMachine.AddAnyStateTransition(overoffState);
            overoffTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "OverOff");
            overoffTransition.duration = 0.1f;

        }
        private void DetachController()
        {
            skToggle.DetachController();
            string path = SKAssetLibrary.UI_ANIM_DIR_PATH + $"/{skToggle.name}";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}