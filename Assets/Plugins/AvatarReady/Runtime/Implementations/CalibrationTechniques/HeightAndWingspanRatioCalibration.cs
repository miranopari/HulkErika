using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{

    [AvatarReadyName("Height and Wingspan Ratio Calibration", "Global scaling using the user's height and wingspan in T-Pose")]
    [AvatarReadySupportedSystem(typeof(IKAnimationSystem))]
    [AvatarReadyCalibrationData(headsetHeightKey, "Distance between the headset and the ground (in meter)")]
    [AvatarReadyCalibrationData(wristWingspanKey, "Distance between the two wrists in T-Pose (in meter)")]
    public class HeightAndWingspanRatioCalibration : CalibrationTechniqueBehaviour
    {
        private const string headsetHeightKey = "headsetHeight";
        private const string wristWingspanKey = "wristWingspan";

        public Transform Headset;
        public Transform LeftWrist;
        public Transform RightWrist;

        private Animator animator;

        private Vector3 tposeAvatarEyePosition;
        private float tposeAvatarHandsDistance;

        public override void InitBehaviour(AvatarReady avatarReady, Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors)
        {
            if (!trackerDescriptors.ContainsKey(HumanBodyBones.Head))
                throw new KeyNotFoundException("Missing Head tracker in the configuration");

            if (!trackerDescriptors.ContainsKey(HumanBodyBones.LeftHand))
                throw new KeyNotFoundException("Missing LeftHand tracker in the configuration");

            if (!trackerDescriptors.ContainsKey(HumanBodyBones.RightHand))
                throw new KeyNotFoundException("Missing RightHand tracker in the configuration");

            Headset = trackerDescriptors[HumanBodyBones.Head].Tracker;
            LeftWrist = trackerDescriptors[HumanBodyBones.LeftHand].Offset;
            RightWrist = trackerDescriptors[HumanBodyBones.RightHand].Offset;
        }

        public override void ResetBehaviour()
        {

        }

        private void Awake()
        {
            Assert.IsNotNull(Headset);
            Assert.IsNotNull(LeftWrist);
            Assert.IsNotNull(RightWrist);

            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator);

            // Take values on Awake to keep the avatar in T-Pose
            Vector3 tposeAvatarLeftEyePosition = animator.GetBoneTransform(HumanBodyBones.LeftEye).position;
            Vector3 tposeAvatarRightEyePosition = animator.GetBoneTransform(HumanBodyBones.RightEye).position;
            // TODO tposeAvatarEyePosition must be relative to avatar's ground too
            tposeAvatarEyePosition = (tposeAvatarLeftEyePosition + tposeAvatarRightEyePosition) / 2.0f;

            Vector3 tposeAvatarLeftHandPosition = animator.GetBoneTransform(HumanBodyBones.LeftHand).position;
            Vector3 tposeAvatarRightHandPosition = animator.GetBoneTransform(HumanBodyBones.RightHand).position;
            tposeAvatarHandsDistance = Vector3.Distance(tposeAvatarLeftHandPosition, tposeAvatarRightHandPosition);
        }

        public override void Calibrate(CalibrationProfile calibrationProfile = null)
        {
            // En T-Pose
            // ratio écart poignet G/D avec offset main G/D
            // aligné yeux avatar avec position y casque

            // casque / position yeux
            // controller - offset / poignet

            float headsetHeight;
            float handsOffsetsDistance;

            if (calibrationProfile == null)
            {
                // Assuming the user is in T-Pose, retrieve value and compute scale
                headsetHeight = GetData(headsetHeightKey);
                handsOffsetsDistance = GetData(wristWingspanKey);
            }
            else
            {
                if (!calibrationProfile.ContainsData(headsetHeightKey))
                    throw new ArgumentException(headsetHeightKey);
                if (!calibrationProfile.ContainsData(wristWingspanKey))
                    throw new ArgumentException(wristWingspanKey);

                headsetHeight = calibrationProfile.Get(headsetHeightKey);
                handsOffsetsDistance = calibrationProfile.Get(wristWingspanKey);
            }

            float yScale = headsetHeight / tposeAvatarEyePosition.y;
            float xScale = handsOffsetsDistance / tposeAvatarHandsDistance;

            transform.localScale = new Vector3(xScale, yScale, (xScale + yScale) / 2.0f);
        }

        public override float GetData(string dataKey)
        {
            switch (dataKey)
            {
                case headsetHeightKey:
                    return Headset.position.y;// TODO relative to camerarig y ?
                case wristWingspanKey:
                    return Vector3.Distance(LeftWrist.position, RightWrist.position);
                default:
                    throw new KeyNotFoundException(dataKey);
            }
        }
    }
}
