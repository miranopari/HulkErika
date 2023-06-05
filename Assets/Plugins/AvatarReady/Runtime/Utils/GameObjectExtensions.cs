using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : MonoBehaviour
        {
            T script = obj.GetComponent<T>();
            if (script == null)
                script = obj.AddComponent<T>();
            return script;
        }
    }
}
