using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public static class TargetHelper
    {
        public static Dictionary<HumanBodyBones, TrackerDescriptor> CreateTargetsAndOffsets(AvatarReady avatarReady)
        {
            Transform parent = avatarReady.transform.parent;
            if (parent == null)
            {
                // If the avatar has no parent, create one and move the avatar
                // It's just a way to avoid creating other objects (ie. targets) in the wild
                GameObject parentObj = new GameObject("Root" + avatarReady.gameObject.name);
                SetParentAndReset(avatarReady.gameObject, parentObj.transform);
                parent = parentObj.transform;
            }

            Dictionary<HumanBodyBones, TrackerDescriptor> trackerDescriptors = new Dictionary<HumanBodyBones, TrackerDescriptor>();

            if (avatarReady.AnimationConfig != null)
            {
                AvatarReadySupportedTrackerAttribute[] supportedTrackers = Attribute.GetCustomAttributes(avatarReady.AnimationConfig, typeof(AvatarReadySupportedTrackerAttribute)) as AvatarReadySupportedTrackerAttribute[];
                supportedTrackers = supportedTrackers.OrderBy(t => t.order).ToArray();

                for (int i = 0; i < avatarReady.Trackers.Length; i++)
                {
                    string trackerName = supportedTrackers[i].trackerName;
                    Transform trackerObject = avatarReady.Trackers[i];
                    HumanBodyBones humanBodyBone = supportedTrackers[i].humanBodyBone;
                    AvatarReadySupportedTrackerAttribute.Necessity trackerNecessity = supportedTrackers[i].necessity;

                    // Validation
                    if (trackerNecessity == AvatarReadySupportedTrackerAttribute.Necessity.Required && trackerObject == null)
                        throw new ArgumentException("The tracker '" + trackerName + "' is mandatory");

                    if (trackerObject != null)
                    {
                        // Target
                        GameObject target = GetOrCreateGameOject(trackerName.Replace(" ", "") + "Target", parent);
                        AddCopyPositionAndRotation(target, trackerObject, CopyPositionAndRotation.Relative.World);

                        // Offset
                        GameObject offset = GetOrCreateGameOject(trackerName.Replace(" ", "") + "Offset", target.transform);
                        if (avatarReady.TrackerTypes[i] != null)
                        {
                            TrackerType trackerType = offset.AddComponent(avatarReady.TrackerTypes[i]) as TrackerType;
                            trackerType.ApplyInitialOffset(humanBodyBone);
                        }

                        TrackerDescriptor trackerDescriptor = new TrackerDescriptor();
                        trackerDescriptor.Name = trackerName;
                        trackerDescriptor.Tracker = trackerObject;
                        trackerDescriptor.Target = target.transform;
                        trackerDescriptor.Offset = offset.transform;

                        trackerDescriptors.Add(humanBodyBone, trackerDescriptor);
                    }
                }
            }

            return trackerDescriptors;
        }

        public static void ReapplyOffsets(AvatarReady avatarReady)
        {
            if (avatarReady.AnimationConfig != null)
            {
                AvatarReadySupportedTrackerAttribute[] supportedTrackers = Attribute.GetCustomAttributes(avatarReady.AnimationConfig, typeof(AvatarReadySupportedTrackerAttribute)) as AvatarReadySupportedTrackerAttribute[];
                supportedTrackers = supportedTrackers.OrderBy(t => t.order).ToArray();

                for (int i = 0; i < avatarReady.Trackers.Length; i++)
                {
                    if (avatarReady.Trackers[i] == null)
                        continue;

                    string trackerName = supportedTrackers[i].trackerName;
                    HumanBodyBones humanBodyBone = supportedTrackers[i].humanBodyBone;

                    Transform parent = avatarReady.transform.parent;

                    Transform target = parent.Find(trackerName.Replace(" ", "") + "Target");
                    if (target == null)
                        throw new Exception("Can't find Target");

                    Transform offset = target.Find(trackerName.Replace(" ", "") + "Offset");
                    if (offset == null)
                        throw new Exception("Can't find Offset");

                    TrackerType currentType = offset.gameObject.GetComponent<TrackerType>();
                    if (currentType != null)
                    {
                        DestroyUtils.Destroy(currentType);
                        offset.localPosition = Vector3.zero;
                        offset.localRotation = Quaternion.identity;
                    }

                    if (avatarReady.TrackerTypes[i] != null)
                    {
                        TrackerType newType = offset.gameObject.AddComponent(avatarReady.TrackerTypes[i]) as TrackerType;
                        newType.ApplyInitialOffset(humanBodyBone);
                    }
                }
            }
        }

        public static void RemoveTargets(AvatarReady avatarReady)
        {
            Transform parent = avatarReady.Animator.transform.parent;
            if (avatarReady.AnimationConfig != null)
            {
                AvatarReadySupportedTrackerAttribute[] supportedTrackers = Attribute.GetCustomAttributes(avatarReady.AnimationConfig, typeof(AvatarReadySupportedTrackerAttribute)) as AvatarReadySupportedTrackerAttribute[];
                supportedTrackers = supportedTrackers.OrderBy(t => t.order).ToArray();

                for (int i = 0; i < avatarReady.Trackers.Length; i++)
                {
                    string trackerName = supportedTrackers[i].trackerName;
                    Transform trackerObject = avatarReady.Trackers[i];

                    if (trackerObject != null)
                    {
                        Transform target = parent.Find(trackerName.Replace(" ", "") + "Target");
                        if (target != null)
                            DestroyUtils.Destroy(target.gameObject);
                    }
                }
            }
        }

        public static void ApplyDynamicOffsets(GameObject rootAvatar)
        {
            foreach (TrackerType trackerType in rootAvatar.transform.parent.GetComponentsInChildren<TrackerType>())
            {
                trackerType.ApplyDynamicOffset(rootAvatar.GetComponent<Animator>());
            }
        }

        public class TrackerDescriptor
        {
            public string Name;
            public Transform Tracker;
            public Transform Target;
            public Transform Offset;
        }

        #region Utils
        private static void SetParentAndReset(GameObject obj, Transform parent)
        {
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        private static GameObject GetOrCreateGameOject(string name, Transform parent)
        {
            GameObject obj;
            Transform t = parent.Find(name);
            if (t == null)
            {
                obj = new GameObject(name);
                SetParentAndReset(obj, parent);
            }
            else
            {
                obj = t.gameObject;
            }
            return obj;
        }

        private static void AddCopyPositionAndRotation(GameObject obj, Transform target, CopyPositionAndRotation.Relative relativeTo)
        {
            CopyPositionAndRotation copy = obj.GetOrAddComponent<CopyPositionAndRotation>();
            copy.transformToCopy = target;
            copy.relative = relativeTo;
        }
        #endregion
    }
}
