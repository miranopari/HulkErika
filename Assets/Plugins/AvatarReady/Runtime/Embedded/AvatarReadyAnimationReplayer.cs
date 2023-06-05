using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public class AvatarReadyAnimationReplayer : MonoBehaviour
    {
        public string Animation;

        public bool Play;

        private Animator animator;

        private bool firstTime = true;

        private StreamReader file;
        private JSONNode json;
        private JSONArray skeletonSnapshots;
        private JSONObject currentSnapshot;

        private int index;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Play)
            {
                if (firstTime)
                {
                    OpenFile();
                    LoadJson();
                    CloseFile();

                    firstTime = false;
                }

                if (index >= skeletonSnapshots.Count)
                {
                    Play = false;
                    index = 0;
                    return;
                }

                // TODO smooth movements if needed
                // TODO control time and frame overlap/skip
                // TODO be frame independant

                currentSnapshot = skeletonSnapshots[index].AsObject;

                transform.position = currentSnapshot["rootPosition"].ReadVector3();
                transform.rotation = currentSnapshot["rootRotation"].ReadQuaternion();
                transform.localScale = currentSnapshot["rootScale"].ReadVector3();

                JSONArray skeletonRotations = currentSnapshot["skeletonRotations"].AsArray;
                foreach (KeyValuePair<string, JSONNode> boneRotationNode in skeletonRotations)
                {
                    JSONObject boneRotation = boneRotationNode.Value.AsObject;

                    Enum.TryParse(boneRotation["bone"], out HumanBodyBones bone);
                    Quaternion rotation = boneRotation.ReadQuaternion();

                    animator.GetBoneTransform(bone).localRotation = rotation;
                }

                index++;
            }
        }

        private void OpenFile()
        {
            string path = Application.persistentDataPath + "/AvatarReadyAnimations/" + Animation + ".json";

            if (!File.Exists(path))
                throw new ArgumentException("Can't find given Animation");

            file = new StreamReader(path);
        }

        private void LoadJson()
        {
            string content = file.ReadToEnd();
            json = JSON.Parse(content);

            skeletonSnapshots = json["skeletonSnapshots"].AsArray;
        }

        private void CloseFile()
        {
            file.Close();
        }
    }
}