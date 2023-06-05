using System;

namespace Inria.Avatar.AvatarReady
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AvatarReadySupportedSystemAttribute : Attribute
    {
        public Type supportedSystem;

        public AvatarReadySupportedSystemAttribute(Type supportedSystem)
        {
            if (!typeof(AnimationSystem).IsAssignableFrom(supportedSystem))
                throw new ArgumentException("The given " + nameof(supportedSystem) + " does not extend '" + nameof(AnimationSystem) + "' and thus is not valid");

            this.supportedSystem = supportedSystem;
        }
    }
}
