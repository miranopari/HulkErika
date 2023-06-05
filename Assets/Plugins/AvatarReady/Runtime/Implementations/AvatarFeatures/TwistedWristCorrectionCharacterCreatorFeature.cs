using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Twisted wrist correction")]
    [AvatarReadySupportedProvider(typeof(CharacterCreatorAvatarProvider), true)]
    public class TwistedWristCorrectionCharacterCreatorFeature : AvatarFeature
    {
        private Animator animator;

        private Transform LeftElbow;
        private Transform LeftTwist;
        private Transform LeftWrist;

        private Transform RightElbow;
        private Transform RightTwist;
        private Transform RightWrist;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator);
        }

        private void Start()
        {
            LeftElbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            LeftTwist = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).FindChildByName("CC_Base_L_ForearmTwist02");
            LeftWrist = animator.GetBoneTransform(HumanBodyBones.LeftHand);

            RightElbow = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            RightTwist = animator.GetBoneTransform(HumanBodyBones.RightLowerArm).FindChildByName("CC_Base_R_ForearmTwist02");
            RightWrist = animator.GetBoneTransform(HumanBodyBones.RightHand);

            Assert.IsNotNull(LeftElbow);
            Assert.IsNotNull(LeftTwist);
            Assert.IsNotNull(LeftWrist);

            Assert.IsNotNull(RightElbow);
            Assert.IsNotNull(RightTwist);
            Assert.IsNotNull(RightWrist);
        }

        private void Update()
        {
            float left = LeftWrist.localEulerAngles.y;
            if (left > 180)
                left -= 360;

            float right = RightWrist.localEulerAngles.y;
            if (right > 180)
                right -= 360;

            LeftTwist.localEulerAngles = new Vector3(0, left / 2f, 0);
            RightTwist.localEulerAngles = new Vector3(0, right / 2f, 0);
        }
    }
}