using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Replay AvatarReady animation file")]
    [AvatarReadySupportedSystem(typeof(ReplayAvatarReadyAnimationSystem))]
    [AvatarReadyReflectionOption(0, "Animation", "animation", nameof(GetAnimationFilesOptions))]
    public class ReplayAvatarReadyConfig : AnimationConfig
    {
        public override void ApplyConfiguration(AvatarReady avatarReady, Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors)
        {
            AvatarReadyAnimationReplayer animationReplayer = avatarReady.GetComponent<AvatarReadyAnimationReplayer>();

            animationReplayer.Animation = avatarReady.ConfigStringOptions["animation"];
        }

        public override void ResetConfiguration(AvatarReady avatarReady)
        {
            AvatarReadyAnimationReplayer animationReplayer = avatarReady.GetComponent<AvatarReadyAnimationReplayer>();

            if (animationReplayer != null)
            {
                animationReplayer.Animation = avatarReady.ConfigStringOptions["animation"];
            }
        }

        public static string[] GetAnimationFilesOptions()
        {
            string path = Application.persistentDataPath + "/AvatarReadyAnimations/";

            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string[] files = Directory.GetFiles(path, "*.json");

            return files.Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
        }
    }
}
