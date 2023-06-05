#if FINALIK
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("FinalIK Fullbody Config", "Headset + 2 hand controllers + 2 feet trackers + 1 pelvis tracker")]
    [AvatarReadySupportedTracker(2, "Left Foot Tracker", HumanBodyBones.LeftFoot, typeof(FootTracker))]
    [AvatarReadySupportedTracker(2, "Right Foot Tracker", HumanBodyBones.RightFoot, typeof(FootTracker))]
    [AvatarReadySupportedTracker(3, "Pelvis Tracker", HumanBodyBones.Hips, typeof(PelvisTracker), AvatarReadySupportedTrackerAttribute.Necessity.Optional)]
    public class FinalIKFullbodyConfig : FinalIKDefaultConfig
    {
        public override void ApplyConfiguration(AvatarReady avatarReady, Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors)
        {
            base.ApplyConfiguration(avatarReady, trackerDescriptors);

            if (!trackerDescriptors.ContainsKey(HumanBodyBones.LeftFoot) || !trackerDescriptors.ContainsKey(HumanBodyBones.RightFoot))
                throw new InvalidOperationException("Some required trackers are missing");

            VRIK finalIK = avatarReady.GetComponent<VRIK>();

            // Left Foot
            TargetHelper.TrackerDescriptor leftFoot = trackerDescriptors[HumanBodyBones.LeftFoot];
            finalIK.solver.leftLeg.target = leftFoot.Offset;
            finalIK.solver.leftLeg.positionWeight = 1;
            finalIK.solver.leftLeg.rotationWeight = 1;

            // Right Foot
            TargetHelper.TrackerDescriptor rightFoot = trackerDescriptors[HumanBodyBones.RightFoot];
            finalIK.solver.rightLeg.target = rightFoot.Offset;
            finalIK.solver.rightLeg.positionWeight = 1;
            finalIK.solver.rightLeg.rotationWeight = 1;

            // Pelvis
            if (trackerDescriptors.ContainsKey(HumanBodyBones.Hips))
            {
                TargetHelper.TrackerDescriptor pelvis = trackerDescriptors[HumanBodyBones.Hips];
                finalIK.solver.spine.pelvisTarget = pelvis.Offset;
                finalIK.solver.spine.pelvisPositionWeight = 1;
                finalIK.solver.spine.pelvisRotationWeight = 1;

                finalIK.solver.plantFeet = false;
            }

            // Disable Locomotion
            finalIK.solver.locomotion.weight = 0;

            // Add FinalIK Utils to move the root with the movement
            avatarReady.gameObject.GetOrAddComponent<VRIKRootController>();
        }

        public override void ResetConfiguration(AvatarReady avatarReady)
        {
            VRIK finalIK = avatarReady.GetComponent<VRIK>();

            if (finalIK != null)
            {
                // Left Foot
                finalIK.solver.leftLeg.target = null;
                finalIK.solver.leftLeg.positionWeight = 0;
                finalIK.solver.leftLeg.rotationWeight = 0;

                // Right Foot
                finalIK.solver.rightLeg.target = null;
                finalIK.solver.rightLeg.positionWeight = 0;
                finalIK.solver.rightLeg.rotationWeight = 0;

                // Pelvis
                finalIK.solver.spine.pelvisTarget = null;
                finalIK.solver.spine.pelvisPositionWeight = 0;
                finalIK.solver.spine.pelvisRotationWeight = 0;

                // Disable Locomotion
                finalIK.solver.locomotion.weight = 1;
            }

            // Add FinalIK Utils to move the root with the movement
            DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<VRIKRootController>());

            base.ResetConfiguration(avatarReady);
        }
    }
}
#endif