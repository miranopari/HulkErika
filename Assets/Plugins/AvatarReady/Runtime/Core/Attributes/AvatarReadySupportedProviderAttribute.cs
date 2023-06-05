using System;

namespace Inria.Avatar.AvatarReady
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AvatarReadySupportedProviderAttribute : Attribute
    {
        public Type supportedProvider;
        public bool enabledByDefault;

        public AvatarReadySupportedProviderAttribute(Type supportedProvider, bool enabledByDefault = false)
        {
            if (!typeof(AvatarProvider).IsAssignableFrom(supportedProvider))
                throw new ArgumentException("The given " + nameof(supportedProvider) + " does not extend '" + nameof(AvatarProvider) + "' and thus is not valid");

            this.supportedProvider = supportedProvider;
            this.enabledByDefault = enabledByDefault;
        }
    }
}
