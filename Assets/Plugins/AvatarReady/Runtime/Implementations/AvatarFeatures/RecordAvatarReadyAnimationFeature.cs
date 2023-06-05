using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Record AvatarReady animations", "Generate custom-made .json file using Animator transforms")]
    [AvatarReadySupportedProvider(typeof(AvatarProvider))]
    [DefaultExecutionOrder(20000)]
    public class RecordAvatarReadyAnimationFeature : AvatarFeature
    {
        public bool Record;
        [Tooltip("Animation name will be in format : yyyy-MM-dd-HH-mm-ss_<AnimationName>")]
        public string AnimationName;

        private Animator animator;

        private List<SkeletonSnapshot> skeletonSnapshots = new List<SkeletonSnapshot>();

        private bool fileOpen;
        private StreamWriter file;
        private string path;

        private bool first = true;

        public static string AnimationsFolder
        {
            get => Application.persistentDataPath + "/AvatarReadyAnimations/";
        }

        private class SkeletonSnapshot
        {
            public int frame;
            public float time;
            public Vector3 rootPosition;
            public Quaternion rootRotation;
            public Vector3 rootScale;
            public Dictionary<HumanBodyBones, Quaternion> skeletonRotations;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            if (Record)
            {
                SkeletonSnapshot skeletonSnapshot = new SkeletonSnapshot();
                skeletonSnapshot.frame = Time.frameCount;
                skeletonSnapshot.time = Time.time;

                skeletonSnapshot.rootPosition = transform.position;
                skeletonSnapshot.rootRotation = transform.rotation;
                skeletonSnapshot.rootScale = transform.localScale;

                Dictionary<HumanBodyBones, Quaternion> skeleton = new Dictionary<HumanBodyBones, Quaternion>();
                foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
                {
                    if (bone == HumanBodyBones.LastBone)
                        continue;

                    //Debug.Log(bone);
                    Transform t = animator.GetBoneTransform(bone);
                    skeleton.Add(bone, t.localRotation);
                }
                skeletonSnapshot.skeletonRotations = skeleton;

                skeletonSnapshots.Add(skeletonSnapshot);

                if (skeletonSnapshots.Count >= 64)
                {
                    WriteToFile();
                }
            }
        }

        private void WriteToFile()
        {
            if (!fileOpen)
                OpenFile();

            Write();
        }

        private void OpenFile()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string name = string.IsNullOrWhiteSpace(AnimationName) ? this.name : AnimationName;

            path = AnimationsFolder + date + "_" + name + ".json";

            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);


            file = new StreamWriter(path);
            fileOpen = true;

            file.WriteLine("{");
            file.WriteLine("\t\"animationName\": \"{0}\",", name);
            file.WriteLine("\t\"date\": \"{0}\",", date);
            file.WriteLine("\t\"skeletonSnapshots\": [");
        }

        private void Write()
        {
            foreach (SkeletonSnapshot skeletonSnapshot in skeletonSnapshots)
            {
                JSONObject jsonSkeletonSnapshot = new JSONObject();

                jsonSkeletonSnapshot.Add("frame", new JSONNumber(skeletonSnapshot.frame));
                jsonSkeletonSnapshot.Add("time", new JSONNumber(skeletonSnapshot.time));
                jsonSkeletonSnapshot.Add("rootPosition", new JSONObject().WriteVector3(skeletonSnapshot.rootPosition));
                jsonSkeletonSnapshot.Add("rootRotation", new JSONObject().WriteQuaternion(skeletonSnapshot.rootRotation));
                jsonSkeletonSnapshot.Add("rootScale", new JSONObject().WriteVector3(skeletonSnapshot.rootScale));

                JSONArray jsonSkeletonRotations = new JSONArray();
                foreach (var rotation in skeletonSnapshot.skeletonRotations)
                {
                    JSONObject jsonRotation = new JSONObject();
                    jsonRotation.Add("bone", new JSONString(rotation.Key.ToString()));
                    jsonRotation.WriteQuaternion(rotation.Value);
                    jsonSkeletonRotations.Add(jsonRotation);
                }

                jsonSkeletonSnapshot.Add("skeletonRotations", jsonSkeletonRotations);

                if (first)
                    first = false;
                else
                    file.Write(",");

                file.WriteLine(jsonSkeletonSnapshot.ToString(4));
            }

            skeletonSnapshots.Clear();
        }

        private void OnDestroy()
        {
            if (skeletonSnapshots.Count > 0)
            {
                WriteToFile();
            }

            if (fileOpen)
            {
                file.WriteLine("\t]");
                file.WriteLine("}");

                file.Close();

                Debug.LogFormat("animation file created at : {0}", path);
            }
        }
    }
}