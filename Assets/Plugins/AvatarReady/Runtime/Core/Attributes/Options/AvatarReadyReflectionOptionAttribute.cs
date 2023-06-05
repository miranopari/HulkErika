using System;

namespace Inria.Avatar.AvatarReady
{
    /// <summary>
    /// Beware, this method is complex and use reflection.
    /// Only use it if you know what you are doing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AvatarReadyReflectionOptionAttribute : AvatarReadyOptionAttribute
    {
        public string methodOptions;

        public AvatarReadyReflectionOptionAttribute(int order, string displayName, string variableName, string methodOptions)
            : base(order, displayName, variableName)
        {
            if (string.IsNullOrWhiteSpace(methodOptions))
                throw new ArgumentException(nameof(methodOptions) + " is mandatory and should not be empty");

            this.methodOptions = methodOptions;
        }
    }
}
