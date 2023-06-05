using UnityEditor;

namespace Inria.Avatar.AvatarReady
{
    public class AvatarReadyMenuItems
    {
        [MenuItem("AvatarReady/Open Avatar AssetBundle cache folder...")]
        private static void OpenAvatarAssetBundleCacheFolder()
        {
            EditorUtility.RevealInFinder(AvatarCache.AvatarCacheFolder);
        }

        [MenuItem("AvatarReady/Open Avatar Profile folder...")]
        private static void OpenAvatarProfilesFolder()
        {
            EditorUtility.RevealInFinder(CalibrationProfilesManager.CalibrationProfilesFolder);
        }

        [MenuItem("AvatarReady/Open Avatar Animations folder...")]
        private static void OpenAvatarAnimationsFolder()
        {
            EditorUtility.RevealInFinder(RecordAvatarReadyAnimationFeature.AnimationsFolder);
        }
    }
}