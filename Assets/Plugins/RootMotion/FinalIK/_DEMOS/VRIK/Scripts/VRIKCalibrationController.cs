using UnityEngine;
using RootMotion.FinalIK;

namespace RootMotion.Demos
{

    public class VRIKCalibrationController : MonoBehaviour
    {
        /// ADDED BY ADE
        [Tooltip("ADDED BY ADE. To make the uer experience the height of Hulk, put 2.22f")] public float playerHeight = 2.22f; 
        [Tooltip("ADDED BY ADE. Slide the Camera under CameraRig Go")] public GameObject CameraRig;

        [Tooltip("Reference to the VRIK component on the avatar.")] public VRIK ik;
        [Tooltip("The settings for VRIK calibration.")] public VRIKCalibrator.Settings settings;
        [Tooltip("The HMD.")] public Transform headTracker;
        [Tooltip("(Optional) A tracker placed anywhere on the body of the player, preferrably close to the pelvis, on the belt area.")] public Transform bodyTracker;
        [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's left hand.")] public Transform leftHandTracker;
        [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's right hand.")] public Transform rightHandTracker;
        [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's left leg.")] public Transform leftFootTracker;
        [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's right leg.")] public Transform rightFootTracker;

        [Header("Data stored by Calibration")]
        public VRIKCalibrator.CalibrationData data = new VRIKCalibrator.CalibrationData();

        /// ADDED BY ADE
        private void Resize()
        {
            Camera VRCamera = CameraRig.GetComponentInChildren<Camera>();
            float headHeight = VRCamera.transform.localPosition.y;
            
            float scale = playerHeight / headHeight;
            CameraRig.transform.localScale = Vector3.one * scale;
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                /// ADDED BY ADE: scales the player to fit Hulk's height
                Resize();

                // Calibrate the character, store data of the calibration
                data = VRIKCalibrator.Calibrate(ik, settings, headTracker, bodyTracker, leftHandTracker, rightHandTracker, leftFootTracker, rightFootTracker);

                // ADDED BY ADE: Display the avatar once calibrated
                CameraRig.GetComponentInChildren<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Body");
            }

            /*
             * calling Calibrate with settings will return a VRIKCalibrator.CalibrationData, which can be used to calibrate that same character again exactly the same in another scene (just pass data instead of settings), 
             * without being dependent on the pose of the player at calibration time.
             * Calibration data still depends on bone orientations though, so the data is valid only for the character that it was calibrated to or characters with identical bone structures.
             * If you wish to use more than one character, it would be best to calibrate them all at once and store the CalibrationData for each one.
             * */
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (data.scale == 0f)
                {
                    Debug.LogError("No Calibration Data to calibrate to, please calibrate with settings first.");
                }
                else
                {
                    // Use data from a previous calibration to calibrate that same character again.
                    VRIKCalibrator.Calibrate(ik, data, headTracker, bodyTracker, leftHandTracker, rightHandTracker, leftFootTracker, rightFootTracker);
                }
            }

            // Recalibrates avatar scale only. Can be called only if the avatar has been calibrated already.
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (data.scale == 0f)
                {
                    Debug.LogError("Avatar needs to be calibrated before RecalibrateScale is called.");
                }
                VRIKCalibrator.RecalibrateScale(ik, data, settings);
            }
        }
    }
}
