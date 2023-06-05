#if SAFBIK
using SA;
using System;

namespace Inria.Avatar.AvatarReady
{
    // https://github.com/Stereoarts/SAFullBodyIK
    [AvatarReadyName("SAFBIK")]
    public class SAFBIKAnimationSystem : IKAnimationSystem
    {

        public override void Init(AvatarReady avatarReady)
        {
            if (!avatarReady.Animator.isHuman)
                throw new NotSupportedException("The Avatar must use the Humanoid Rig");

            // SAFBIK
            FullBodyIKBehaviour safbik = avatarReady.gameObject.GetOrAddComponent<FullBodyIKBehaviour>();

            safbik.fullBodyIK.Awake(avatarReady.transform);
        }

        public override void Reset(AvatarReady avatarReady)
        {
            DestroyUtils.Destroy(avatarReady.gameObject.GetComponent<FullBodyIKBehaviour>());
        }
    }
}
#endif