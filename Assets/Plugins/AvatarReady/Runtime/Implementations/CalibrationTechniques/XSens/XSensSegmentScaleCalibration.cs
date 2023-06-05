#if XSENS
using xsens;

namespace Inria.Avatar.AvatarReady
{

    [AvatarReadyName("Segment scale for XSens")]
    [AvatarReadySupportedSystem(typeof(XSensAnimationSystem))]
    public class XSensSegmentScaleCalibration : CalibrationTechniqueSetting
    {
        public override void ApplySetting(AvatarReady avatarReady)
        {
            XsLiveAnimator xsLiveAnimator = avatarReady.gameObject.GetComponent<XsLiveAnimator>();
            xsLiveAnimator.scale_mode = XsLiveAnimator.ScalingMode.SEGMENT_SCALE;
        }

        public override void ResetSetting(AvatarReady avatarReady)
        {
            XsLiveAnimator xsLiveAnimator = avatarReady.gameObject.GetComponent<XsLiveAnimator>();
            if (xsLiveAnimator != null)
                xsLiveAnimator.scale_mode = XsLiveAnimator.ScalingMode.NO_SCALE;
        }
    }
}
#endif