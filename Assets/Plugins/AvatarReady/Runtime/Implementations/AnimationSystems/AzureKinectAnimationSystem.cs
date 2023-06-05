#if AZUREKINECT
using System;
using UnityEngine;
using kinect;

namespace Inria.Avatar.AvatarReady
{
    /* Azure Kinect tracking system implementation for the AvatarReady component. 
     * IMPORTANT NOTE: please add a Controller to your character's Animator for 
     * this to work (it can be empty). */
    [AvatarReadyName("AzureKinect")]
    public class AzureKinectAnimationSystem : MocapAnimationSystem
    {
        private AzureKinectTrackerHandler trackerHandler;
        private AzureKinectCtrl kinectCtrl;
        private AzureKinectConfigLoader kinectConfig;

        // Instantiate two gameObjects ("AzureKinectTrackerHandler" and "AzureKinectCtrl") required for
        // kinect tracking in the scene. Also attaches this script as a component of the avatar's gameObject.
        public override void Init(AvatarReady avatarReady)  
        {
            // Check if avatar rig is humanoid
            if (!avatarReady.Animator.isHuman)
                throw new NotSupportedException("The Avatar must use the Humanoid Rig");

            // If not already existing, instantiate a "AzureKinectTracker" prefab.
            trackerHandler = UnityEngine.Object.FindObjectOfType<AzureKinectTrackerHandler>();
            if (trackerHandler == null)
            {
                GameObject trackerHandlerObj = GameObject.Instantiate(Resources.Load("Prefabs/AzureKinectTrackerHandler")) as GameObject;
                // Remove "(Clone)" from the object's name
                trackerHandlerObj.name = "AzureKinectTrackerHandler";
                // Retrieve the "AzureKinectTrackerHandler" component it has attached
                trackerHandler = trackerHandlerObj.gameObject.GetComponent<AzureKinectTrackerHandler>();
            }

            // If not already existing, create an empty GameObject called "AzureKinectCtrl"
            kinectCtrl = UnityEngine.Object.FindObjectOfType<AzureKinectCtrl>();
            if (kinectCtrl == null)
            {
                GameObject AzureKinectCtrlObj = new GameObject("AzureKinectCtrl"); //main
                // Attach an "AzureKinectCtrl" component to this object.
                kinectCtrl = AzureKinectCtrlObj.AddComponent<AzureKinectCtrl>();
                // Attach an "AzureKinectConfigLoader" component to this object.
                kinectConfig = AzureKinectCtrlObj.AddComponent<AzureKinectConfigLoader>();
                // Define our tracker instance in our kinect controller parameters
                kinectCtrl.tracker = trackerHandler.gameObject;
            }

            // Attach an "AzureKinectLiveAnimator" component to the avatar 
            AzureKinectLiveAnimator azureKinectLiveAnimator = avatarReady.gameObject.GetOrAddComponent<AzureKinectLiveAnimator>();

            // Fill the parameters of this component
            azureKinectLiveAnimator.trackerHandler = trackerHandler;
            azureKinectLiveAnimator.RootPosition = trackerHandler.gameObject.transform.GetChild(0).FindChildByName("pelvis").gameObject;
            azureKinectLiveAnimator.CharacterRootTransform = avatarReady.Animator.GetBoneTransform(HumanBodyBones.Hips);
        }
        
        // Destroy created components and gameObjects in the hierarchy
        public override void Reset(AvatarReady avatarReady)
        {
            UnityEngine.Object.DestroyImmediate(avatarReady.gameObject.GetComponent<AzureKinectLiveAnimator>());
            UnityEngine.Object.DestroyImmediate(GameObject.Find("AzureKinectCtrl"));
            UnityEngine.Object.DestroyImmediate(GameObject.Find("AzureKinectTrackerHandler"));
        }
    }
}
#endif
