#if FINALIK
using RootMotion.FinalIK;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public class FixFinalIKLocomotionDesync : MonoBehaviour
    {
        private VRIK finalIK;

        private Transform avatarHead;
        private Transform userHead;

        private Renderer[] renderers;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void Start()
        {
            finalIK = GetComponent<VRIK>();
            if (finalIK == null)
                throw new MissingComponentException("Missing VRIK");

            finalIK.solver.OnPostUpdate += TestDistance;
        }

        private void TestDistance()
        {
            if (finalIK.references.head == null || finalIK.solver.spine.headTarget == null)
                return;

            if (Vector3.Distance(finalIK.references.head.position, finalIK.solver.spine.headTarget.position) > 0.3f)
            {
                foreach (Renderer rendrer in renderers)
                {
                    if (rendrer.enabled == true)
                        rendrer.enabled = false;
                }
            }
            else
            {
                foreach (Renderer rendrer in renderers)
                {
                    if (rendrer.enabled == false)
                        rendrer.enabled = true;
                }
            }

            /*
            if (Vector3.Distance(finalIK.references.head.position, finalIK.solver.spine.headTarget.position) > 0.3f)
            {
                for (int i = 0; i < 30; i++)
                    finalIK.UpdateSolverExternal();
            }
            */
        }
    }
}
#endif