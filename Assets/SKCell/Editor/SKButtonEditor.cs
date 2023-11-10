using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;

#endif

namespace SKCell
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SKButton))]
    [CanEditMultipleObjects]
    public class SKButtonEditor : Editor
    {
        static SKButton skButton;
        public override void OnInspectorGUI()
        {
            skButton = (SKButton)target;
            if ((!skButton.initialized) && UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null)
                if (GUILayout.Button("<Generate structure>", GUILayout.Height(30)))
                {
                    skButton.GenerateStructure();
                }
            if (skButton.initialized || UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                if (skButton.transitionMode == SKButton.SKButtonTransitionMode.Animation)
                {
                    GUILayout.Label("The animation template is automatically generated.");
                    if (!skButton.hasAnimator&&GUILayout.Button("<Generate animator>", GUILayout.Height(30)))
                    {
                        CreateController();
                    }
                }
                else if (skButton.hasAnimator)
                {
                    if (GUILayout.Button("<Delete animation>", GUILayout.Height(30)))
                    {
                        DetachController();
                    }
                }
                base.OnInspectorGUI();
                if (!Application.isPlaying)
                    skButton.DrawEditorPreview();
            }
        }
       
        private static void CreateController()
        {
            string path= SKAssetLibrary.UI_ANIM_DIR_PATH + $"/{skButton.name}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            // Creates the controller
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path + $"/{skButton.name}.controller");
            AnimationClip normalClip = new AnimationClip();
            AssetDatabase.CreateAsset(normalClip, path + $"/Normal.anim");
            AnimationClip overClip = new AnimationClip();
            AssetDatabase.CreateAsset(overClip, path + $"/Over.anim");
            AnimationClip pressedClip = new AnimationClip();
            AssetDatabase.CreateAsset(pressedClip, path + $"/Pressed.anim");
            AnimationClip disabledClip = new AnimationClip();
            AssetDatabase.CreateAsset(disabledClip, path + $"/Disabled.anim");
            AnimationClip hold1Clip = new AnimationClip();
            AssetDatabase.CreateAsset(hold1Clip, path + $"/Hold1.anim");
            AnimationClip hold2Clip = new AnimationClip();
            AssetDatabase.CreateAsset(hold2Clip, path + $"/Hold2.anim");
            skButton.AttachController(controller);



            // Add parameters
            controller.AddParameter("Normal", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Pressed", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Over", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Disabled", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("HoldForSeconds1", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("HoldForSeconds2", AnimatorControllerParameterType.Trigger);

            // Add StateMachines
            var rootStateMachine = controller.layers[0].stateMachine;
            
            // Add States
            var normalState = rootStateMachine.AddState("Normal");
            normalState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Normal.anim");
            var overState = rootStateMachine.AddState("Over");
            overState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Over.anim");
            var pressedState = rootStateMachine.AddState("Pressed");
            pressedState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Pressed.anim");
            var disabledState = rootStateMachine.AddState("Disabled");
            disabledState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Disabled.anim");
            var hold1State = rootStateMachine.AddState("HoldForSeconds1");
            hold1State.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Hold1.anim");
            var hold2State = rootStateMachine.AddState("HoldForSeconds2");
            hold2State.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + $"/Hold2.anim");

            rootStateMachine.defaultState = normalState;

            // Add Transitions
            var normalTransition = rootStateMachine.AddAnyStateTransition(normalState);
            normalTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Normal");
            normalTransition.duration = 0.1f;

            var overTransition = rootStateMachine.AddAnyStateTransition(overState);
            overTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Over");
            overTransition.duration = 0.1f;

            var pressedTransition = rootStateMachine.AddAnyStateTransition(pressedState);
            pressedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Pressed");
            pressedTransition.duration = 0.1f;

            var disabledTransition = rootStateMachine.AddAnyStateTransition(disabledState);
            disabledTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Disabled");
            disabledTransition.duration = 0.1f;

            var hold1Transition = rootStateMachine.AddAnyStateTransition(hold1State);
            hold1Transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "HoldForSeconds1");
            hold1Transition.duration = 0.1f;

            var hold2Transition = rootStateMachine.AddAnyStateTransition(hold2State);
            hold2Transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "HoldForSeconds2");
            hold2Transition.duration = 0.1f;

        }
        private void DetachController()
        {
            skButton.DetachController();
            string path = SKAssetLibrary.UI_ANIM_DIR_PATH + $"/{skButton.name}";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
#endif
}
