using System;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AvatarReadyCalibrationData : Attribute
    {
        public string DataKey;
        public string DataDesc;

        public AvatarReadyCalibrationData(string dataKey, string dataDesc)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataKey));

            DataKey = dataKey;
            DataDesc = dataDesc;
        }
    }
}
