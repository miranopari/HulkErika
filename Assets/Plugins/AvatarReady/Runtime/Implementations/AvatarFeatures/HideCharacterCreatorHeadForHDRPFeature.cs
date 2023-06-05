#if USING_HDRP

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Hide self avatar head")]
    [AvatarReadySupportedProvider(typeof(CharacterCreatorAvatarProvider), true)]
    public class HideCharacterCreatorHeadForHDRPFeature : AvatarFeature
    {
        [Tooltip("If empty, the camera tagged 'MainCamera' will be use")]
        public Camera CameraToUse;

        private Dictionary<Renderer, Dictionary<int, Material>> materialsToHide = new Dictionary<Renderer, Dictionary<int, Material>>();
        public Material InvisibleMaterial;

        // if a material contains this word, make it invisible for the CameraToUse
        private List<string> matNames = new List<string>()
        {
            "head",
            "eye",
            "cornea",
            "tearline",
            "teeth",
            "tongue",
            "beard",
            "hair",
            "scalp",
            "polytail",
            "bushy",
            "angled",
        };
        private bool inCameraRender = false;

        private void Awake()
        {
            if (CameraToUse == null)
                CameraToUse = Camera.main;
            Assert.IsNotNull(CameraToUse, "Assign the camera that will hide the materials");

            if (InvisibleMaterial == null)
                InvisibleMaterial = Resources.Load<Material>("InvisibleMaterial");
            Assert.IsNotNull(InvisibleMaterial, "Assign an invisible material or add a material named 'InvisibleMaterial' in a 'Resources' folder");
        }

        private void Start()
        {
            // compute materials to hide
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                materialsToHide.Add(renderer, new Dictionary<int, Material>());

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    Material mat = renderer.materials[i];
                    if (matNames.Any(name => mat.name.ToLower().Contains(name)))
                    {
                        materialsToHide[renderer].Add(i, mat);
                    }
                }
            }

            // hide head callbacks
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        private void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }

        // execution flow goes :
        // begin editor camera      // do nothing
        // > begin mirror camera    // do nothing
        // > end mirror camera      // do nothing
        // end editor camera        // do nothing
        // begin main camera        // set invisible mats
        // > begin mirror camera    // restore mats
        // > end mirror camera      // set invisible mats
        // end main camera          // restore mats

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == CameraToUse)
            {
                //  change needed materials to an invisible one
                SetInvisibleMaterials();
                inCameraRender = true;
            }

            // Mirror / PlanarReflectionProbe exception
            if (inCameraRender && (camera.cameraType == CameraType.Reflection || camera.name.ToLower().Contains("mirror")))
            {
                RestoreInitialMaterials();
            }
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            // Mirror / PlanarReflectionProbe exception
            if (inCameraRender && (camera.cameraType == CameraType.Reflection || camera.name.ToLower().Contains("mirror")))
            {
                SetInvisibleMaterials();
            }

            if (camera == CameraToUse)
            {
                // restore materials
                RestoreInitialMaterials();
                inCameraRender = false;
            }
        }

        private void SetInvisibleMaterials()
        {
            foreach (var matData in materialsToHide)
            {
                if (matData.Value.Count != 0)
                {
                    Material[] mats = matData.Key.materials;
                    foreach (int matIndex in matData.Value.Keys)
                    {
                        mats[matIndex] = InvisibleMaterial;
                    }
                    matData.Key.materials = mats;
                }
            }
        }

        private void RestoreInitialMaterials()
        {
            foreach (var matData in materialsToHide)
            {
                if (matData.Value.Count != 0)
                {
                    Material[] mats = matData.Key.materials;
                    foreach (int matIndex in matData.Value.Keys)
                    {
                        mats[matIndex] = matData.Value[matIndex];
                    }
                    matData.Key.materials = mats;
                }
            }
        }
    }
}

#endif