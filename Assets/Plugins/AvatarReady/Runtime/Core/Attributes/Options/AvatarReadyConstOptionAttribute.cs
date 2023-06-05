using System;

namespace Inria.Avatar.AvatarReady
{
    /// <summary>
    /// Only support Enum, bool, int, float, double and string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AvatarReadyConstOptionAttribute : AvatarReadyOptionAttribute
    {
        public Type variableType;
        public object defaultValue;

        public AvatarReadyConstOptionAttribute(int order, string displayName, string variableName, Type variableType, object defaultValue)
            : base(order, displayName, variableName)
        {
            if (variableType == null)
                throw new ArgumentException(nameof(displayName) + " is mandatory and should not be null");

            if (defaultValue == null)
                throw new ArgumentException(nameof(displayName) + " is mandatory and should not be null");

            this.variableType = variableType;
            this.defaultValue = defaultValue;
        }
    }
}
