using System;

namespace Inria.Avatar.AvatarReady
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AvatarReadyNameAttribute : Attribute
    {
        public string displayName;
        public string desc;

        public AvatarReadyNameAttribute(string displayName, string desc = null)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException(nameof(displayName) + " is mandatory and should not be empty");

            this.displayName = displayName;
            this.desc = desc;
        }
    }
}
