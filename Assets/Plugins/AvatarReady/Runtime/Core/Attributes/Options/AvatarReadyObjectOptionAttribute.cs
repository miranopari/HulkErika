using System;

namespace Inria.Avatar.AvatarReady
{
    /// <summary>
    /// Only support UnityEngine.Object and subclasses.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AvatarReadyObjectOptionAttribute : AvatarReadyOptionAttribute
    {
        public Type ObjectType;

        public AvatarReadyObjectOptionAttribute(int order, string displayName, string variableName, Type objectType)
            : base(order, displayName, variableName)
        {
            if (objectType == null)
                throw new ArgumentException(nameof(objectType) + " is mandatory and should not be null");

            if (!objectType.IsSubclassOf(typeof(UnityEngine.Object)))
                throw new ArgumentException(objectType.Name + " is not a UnityEngine.Object");

            ObjectType = objectType;
        }
    }
}
