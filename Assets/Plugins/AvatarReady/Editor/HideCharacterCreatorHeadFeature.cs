#if !AVATARREADY_HDRP && !AVATARREADY_URP

using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// Component used to hide the avatar head in order to hide it in the VR camera. Upon awake, the component is
/// copied to the main camera in order to support the OnPreRender and OnPostRender.
/// 
/// It follows the same principle than HideCharacterCreatorHeadForHDRPFeature class.
/// 
/// SteamVR: Attach it to the VR camera.
/// 
/// </summary>
/// 
namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Hide avatar head from first person perspective.",
        "Hides all the gameobjects of the head when rendering the view of the VR camera. \n" +
        "This is only needed for a VR mode.")]
    [AvatarReadySupportedProvider(typeof(CharacterCreatorAvatarProvider), true)]
    public class HideCharacterCreatorHeadFeature : AvatarFeature
    {
        //Material which will replace the 
        public Material InvisibleMaterial;

        private Dictionary<Renderer, Dictionary<int, Material>> materialsToHide = new Dictionary<Renderer, Dictionary<int, Material>>();

        [Tooltip("If empty, the camera tagged 'MainCamera' will be used.")]
        public Camera CameraToUse;

        private GameObject AvatarRoot;

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
            "oculos",
            "blinn",
        };

        private void Awake()
        {
            if(GetComponent<Camera>() == null)
            {
                if (CameraToUse == null)
                    CameraToUse = Camera.main;

                Assert.IsNotNull(CameraToUse, "Please assign the main camera flag to the VR camera.");

                //Clone the component in the main camera and destroy this one
                HideCharacterCreatorHeadFeature f = CameraToUse.gameObject.AddComponent<HideCharacterCreatorHeadFeature>();
                f.AvatarRoot = gameObject;
                Destroy(this);
            }
            else
            {
                //This should happen on the awake of the component on the main camera.
                if (InvisibleMaterial == null)
                    InvisibleMaterial = Resources.Load<Material>("InvisibleMaterial");
                Assert.IsNotNull(InvisibleMaterial, "Assign an invisible material or add a material named 'InvisibleMaterial' in a 'Resources' folder");
            }
        }

        private void Start()
        {
            Assert.IsNotNull(AvatarRoot, "Something went wrong.");

            // compute materials to hide
            foreach (Renderer renderer in AvatarRoot.GetComponentsInChildren<Renderer>())
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
        }

        void OnPreRender()
        {
            SetInvisibleMaterials();
        }

        void OnPostRender()
        {
            RestoreInitialMaterials();
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