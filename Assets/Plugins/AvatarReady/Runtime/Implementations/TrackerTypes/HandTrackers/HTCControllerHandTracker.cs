using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    /// <summary>
    /// Hand offset for the HTC Vive controller
    /// The offset position between the controller tracking position and actual wrist position.
    /// Hardware dependant.
    /// </summary>
    public class HTCControllerHandTracker : HandTracker
    {
        public override void ApplyInitialOffset(HumanBodyBones humanBodyBone)
        {
            Assert.IsTrue(humanBodyBone == HumanBodyBones.LeftHand || humanBodyBone == HumanBodyBones.RightHand);

            if (humanBodyBone == HumanBodyBones.LeftHand)
                transform.localPosition = new Vector3(-0.015f, 0.02f, -0.07f);
            else if (humanBodyBone == HumanBodyBones.RightHand)
                transform.localPosition = new Vector3(0.015f, 0.02f, -0.07f);

            transform.localRotation = Quaternion.Euler(-55, 0, 180);
        }
    }
}
