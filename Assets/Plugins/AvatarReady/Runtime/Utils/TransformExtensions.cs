using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public static class TransformExtensions
    {
        public static Transform FindChildByName(this Transform t, string beginOfChildName)
        {
            foreach (Transform child in t)
            {
                if (child.name.StartsWith(beginOfChildName))
                    return child;
                var result = child.FindChildByName(beginOfChildName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
