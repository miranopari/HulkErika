#if FINALIK
using RootMotion.FinalIK;
using System;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("FinalIK")]
    public class FinalIKAnimationSystem : IKAnimationSystem
    {
        public VRIK FinalIK;

        public override void Init(AvatarReady avatarReady)
        {
            if (!avatarReady.Animator.isHuman)
                throw new NotSupportedException("The Avatar must use the Humanoid Rig");

            // VRIK
            FinalIK = avatarReady.gameObject.GetOrAddComponent<VRIK>();
        }

        public override void Reset(AvatarReady avatarReady)
        {
            DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<VRIK>());
        }
    }
}
#endif