using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    /// <summary>
    /// Head offset for standard HMD.
    /// The offset position between the headset tracking position and actual head position (neck joint).
    /// Avatar dependant.
    /// </summary>
    public class HMDTracker : HeadTracker
    {
        public override void ApplyInitialOffset(HumanBodyBones humanBodyBone)
        {
            Assert.IsTrue(humanBodyBone == HumanBodyBones.Head);

            // standard value for a HMD with a normal sized male avatar
            transform.localPosition = new Vector3(0, -0.07f, -0.08f);
        }

        /// <summary>
        /// Called after calibration step
        /// </summary>
        /// <param name="animator"></param>
        public override void ApplyDynamicOffset(Animator animator)
        {
            Debug.Log(animator.GetBoneTransform(HumanBodyBones.Head));
            Vector3 avatarHeadPosition = animator.GetBoneTransform(HumanBodyBones.Head).position;
            Vector3 avatarLeftEyePosition = animator.GetBoneTransform(HumanBodyBones.LeftEye).position;
            Vector3 avatarRightEyePosition = animator.GetBoneTransform(HumanBodyBones.RightEye).position;

            Vector3 avatarMiddleEyePosition = (avatarLeftEyePosition + avatarRightEyePosition) / 2;

            // compute the vector from the avatar middle eye to the head (neck) position
            Vector3 offsetValue = avatarHeadPosition - avatarMiddleEyePosition;

            // convert this offset in the user orientation
            Vector3 adjustedOffsetValue = transform.InverseTransformVector(offsetValue);

            adjustedOffsetValue.x = 0;

            // apply the offset to keep the camera on the middleEyePosition of the avatar
            transform.localPosition = adjustedOffsetValue;
        }
    }
}
