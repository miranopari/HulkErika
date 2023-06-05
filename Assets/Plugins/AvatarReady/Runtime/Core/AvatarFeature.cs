using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public abstract class AvatarFeature : MonoBehaviour
    {
        public virtual void InitFeature() { }

        public virtual void ResetFeature() { }
    }
}
