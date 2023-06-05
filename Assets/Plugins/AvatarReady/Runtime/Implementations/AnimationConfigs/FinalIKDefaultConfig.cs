using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("FinalIK Default Config", "Headset + 2 controllers, embedded Locomotion animation used for the feet")]
    [AvatarReadySupportedSystem(typeof(FinalIKAnimationSystem))]
    [AvatarReadySupportedTracker(0, "Headset", HumanBodyBones.Head, typeof(HeadTracker))]
    [AvatarReadySupportedTracker(1, "Left Hand Tracker", HumanBodyBones.LeftHand, typeof(HandTracker))]
    [AvatarReadySupportedTracker(1, "Right Hand Tracker", HumanBodyBones.RightHand, typeof(HandTracker))]
    [AvatarReadyConstOption(0, "Locomotion mode", "locomotionMode", typeof(IKSolverVR.Locomotion.Mode), IKSolverVR.Locomotion.Mode.Procedural)]
    public class FinalIKDefaultConfig : AnimationConfig
    {
        public override void ApplyConfiguration(AvatarReady avatarReady, Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors)
        {
            if (!trackerDescriptors.ContainsKey(HumanBodyBones.Head) || !trackerDescriptors.ContainsKey(HumanBodyBones.LeftHand) || !trackerDescriptors.ContainsKey(HumanBodyBones.RightHand))
                throw new InvalidOperationException("Some required trackers are missing");

            VRIK finalIK = avatarReady.GetComponent<VRIK>();

            // Head
            TargetHelper.TrackerDescriptor head = trackerDescriptors[HumanBodyBones.Head];
            finalIK.solver.spine.headTarget = head.Offset;
            finalIK.solver.spine.positionWeight = 1;
            finalIK.solver.spine.rotationWeight = 1;

            // Left Hand
            TargetHelper.TrackerDescriptor leftHand = trackerDescriptors[HumanBodyBones.LeftHand];
            finalIK.solver.leftArm.target = leftHand.Offset;
            finalIK.solver.leftArm.positionWeight = 1;
            finalIK.solver.leftArm.rotationWeight = 1;

            // Right Hand
            TargetHelper.TrackerDescriptor rightHand = trackerDescriptors[HumanBodyBones.RightHand];
            finalIK.solver.rightArm.target = rightHand.Offset;
            finalIK.solver.rightArm.positionWeight = 1;
            finalIK.solver.rightArm.rotationWeight = 1;

            // Locomotion mode
            IKSolverVR.Locomotion.Mode locomotionMode = (IKSolverVR.Locomotion.Mode)Enum.Parse(typeof(IKSolverVR.Locomotion.Mode), avatarReady.ConfigStringOptions["locomotionMode"]);
            finalIK.solver.locomotion.mode = locomotionMode;

            // Procedural locomotion settings
            finalIK.solver.locomotion.footDistance = 0.15f;
            finalIK.solver.locomotion.stepThreshold = 0.2f;

            // Animated locomotion settings
            if (locomotionMode == IKSolverVR.Locomotion.Mode.Animated)
            {
#if UNITY_EDITOR
                // Try set Animator
                string guidController = UnityEditor.AssetDatabase.FindAssets("VRIK Animated Locomotion", null)[0];
                string pathController = UnityEditor.AssetDatabase.GUIDToAssetPath(guidController);
                avatarReady.GetComponent<Animator>().runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(pathController);
#endif
            }

            avatarReady.gameObject.GetOrAddComponent<FixFinalIKLocomotionDesync>();
        }

        public override void ResetConfiguration(AvatarReady avatarReady)
        {
            VRIK finalIK = avatarReady.GetComponent<VRIK>();

            if (finalIK != null)
            {
                // Head
                finalIK.solver.spine.headTarget = null;
                finalIK.solver.spine.positionWeight = 1;
                finalIK.solver.spine.rotationWeight = 1;

                // Left Hand
                finalIK.solver.leftArm.target = null;
                finalIK.solver.leftArm.positionWeight = 1;
                finalIK.solver.leftArm.rotationWeight = 1;

                // Right Hand
                finalIK.solver.rightArm.target = null;
                finalIK.solver.rightArm.positionWeight = 1;
                finalIK.solver.rightArm.rotationWeight = 1;

                // Locomotion settings
                finalIK.solver.locomotion.mode = default(IKSolverVR.Locomotion.Mode);
                finalIK.solver.locomotion.footDistance = 0.3f;
                finalIK.solver.locomotion.stepThreshold = 0.4f;
                avatarReady.GetComponent<Animator>().runtimeAnimatorController = null;
            }

            DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<FixFinalIKLocomotionDesync>());
        }
    }
}