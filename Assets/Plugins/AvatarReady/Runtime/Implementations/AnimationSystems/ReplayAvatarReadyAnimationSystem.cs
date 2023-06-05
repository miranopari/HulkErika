using System;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Replay AvatarReady animations")]
    public class ReplayAvatarReadyAnimationSystem : ReplayAnimationSystem
    {
        public override void Init(AvatarReady avatarReady)
        {
            if (!avatarReady.Animator.isHuman)
                throw new NotSupportedException("The Avatar must use the Humanoid Rig");

            avatarReady.gameObject.GetOrAddComponent<AvatarReadyAnimationReplayer>();
        }

        public override void Reset(AvatarReady avatarReady)
        {
            DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<AvatarReadyAnimationReplayer>());
        }
    }
}
