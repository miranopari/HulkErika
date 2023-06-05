using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public static class DestroyUtils
    {
        /// <summary>
        /// Destroys a UnityEngine.Object safely.
        /// Use either Destroy or DestroyImmediate when needed.
        /// </summary>
        /// <param name="obj">Object to be destroyed.</param>
        public static void Destroy(Object obj)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying && !UnityEditor.EditorApplication.isPaused)
                    Object.Destroy(obj);
                else
                    Object.DestroyImmediate(obj);
#else
                Object.Destroy(obj);
#endif
            }
        }
    }
}