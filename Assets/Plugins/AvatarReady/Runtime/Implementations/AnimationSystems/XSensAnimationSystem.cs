#if XSENS
using System;
using UnityEngine;
using xsens;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("XSens")]
    [AvatarReadyObjectOption(0, "Headset", "headset", typeof(Transform))]
    public class XSensAnimationSystem : MocapAnimationSystem
    {
        public override void Init(AvatarReady avatarReady)
        {
            if (!avatarReady.Animator.isHuman)
                throw new NotSupportedException("The Avatar must use the Humanoid Rig");

            if (!avatarReady.SystemObjectOptions.ContainsKey("headset"))
                throw new Exception("Headset option is mandatory and must not be null");

            XsStreamReader xsStreamReader = UnityEngine.Object.FindObjectOfType<XsStreamReader>();
            if (xsStreamReader == null)
            {
                GameObject mvnActorsObj = new GameObject("MvnActors");
                xsStreamReader = mvnActorsObj.AddComponent<XsStreamReader>();
            }

            // XsLiveAnimator
            XsLiveAnimator xsLiveAnimator = avatarReady.gameObject.GetOrAddComponent<XsLiveAnimator>();
            xsLiveAnimator.mvnActors = xsStreamReader;

            SynchronizeXSensPositionWithHmdPosition synchronizeXSensPositionWithHmdPosition = avatarReady.gameObject.GetOrAddComponent<SynchronizeXSensPositionWithHmdPosition>();

            Transform headset = avatarReady.SystemObjectOptions["headset"] as Transform;

            synchronizeXSensPositionWithHmdPosition.targetCamera = headset;
        }

        public override void Reset(AvatarReady avatarReady)
        {
            DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<XsLiveAnimator>());

            DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<SynchronizeXSensPositionWithHmdPosition>());
        }
    }
}
#endif