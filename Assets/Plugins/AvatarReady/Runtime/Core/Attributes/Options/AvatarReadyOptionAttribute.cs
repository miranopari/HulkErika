using System;

namespace Inria.Avatar.AvatarReady
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class AvatarReadyOptionAttribute : Attribute
    {
        public int order;
        public string displayName;
        public string variableName;

        public AvatarReadyOptionAttribute(int order, string displayName, string variableName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException(nameof(displayName) + " is mandatory and should not be empty");

            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException(nameof(displayName) + " is mandatory and should not be empty");

            this.order = order;
            this.displayName = displayName;
            this.variableName = variableName;
        }
    }
}
