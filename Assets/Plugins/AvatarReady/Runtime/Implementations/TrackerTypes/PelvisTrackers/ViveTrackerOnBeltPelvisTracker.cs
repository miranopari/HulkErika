using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    public class ViveTrackerOnBeltPelvisTracker : PelvisTracker
    {
        public override void ApplyInitialOffset(HumanBodyBones humanBodyBone)
        {
            Assert.IsTrue(humanBodyBone == HumanBodyBones.Hips);

            transform.localPosition = new Vector3(-0.0010f, 0.0149f, -0.123f);

            transform.localRotation = Quaternion.Euler(326.194092f, 241.495758f, 112.881615f);
        }
    }
}
