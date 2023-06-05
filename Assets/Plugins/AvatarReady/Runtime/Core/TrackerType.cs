using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public abstract class TrackerType : MonoBehaviour
    {
        /// <summary>
        /// Called during the avatar configuration in editor.
        /// </summary>
        public virtual void ApplyInitialOffset(HumanBodyBones humanBodyBone) { }

        /// <summary>
        /// Called after the avatar calibration in runtime.
        /// </summary>
        public virtual void ApplyDynamicOffset(Animator animator) { }
    }
}
