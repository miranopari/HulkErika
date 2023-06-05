using System;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AvatarReadySupportedTrackerAttribute : Attribute
    {
        public enum Necessity
        {
            Required,
            Optional,
        }

        public int order;
        public string trackerName;
        public HumanBodyBones humanBodyBone;
        public Type trackerType;
        public Necessity necessity;

        public AvatarReadySupportedTrackerAttribute(int order, string trackerName, HumanBodyBones humanBodyBone, Type trackerType, Necessity necessity = Necessity.Required)
        {
            if (string.IsNullOrWhiteSpace(trackerName))
                throw new ArgumentException(nameof(trackerName) + " is mandatory and should not be empty");

            if (!trackerType.IsSubclassOf(typeof(TrackerType)))
                throw new ArgumentException("The given " + nameof(trackerType) + " does not extend '" + nameof(TrackerType) + "' and thus is not valid");

            this.order = order;
            this.trackerName = trackerName;
            this.humanBodyBone = humanBodyBone;
            this.trackerType = trackerType;
            this.necessity = necessity;
        }
    }
}
