#if FINALIK
using SA;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    // https://github.com/Stereoarts/SAFullBodyIK
    [AvatarReadyName("SAFBIK Config", "Headset + 2 controllers + optional feet + optional pelvis. No leg animation")]
    [AvatarReadySupportedSystem(typeof(SAFBIKAnimationSystem))]
    [AvatarReadySupportedTracker(0, "Headset", HumanBodyBones.Head, typeof(HeadTracker))]
    [AvatarReadySupportedTracker(1, "Left Hand Tracker", HumanBodyBones.LeftHand, typeof(HandTracker))]
    [AvatarReadySupportedTracker(1, "Right Hand Tracker", HumanBodyBones.RightHand, typeof(HandTracker))]
    [AvatarReadySupportedTracker(2, "Left Foot Tracker", HumanBodyBones.LeftFoot, typeof(FootTracker), AvatarReadySupportedTrackerAttribute.Necessity.Optional)]
    [AvatarReadySupportedTracker(2, "Right Foot Tracker", HumanBodyBones.RightFoot, typeof(FootTracker), AvatarReadySupportedTrackerAttribute.Necessity.Optional)]
    [AvatarReadySupportedTracker(3, "Pelvis Tracker", HumanBodyBones.Hips, typeof(PelvisTracker), AvatarReadySupportedTrackerAttribute.Necessity.Optional)]
    public class SAFBIKFullbodyConfig : AnimationConfig
    {
        public override void ApplyConfiguration(AvatarReady avatarReady, Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors)
        {
            if (!trackerDescriptors.ContainsKey(HumanBodyBones.Head) || !trackerDescriptors.ContainsKey(HumanBodyBones.LeftHand) || !trackerDescriptors.ContainsKey(HumanBodyBones.RightHand))
                throw new InvalidOperationException("Some required trackers are missing");

            // SAFBIK
            FullBodyIKBehaviour safbik = avatarReady.GetComponent<FullBodyIKBehaviour>();

            // Head
            TargetHelper.TrackerDescriptor head = trackerDescriptors[HumanBodyBones.Head];

            safbik.fullBodyIK.headEffectors.head.positionEnabled = true;
            safbik.fullBodyIK.headEffectors.head.positionWeight = 1;
            safbik.fullBodyIK.headEffectors.head.pull = 1;
            safbik.fullBodyIK.headEffectors.head.rotationEnabled = true;
            safbik.fullBodyIK.headEffectors.head.rotationWeight = 1;

            CopyPositionAndRotation headEffector = safbik.fullBodyIK.headEffectors.head.transform.gameObject.GetOrAddComponent<CopyPositionAndRotation>();
            headEffector.transformToCopy = head.Offset;
            headEffector.relative = CopyPositionAndRotation.Relative.World;

            // Left Hand
            TargetHelper.TrackerDescriptor leftHand = trackerDescriptors[HumanBodyBones.LeftHand];

            safbik.fullBodyIK.leftArmEffectors.wrist.positionEnabled = true;
            safbik.fullBodyIK.leftArmEffectors.wrist.positionWeight = 1;
            safbik.fullBodyIK.leftArmEffectors.wrist.pull = 1;
            safbik.fullBodyIK.leftArmEffectors.wrist.rotationEnabled = true;
            safbik.fullBodyIK.leftArmEffectors.wrist.rotationWeight = 1;

            CopyPositionAndRotation leftHandEffector = safbik.fullBodyIK.leftArmEffectors.wrist.transform.gameObject.GetOrAddComponent<CopyPositionAndRotation>();
            leftHandEffector.transformToCopy = leftHand.Offset;
            leftHandEffector.relative = CopyPositionAndRotation.Relative.World;

            // Right Hand
            TargetHelper.TrackerDescriptor rightHand = trackerDescriptors[HumanBodyBones.RightHand];

            safbik.fullBodyIK.rightArmEffectors.wrist.positionEnabled = true;
            safbik.fullBodyIK.rightArmEffectors.wrist.positionWeight = 1;
            safbik.fullBodyIK.rightArmEffectors.wrist.pull = 1;
            safbik.fullBodyIK.rightArmEffectors.wrist.rotationEnabled = true;
            safbik.fullBodyIK.rightArmEffectors.wrist.rotationWeight = 1;

            CopyPositionAndRotation rightHandEffector = safbik.fullBodyIK.rightArmEffectors.wrist.transform.gameObject.GetOrAddComponent<CopyPositionAndRotation>();
            rightHandEffector.transformToCopy = rightHand.Offset;
            rightHandEffector.relative = CopyPositionAndRotation.Relative.World;

            // Left Foot
            if (trackerDescriptors.ContainsKey(HumanBodyBones.LeftFoot))
            {
                TargetHelper.TrackerDescriptor leftFoot = trackerDescriptors[HumanBodyBones.LeftFoot];

                safbik.fullBodyIK.leftLegEffectors.foot.positionEnabled = true;
                safbik.fullBodyIK.leftLegEffectors.foot.positionWeight = 1;
                safbik.fullBodyIK.leftLegEffectors.foot.pull = 1;
                safbik.fullBodyIK.leftLegEffectors.foot.rotationEnabled = false;
                safbik.fullBodyIK.leftLegEffectors.foot.rotationWeight = 0;

                CopyPositionAndRotation leftFootEffector = safbik.fullBodyIK.leftLegEffectors.foot.transform.gameObject.GetOrAddComponent<CopyPositionAndRotation>();
                leftFootEffector.transformToCopy = leftFoot.Offset;
                leftFootEffector.relative = CopyPositionAndRotation.Relative.World;
            }

            // Right Foot
            if (trackerDescriptors.ContainsKey(HumanBodyBones.RightFoot))
            {
                TargetHelper.TrackerDescriptor rightFoot = trackerDescriptors[HumanBodyBones.RightHand];

                safbik.fullBodyIK.rightLegEffectors.foot.positionEnabled = true;
                safbik.fullBodyIK.rightLegEffectors.foot.positionWeight = 1;
                safbik.fullBodyIK.rightLegEffectors.foot.pull = 1;
                safbik.fullBodyIK.rightLegEffectors.foot.rotationEnabled = false;
                safbik.fullBodyIK.rightLegEffectors.foot.rotationWeight = 0;

                CopyPositionAndRotation rightFootEffector = safbik.fullBodyIK.rightLegEffectors.foot.transform.gameObject.GetOrAddComponent<CopyPositionAndRotation>();
                rightFootEffector.transformToCopy = rightFoot.Offset;
                rightFootEffector.relative = CopyPositionAndRotation.Relative.World;
            }

            // Pelvis
            if (trackerDescriptors.ContainsKey(HumanBodyBones.Hips))
            {
                TargetHelper.TrackerDescriptor pelvis = trackerDescriptors[HumanBodyBones.Hips];

                safbik.fullBodyIK.bodyEffectors.hips.positionEnabled = true;
                safbik.fullBodyIK.bodyEffectors.hips.positionWeight = 1;
                safbik.fullBodyIK.bodyEffectors.hips.pull = 1;

                CopyPositionAndRotation pelvisEffector = safbik.fullBodyIK.bodyEffectors.hips.transform.gameObject.GetOrAddComponent<CopyPositionAndRotation>();
                pelvisEffector.transformToCopy = pelvis.Offset;
                pelvisEffector.relative = CopyPositionAndRotation.Relative.World;
            }

            // Ensure that the root gameobject follow the body
            // TODO check if still useful
            ForceBodyPosition forceBodyPosition = avatarReady.gameObject.GetOrAddComponent<ForceBodyPosition>();
            forceBodyPosition.HeadTarget = head.Offset.gameObject;

            ForceBodyRotation forceBodyRotation = avatarReady.gameObject.GetOrAddComponent<ForceBodyRotation>();
            forceBodyRotation.HeadTarget = head.Offset.gameObject;

            // Fix default settings
            safbik.fullBodyIK.settings.headIK.neckLimitPitchUp = 45;
            safbik.fullBodyIK.settings.headIK.neckLimitPitchDown = 45;
            safbik.fullBodyIK.settings.headIK.headLimitPitchUp = 45;
            safbik.fullBodyIK.settings.headIK.headLimitPitchDown = 45;
        }

        public override void ResetConfiguration(AvatarReady avatarReady)
        {
            // SAFBIK
            FullBodyIKBehaviour safbik = avatarReady.GetComponent<FullBodyIKBehaviour>();

            if (safbik != null)
            {
                // Head
                safbik.fullBodyIK.headEffectors.head.positionEnabled = false;
                safbik.fullBodyIK.headEffectors.head.positionWeight = 1;
                safbik.fullBodyIK.headEffectors.head.pull = 0;
                safbik.fullBodyIK.headEffectors.head.rotationEnabled = false;
                safbik.fullBodyIK.headEffectors.head.rotationWeight = 1;

                DestroyUtils.Destroy(safbik.fullBodyIK.headEffectors.head.transform.gameObject.GetComponent<CopyPositionAndRotation>());

                // Left Hand
                safbik.fullBodyIK.leftArmEffectors.wrist.positionEnabled = false;
                safbik.fullBodyIK.leftArmEffectors.wrist.positionWeight = 1;
                safbik.fullBodyIK.leftArmEffectors.wrist.pull = 0;
                safbik.fullBodyIK.leftArmEffectors.wrist.rotationEnabled = false;
                safbik.fullBodyIK.leftArmEffectors.wrist.rotationWeight = 1;

                DestroyUtils.Destroy(safbik.fullBodyIK.leftArmEffectors.wrist.transform.gameObject.GetComponent<CopyPositionAndRotation>());


                // Right Hand
                safbik.fullBodyIK.rightArmEffectors.wrist.positionEnabled = false;
                safbik.fullBodyIK.rightArmEffectors.wrist.positionWeight = 1;
                safbik.fullBodyIK.rightArmEffectors.wrist.pull = 0;
                safbik.fullBodyIK.rightArmEffectors.wrist.rotationEnabled = false;
                safbik.fullBodyIK.rightArmEffectors.wrist.rotationWeight = 1;

                DestroyUtils.Destroy(safbik.fullBodyIK.rightArmEffectors.wrist.transform.gameObject.GetComponent<CopyPositionAndRotation>());

                // Left Foot
                safbik.fullBodyIK.leftLegEffectors.foot.positionEnabled = false;
                safbik.fullBodyIK.leftLegEffectors.foot.positionWeight = 1;
                safbik.fullBodyIK.leftLegEffectors.foot.pull = 0;
                safbik.fullBodyIK.leftLegEffectors.foot.rotationEnabled = false;
                safbik.fullBodyIK.leftLegEffectors.foot.rotationWeight = 1;

                DestroyUtils.Destroy(safbik.fullBodyIK.leftLegEffectors.foot.transform.gameObject.GetComponent<CopyPositionAndRotation>());

                // Right Foot

                safbik.fullBodyIK.rightLegEffectors.foot.positionEnabled = false;
                safbik.fullBodyIK.rightLegEffectors.foot.positionWeight = 1;
                safbik.fullBodyIK.rightLegEffectors.foot.pull = 0;
                safbik.fullBodyIK.rightLegEffectors.foot.rotationEnabled = false;
                safbik.fullBodyIK.rightLegEffectors.foot.rotationWeight = 1;

                DestroyUtils.Destroy(safbik.fullBodyIK.rightLegEffectors.foot.transform.gameObject.GetComponent<CopyPositionAndRotation>());

                // Pelvis
                safbik.fullBodyIK.bodyEffectors.hips.positionEnabled = false;
                safbik.fullBodyIK.bodyEffectors.hips.positionWeight = 1;
                safbik.fullBodyIK.bodyEffectors.hips.pull = 0;

                DestroyUtils.Destroy(safbik.fullBodyIK.bodyEffectors.hips.transform.gameObject.GetOrAddComponent<CopyPositionAndRotation>());

                DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<ForceBodyPosition>());
                DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<ForceBodyRotation>());

                // Restore default settings
                safbik.fullBodyIK.settings.headIK.neckLimitPitchUp = 15;
                safbik.fullBodyIK.settings.headIK.neckLimitPitchDown = 30;
                safbik.fullBodyIK.settings.headIK.headLimitPitchUp = 15;
                safbik.fullBodyIK.settings.headIK.headLimitPitchDown = 15;
            }
        }
    }
}
#endif