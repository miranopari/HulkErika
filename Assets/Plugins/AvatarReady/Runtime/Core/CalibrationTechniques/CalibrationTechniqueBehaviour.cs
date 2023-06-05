using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public abstract class CalibrationTechniqueBehaviour : MonoBehaviour, CalibrationTechnique
    {
        public abstract void InitBehaviour(AvatarReady avatarReady, Dictionary<HumanBodyBones, TargetHelper.TrackerDescriptor> trackerDescriptors);

        public abstract void ResetBehaviour();

        public abstract void Calibrate(CalibrationProfile calibrationProfile = null);

        public abstract float GetData(string dataKey);
    }
}
