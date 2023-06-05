using UnityEngine;
using UnityEngine.Events;

namespace Inria.Avatar.AvatarReady
{
    public class AvatarReadyRuntimeLink : MonoBehaviour
    {
        /// <summary>
        /// Called when the new avatar is ready, the avatar gameObject is given in parameter.
        /// </summary>
        public UnityEvent<GameObject> OnAvatarLoaded;

        /// <summary>
        /// Called when an Avatar is about to get destroyed (and a new one will replace it).
        /// Use it to clear links between scripts and the avatar about to be Destroy.
        /// </summary>
        public UnityEvent OnAvatarDestroy;
    }
}
