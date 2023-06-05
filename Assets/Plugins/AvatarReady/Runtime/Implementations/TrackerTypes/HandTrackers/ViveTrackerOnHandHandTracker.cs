using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    public class ViveTrackerOnHandHandTracker : HandTracker
    {
        private HumanBodyBones humanBodyBone;

        public override void ApplyInitialOffset(HumanBodyBones humanBodyBone)
        {
            Assert.IsTrue(humanBodyBone == HumanBodyBones.LeftHand || humanBodyBone == HumanBodyBones.RightHand);

            if (humanBodyBone == HumanBodyBones.LeftHand)
                transform.localPosition = new Vector3(-0.0724f, -0.0035f, -0.0274f);
            else if (humanBodyBone == HumanBodyBones.RightHand)
                transform.localPosition = new Vector3(-0.006f, 0.0732f, -0.0322f);

            if (humanBodyBone == HumanBodyBones.LeftHand)
                transform.localRotation = Quaternion.Euler(280.741455f, 59.6710587f, 220.07634f);
            else if (humanBodyBone == HumanBodyBones.RightHand)
                transform.localRotation = Quaternion.Euler(20.9760723f, 262.278015f, 177.221024f);

            this.humanBodyBone = humanBodyBone;
        }

        public override void ApplyDynamicOffset(Animator animator)
        {
            Assert.IsTrue(humanBodyBone == HumanBodyBones.LeftHand || humanBodyBone == HumanBodyBones.RightHand);

            // trackers rotates around Z axis only
            // trackers world pos/rot = offset's parent's world pos/rot

            // TODO get head
            Transform HeadTracker = transform.parent.parent;

            // TODO align hand (axis wrist/index), with arm (axis elbow/wrist)
            // but rotate around the parent (tracker) transform

            // or use head transform that fix the global transform and use it to determine the front, left/right axies

            if (humanBodyBone == HumanBodyBones.LeftHand)
            {

            }
            else if (humanBodyBone == HumanBodyBones.RightHand)
            {

            }
        }
    }
}
