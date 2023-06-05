#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Record Humanoid animations", "Generate a Unity .anim file with Humanoid animation. (Only in Editor)")]
    [AvatarReadySupportedProvider(typeof(AvatarProvider))]
    [DefaultExecutionOrder(20000)]
    public class RecordHumanoidAnimationFeature : AvatarFeature
    {
        // https://gitlab.inria.fr/ldumesni/Integration-Xsens/-/blob/master/Unity%20Project/Assets/Xsens/MvnLive/Scripts/XsAnimationRecorder.cs

        public bool Record;

        [Tooltip("Animation name will be in format : yyyy-MM-dd-HH-mm-ss_<AnimationName>")]
        public string AnimationName;

        private Animator animator;
        private HumanPoseHandler humanPoseHandler;
        private HumanPose humanPose;
        private AnimationCurve[] curves;

        private float time = 0f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);

            humanPose = new HumanPose();
            humanPoseHandler.GetHumanPose(ref humanPose);
            curves = new AnimationCurve[humanPose.muscles.Length];

            for (int i = 0; i < humanPose.muscles.Length; i++)
            {
                curves[i] = new AnimationCurve();
            }
        }

        private void LateUpdate()
        {
            if (Record)
            {
                humanPoseHandler.GetHumanPose(ref humanPose);

                for (int i = 0; i < humanPose.muscles.Length; i++)
                {
                    int success = curves[i].AddKey(time, humanPose.muscles[i]);
                    if (success == -1)
                    {
                        Debug.LogError($"Can't AddKey for {HumanTrait.MuscleName[i]} at time={time} and value={humanPose.muscles[i]}");
                    }
                }

                time += Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            if (curves[0].keys.Length == 0)
                return;

            string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string name = string.IsNullOrWhiteSpace(AnimationName) ? this.name : AnimationName;

            string file = $"{date}_{name}";

            AnimationClip clip = new AnimationClip();
            clip.name = file;

            humanPoseHandler.GetHumanPose(ref humanPose);
            for (int i = 0; i < humanPose.muscles.Length; i++)
            {
                clip.SetCurve("", typeof(Animator), HumanTrait.MuscleName[i], curves[i]);
            }
            clip.EnsureQuaternionContinuity();

            string animationFolder = Application.dataPath + "/Animations";
            if (!Directory.Exists(animationFolder))
                Directory.CreateDirectory(animationFolder);

            AssetDatabase.CreateAsset(clip, $"Assets/Animations/{file}.anim");
            Debug.Log($"Anim saved at 'Assets/Animation/{file}.anim'");
        }
    }
}
#endif