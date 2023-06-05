using UnityEngine;

namespace Inria.Avatar.AvatarReady
{

    [AvatarReadyName("Manual finger animation helper")]
    [AvatarReadySupportedProvider(typeof(AvatarProvider))]
    public class HumanoidManualFingerAnimationFeature : AvatarFeature
    {
        private Animator animator;

        /*
         * Muscle ID / Muscle name / Default Min / Default Max
         * 
         * 55   / Left Thumb 1 Stretched      / min: -20  / max: 20
         * 56   / Left Thumb Spread           / min: -25  / max: 25
         * 57   / Left Thumb 2 Stretched      / min: -40  / max: 35
         * 58   / Left Thumb 3 Stretched      / min: -40  / max: 35
         * 59   / Left Index 1 Stretched      / min: -50  / max: 50
         * 60   / Left Index Spread           / min: -20  / max: 20
         * 61   / Left Index 2 Stretched      / min: -45  / max: 45
         * 62   / Left Index 3 Stretched      / min: -45  / max: 45
         * 63   / Left Middle 1 Stretched     / min: -50  / max: 50
         * 64   / Left Middle Spread          / min: -7,5 / max: 7,5
         * 65   / Left Middle 2 Stretched     / min: -45  / max: 45
         * 66   / Left Middle 3 Stretched     / min: -45  / max: 45
         * 67   / Left Ring 1 Stretched       / min: -50  / max: 50
         * 68   / Left Ring Spread            / min: -7,5 / max: 7,5
         * 69   / Left Ring 2 Stretched       / min: -45  / max: 45
         * 70   / Left Ring 3 Stretched       / min: -45  / max: 45
         * 71   / Left Little 1 Stretched     / min: -50  / max: 50
         * 72   / Left Little Spread          / min: -20  / max: 20
         * 73   / Left Little 2 Stretched     / min: -45  / max: 45
         * 74   / Left Little 3 Stretched     / min: -45  / max: 45
         * 
         * 75   / Right Thumb 1 Stretched     / min: -20  / max: 20
         * 76   / Right Thumb Spread          / min: -25  / max: 25
         * 77   / Right Thumb 2 Stretched     / min: -40  / max: 35
         * 78   / Right Thumb 3 Stretched     / min: -40  / max: 35
         * 79   / Right Index 1 Stretched     / min: -50  / max: 50
         * 80   / Right Index Spread          / min: -20  / max: 20
         * 81   / Right Index 2 Stretched     / min: -45  / max: 45
         * 82   / Right Index 3 Stretched     / min: -45  / max: 45
         * 83   / Right Middle 1 Stretched    / min: -50  / max: 50
         * 84   / Right Middle Spread         / min: -7,5 / max: 7,5
         * 85   / Right Middle 2 Stretched    / min: -45  / max: 45
         * 86   / Right Middle 3 Stretched    / min: -45  / max: 45
         * 87   / Right Ring 1 Stretched      / min: -50  / max: 50
         * 88   / Right Ring Spread           / min: -7,5 / max: 7,5
         * 89   / Right Ring 2 Stretched      / min: -45  / max: 45
         * 90   / Right Ring 3 Stretched      / min: -45  / max: 45
         * 91   / Right Little 1 Stretched    / min: -50  / max: 50
         * 92   / Right Little Spread         / min: -20  / max: 20
         * 93   / Right Little 2 Stretched    / min: -45  / max: 45
         * 94   / Right Little 3 Stretched    / min: -45  / max: 45
         */

        #region Muscles
        [Range(-1, 1)]
        public float LeftThumb = 0f;

        [Range(-1, 1)]
        public float LeftIndex = 0.6f;

        [Range(-1, 1)]
        public float LeftMiddle = 0.6f;

        [Range(-1, 1)]
        public float LeftRing = 0.6f;

        [Range(-1, 1)]
        public float LeftLittle = 0.6f;


        [Range(-1, 1)]
        public float RightThumb = 0f;

        [Range(-1, 1)]
        public float RightIndex = 0.6f;

        [Range(-1, 1)]
        public float RightMiddle = 0.6f;

        [Range(-1, 1)]
        public float RightRing = 0.6f;

        [Range(-1, 1)]
        public float RightLittle = 0.6f;


        [Range(-1, 1)]
        public float LeftThumbSpread = 0f;

        [Range(-1, 1)]
        public float LeftIndexSpread = -0.6f;

        [Range(-1, 1)]
        public float LeftMiddleSpread = -0.6f;

        [Range(-1, 1)]
        public float LeftRingSpread = -0.6f;

        [Range(-1, 1)]
        public float LeftLittleSpread = -0.6f;


        [Range(-1, 1)]
        public float RightThumbSpread = 0f;

        [Range(-1, 1)]
        public float RightIndexSpread = -0.6f;

        [Range(-1, 1)]
        public float RightMiddleSpread = -0.6f;

        [Range(-1, 1)]
        public float RightRingSpread = -0.6f;

        [Range(-1, 1)]
        public float RightLittleSpread = -0.6f;
        #endregion

        private HumanPoseHandler humanPoseHandler;
        private HumanPose humanPose;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);

            humanPose = new HumanPose();
        }

        private void Update()
        {
            humanPoseHandler.GetHumanPose(ref humanPose);

            humanPose.muscles[55] = LeftThumb;
            humanPose.muscles[56] = LeftThumbSpread;
            humanPose.muscles[57] = LeftThumb;
            humanPose.muscles[58] = LeftThumb;

            humanPose.muscles[59] = LeftIndex;
            humanPose.muscles[60] = LeftIndexSpread;
            humanPose.muscles[61] = LeftIndex;
            humanPose.muscles[62] = LeftIndex;

            humanPose.muscles[63] = LeftMiddle;
            humanPose.muscles[64] = LeftMiddleSpread;
            humanPose.muscles[65] = LeftMiddle;
            humanPose.muscles[66] = LeftMiddle;

            humanPose.muscles[67] = LeftRing;
            humanPose.muscles[68] = LeftRingSpread;
            humanPose.muscles[69] = LeftRing;
            humanPose.muscles[70] = LeftRing;

            humanPose.muscles[71] = LeftLittle;
            humanPose.muscles[72] = LeftLittleSpread;
            humanPose.muscles[73] = LeftLittle;
            humanPose.muscles[74] = LeftLittle;

            humanPose.muscles[75] = RightThumb;
            humanPose.muscles[76] = RightThumbSpread;
            humanPose.muscles[77] = RightThumb;
            humanPose.muscles[78] = RightThumb;

            humanPose.muscles[79] = RightIndex;
            humanPose.muscles[80] = RightIndexSpread;
            humanPose.muscles[81] = RightIndex;
            humanPose.muscles[82] = RightIndex;

            humanPose.muscles[83] = RightMiddle;
            humanPose.muscles[84] = RightMiddleSpread;
            humanPose.muscles[85] = RightMiddle;
            humanPose.muscles[86] = RightMiddle;

            humanPose.muscles[87] = RightRing;
            humanPose.muscles[88] = RightRingSpread;
            humanPose.muscles[89] = RightRing;
            humanPose.muscles[90] = RightRing;

            humanPose.muscles[91] = RightLittle;
            humanPose.muscles[92] = RightLittleSpread;
            humanPose.muscles[93] = RightLittle;
            humanPose.muscles[94] = RightLittle;

            humanPoseHandler.SetHumanPose(ref humanPose);
        }
    }
}
