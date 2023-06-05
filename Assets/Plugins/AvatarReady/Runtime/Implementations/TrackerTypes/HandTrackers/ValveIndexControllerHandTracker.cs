using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    /// <summary>
    /// Hand offset for the Valve Index controller.
    /// The offset position between the controller tracking position and actual wrist position.
    /// Hardware dependant.
    /// </summary>
    public class ValveIndexControllerHandTracker : HandTracker
    {
        public override void ApplyInitialOffset(HumanBodyBones humanBodyBone)
        {
            Assert.IsTrue(humanBodyBone == HumanBodyBones.LeftHand || humanBodyBone == HumanBodyBones.RightHand);

            if (humanBodyBone == HumanBodyBones.LeftHand)
                transform.localPosition = new Vector3(-0.042f, 0.064f, -0.145f);
            else if (humanBodyBone == HumanBodyBones.RightHand)
                transform.localPosition = new Vector3(0.042f, 0.064f, -0.145f);

            transform.localRotation = Quaternion.Euler(-25, 0, 180);
        }
    }
}
