using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public abstract class AnimationConfig
    {
        public abstract void ApplyConfiguration(AvatarReady avatarReady, Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors);

        public abstract void ResetConfiguration(AvatarReady avatarReady);
    }
}
