using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Character Creator Facial Animation Helper")]
    [AvatarReadySupportedProvider(typeof(CharacterCreatorAvatarProvider), true)]
    public partial class CharacterCreatorFacialAnimationFeature : AvatarFeature
    {
        private List<SkinnedMeshRenderer> skinnedMeshRenderers;

        private class BlendshapeData
        {
            public float blendshapeWeight;
            public Dictionary<SkinnedMeshRenderer, int> blendshapeLink;
        }

        // blendshape_name -> weight + [ mesh -> blendshape_index ]
        private Dictionary<string, BlendshapeData> blendshapesWeight = new Dictionary<string, BlendshapeData>();

        private void Awake()
        {
            // skinnedMeshRenderers
            skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (skinnedMeshRenderer.sharedMesh.blendShapeCount != 0)
                    skinnedMeshRenderers.Add(skinnedMeshRenderer);
            }

            AwakeData();

            AwakeProcedural();
        }

        private void Start()
        {
            StartProcedural();
        }

        private void Update()
        {
            // Set weight here or inside coroutine by calling SetBlendshape() every frame

            UpdateProcedural();
        }

        private void LateUpdate()
        {
            // Actually apply blendshapes to meshes

            foreach (var blendshape in blendshapesWeight)
            {
                // Set blendshape on every skinnedMeshRenderer
                foreach (var mesh in blendshape.Value.blendshapeLink)
                {
                    mesh.Key.SetBlendShapeWeight(mesh.Value, blendshape.Value.blendshapeWeight);
                }

                // Reset weight
                blendshape.Value.blendshapeWeight = 0;
            }
        }

        /// <summary>
        /// Call this method every frame to set the desired blendshape
        /// </summary>
        /// <param name="weight">Weight between 0 and 100</param>
        public void SetBlendshape(string blendshapeName, float weight)
        {
            if (!blendshapesWeight.ContainsKey(blendshapeName))
            {
                Dictionary<SkinnedMeshRenderer, int> dict = new Dictionary<SkinnedMeshRenderer, int>();
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    int index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendshapeName);
                    if (index != -1)
                        dict.Add(skinnedMeshRenderer, index);
                }

                BlendshapeData var = new BlendshapeData() { blendshapeWeight = weight, blendshapeLink = dict };

                blendshapesWeight.Add(blendshapeName, var);
            }

            // Store only the max value of the frame
            // TODO maybe use a priority settings or a average computation to deal with multiple call on the same blendshape
            blendshapesWeight[blendshapeName].blendshapeWeight = Mathf.Max(blendshapesWeight[blendshapeName].blendshapeWeight, weight);
        }
    }
}
