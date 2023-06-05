#if FINALIK
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("FinalIK Mimic Config", "Headset + 2 controllers, Locomotion procedural animation used for the feet")]
    [AvatarReadySupportedSystem(typeof(FinalIKAnimationSystem))]
    [AvatarReadySupportedTracker(0, "Headset", HumanBodyBones.Head, typeof(HeadTracker))]
    [AvatarReadySupportedTracker(1, "Left Hand Tracker", HumanBodyBones.LeftHand, typeof(HandTracker))]
    [AvatarReadySupportedTracker(1, "Right Hand Tracker", HumanBodyBones.RightHand, typeof(HandTracker))]
    [AvatarReadySupportedTracker(2, "Left Foot Tracker", HumanBodyBones.LeftFoot, typeof(FootTracker), AvatarReadySupportedTrackerAttribute.Necessity.Optional)]
    [AvatarReadySupportedTracker(2, "Right Foot Tracker", HumanBodyBones.RightFoot, typeof(FootTracker), AvatarReadySupportedTrackerAttribute.Necessity.Optional)]
    [AvatarReadySupportedTracker(3, "Pelvis Tracker", HumanBodyBones.Hips, typeof(PelvisTracker), AvatarReadySupportedTrackerAttribute.Necessity.Optional)]
    public class FinalIKMimicConfig : AnimationConfig
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
            head.Target.GetComponent<CopyPositionAndRotation>().relative = CopyPositionAndRotation.Relative.Local;

            // Left Hand
            TargetHelper.TrackerDescriptor leftHand = trackerDescriptors[HumanBodyBones.LeftHand];
            finalIK.solver.leftArm.target = leftHand.Offset;
            finalIK.solver.leftArm.positionWeight = 1;
            finalIK.solver.leftArm.rotationWeight = 1;
            leftHand.Target.GetComponent<CopyPositionAndRotation>().relative = CopyPositionAndRotation.Relative.Local;

            // Right Hand
            TargetHelper.TrackerDescriptor rightHand = trackerDescriptors[HumanBodyBones.RightHand];
            finalIK.solver.rightArm.target = rightHand.Offset;
            finalIK.solver.rightArm.positionWeight = 1;
            finalIK.solver.rightArm.rotationWeight = 1;
            rightHand.Target.GetComponent<CopyPositionAndRotation>().relative = CopyPositionAndRotation.Relative.Local;

            // Left & Right Foot
            if (trackerDescriptors.ContainsKey(HumanBodyBones.LeftFoot) && trackerDescriptors.ContainsKey(HumanBodyBones.RightFoot))
            {
                TargetHelper.TrackerDescriptor leftFoot = trackerDescriptors[HumanBodyBones.LeftFoot];
                finalIK.solver.leftLeg.target = leftFoot.Offset;
                finalIK.solver.leftLeg.positionWeight = 1;
                finalIK.solver.leftLeg.rotationWeight = 1;
                leftFoot.Target.GetComponent<CopyPositionAndRotation>().relative = CopyPositionAndRotation.Relative.Local;

                TargetHelper.TrackerDescriptor rightFoot = trackerDescriptors[HumanBodyBones.RightFoot];
                finalIK.solver.rightLeg.target = rightFoot.Offset;
                finalIK.solver.rightLeg.positionWeight = 1;
                finalIK.solver.rightLeg.rotationWeight = 1;
                rightFoot.Target.GetComponent<CopyPositionAndRotation>().relative = CopyPositionAndRotation.Relative.Local;

                // Disable Locomotion
                finalIK.solver.locomotion.weight = 0;

                // Add FinalIK Utils to move the root with the movement
                avatarReady.gameObject.GetOrAddComponent<VRIKRootController>();
            }
            else
            {
                // Locomotion settings
                finalIK.solver.locomotion.footDistance = 0.15f;
                finalIK.solver.locomotion.stepThreshold = 0.2f;
            }

            // Pelvis
            if (trackerDescriptors.ContainsKey(HumanBodyBones.Hips))
            {
                TargetHelper.TrackerDescriptor pelvis = trackerDescriptors[HumanBodyBones.Hips];
                finalIK.solver.spine.pelvisTarget = pelvis.Offset;
                finalIK.solver.spine.pelvisPositionWeight = 1;
                finalIK.solver.spine.pelvisRotationWeight = 1;
                pelvis.Target.GetComponent<CopyPositionAndRotation>().relative = CopyPositionAndRotation.Relative.Local;
            }
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

                // Left & Right Foot

                finalIK.solver.leftLeg.target = null;
                finalIK.solver.leftLeg.positionWeight = 0;
                finalIK.solver.leftLeg.rotationWeight = 0;

                finalIK.solver.rightLeg.target = null;
                finalIK.solver.rightLeg.positionWeight = 0;
                finalIK.solver.rightLeg.rotationWeight = 0;

                // Disable Locomotion
                finalIK.solver.locomotion.weight = 1;

                // Locomotion settings
                finalIK.solver.locomotion.footDistance = 0.3f;
                finalIK.solver.locomotion.stepThreshold = 0.4f;

                // Add FinalIK Utils to move the root with the movement
                DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<VRIKRootController>());

                // Pelvis
                finalIK.solver.spine.pelvisTarget = null;
                finalIK.solver.spine.pelvisPositionWeight = 0;
                finalIK.solver.spine.pelvisRotationWeight = 0;
            }
        }
    }
}
#endif