using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public class AvatarReadySetup : MonoBehaviour
    {
        public bool activated;

        public enum AvatarTypes
        {
            RuntimeImport,
            Static,
        }

        public AvatarTypes AvatarType;

        public string SelectedStaticAvatar;
    }
}
