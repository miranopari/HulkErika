namespace Inria.Avatar.AvatarReady
{
    public abstract class CalibrationTechniqueSetting : CalibrationTechnique
    {
        public abstract void ApplySetting(AvatarReady avatarReady);

        public abstract void ResetSetting(AvatarReady avatarReady);
    }
}
