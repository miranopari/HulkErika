using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public class SynchronizeXSensPositionWithHmdPosition : MonoBehaviour
    {
        protected Transform leftEyePosition;
        protected Transform rightEyePosition;
        protected Transform headTransform;

        public Transform targetCamera;

        public bool calibration = false;

        // Start is called before the first frame update
        void Start()
        {
            Animator anim = gameObject.GetComponent<Animator>();
            leftEyePosition = anim.GetBoneTransform(HumanBodyBones.LeftEye);
            rightEyePosition = anim.GetBoneTransform(HumanBodyBones.RightEye);
            headTransform = anim.GetBoneTransform(HumanBodyBones.Head);

            if (targetCamera == null)
                targetCamera = Camera.main.transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 headPosition = (leftEyePosition.position + rightEyePosition.position) * .5f;

            Vector3 deltaPos = targetCamera.position - headPosition;
            gameObject.transform.position = gameObject.transform.position + deltaPos;

            if (UnityEngine.Input.GetKeyDown(KeyCode.D)) calibration = !calibration;
            if (calibration)
            {
                float deltaRot_y = targetCamera.rotation.eulerAngles.y - headTransform.rotation.eulerAngles.y;
                gameObject.transform.Rotate(new Vector3(0, deltaRot_y, 0));
                calibration = false;
            }
        }
    }
}
