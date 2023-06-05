using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    public class ViveTrackerOnShoeFootTracker : FootTracker
    {
        public override void ApplyInitialOffset(HumanBodyBones humanBodyBone)
        {
            Assert.IsTrue(humanBodyBone == HumanBodyBones.LeftFoot || humanBodyBone == HumanBodyBones.RightFoot);

            if (humanBodyBone == HumanBodyBones.LeftFoot)
                transform.localPosition = new Vector3(0.026f, 0.023f, -0.052f);
            else if (humanBodyBone == HumanBodyBones.RightFoot)
                transform.localPosition = new Vector3(-0.037f, -0.054f, -0.0399f);

            if (humanBodyBone == HumanBodyBones.LeftFoot)
                transform.localRotation = Quaternion.Euler(359.50885f, 359.496155f, 121.053413f);
            else if (humanBodyBone == HumanBodyBones.RightFoot)
                transform.localRotation = Quaternion.Euler(348.637939f, 13.8916388f, 3.14330411f);
        }
    }
}
