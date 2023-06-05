using UnityEditor;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public class ShowSkeleton : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //Gizmos.color = new Color32(41, 161, 214, 255);

            Handles.color = new Color32(41, 161, 214, 255);
            Handles.lighting = true;

            DrawSkeleton(transform, 20);
        }

        private void DrawSkeleton(Transform t, float thickness)
        {
            foreach (Transform child in t)
            {
                if (child.name == "CC_Base_R_RibsTwist" || child.name == "CC_Base_L_RibsTwist" || child.name == "CC_Base_JawRoot")
                    continue;

                if (t.name != "CC_Base_BoneRoot")
                {
                    Gizmos.DrawSphere(child.position, thickness / 800f);
#if UNITY_2020_2_OR_NEWER
                    Handles.DrawLine(t.position, child.position, thickness);
#else
                    Handles.DrawLine(t.position, child.position);
#endif
                }
                DrawSkeleton(child, thickness - 2);
            }
        }
#endif
    }
}
