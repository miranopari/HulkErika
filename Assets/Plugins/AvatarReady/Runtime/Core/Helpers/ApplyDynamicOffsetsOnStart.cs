using System.Collections;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public class ApplyDynamicOffsetsOnStart : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return null;

            TargetHelper.ApplyDynamicOffsets(gameObject);
        }
    }

}
