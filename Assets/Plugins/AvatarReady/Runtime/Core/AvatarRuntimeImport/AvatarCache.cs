using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Inria.Avatar.AvatarReady
{
    public static class AvatarCache
    {
        private static Dictionary<string, string> avatarCache = new Dictionary<string, string>();

        public static string AvatarCacheFolder
        {
            // %USERPROFILE%/AvatarReadyAvatars, ie. 'C:/Users/<name>/AvatarReadyAvatars'
            get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AvatarReadyAvatars");
        }

        public static string CurrentRenderPipeline
        {
            get
            {
#if USING_HDRP
                return "hdrp";
#elif USING_URP
                return "urp";
#else
                return "builtin";
#endif
            }
        }

        /// <summary>
        /// Returns the Avatars in cache, ie. localy stored
        /// </summary>
        public static IEnumerable<string> Keys
        {
            get => avatarCache.Keys;
        }

        static AvatarCache()
        {
            if (!Directory.Exists(AvatarCacheFolder))
                Directory.CreateDirectory(AvatarCacheFolder);

            string[] avatarFiles = Directory.GetFiles(AvatarCacheFolder);

            // Search for assetbundle files in local folder that match the current render pipeline
            foreach (string avatarFile in avatarFiles)
            {
                if (avatarFile.EndsWith("." + CurrentRenderPipeline))
                {
                    avatarCache.Add(Path.GetFileNameWithoutExtension(avatarFile).ToLower(), avatarFile);
                }
            }
        }

        /// <summary>
        /// Check if the given avatar is in the cache, ie. localy stored
        /// </summary>
        public static bool Contains(string avatarName)
        {
            return avatarCache.ContainsKey(avatarName.ToLower());
        }

        /// <summary>
        /// Download the given avatar and put it in the cache
        /// </summary>
        public static IEnumerator Download(string avatarName)
        {
            if (Contains(avatarName))
            {
                Debug.LogWarning("Avatar already in cache, download ignored");
                yield break;
            }

            string url = $"https://avatar-gallery.irisa.fr/cc_avatars/{avatarName}/assetBundle/{avatarName.ToLower()}.{CurrentRenderPipeline}";

            Debug.Log("Downloading " + url);
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string path = $"{AvatarCacheFolder}/{avatarName.ToLower()}.{CurrentRenderPipeline}";

                    if (File.Exists(path))
                    {
                        Debug.LogError("File already exist at " + path);
                        yield break;
                    }

                    Debug.Log("Writing file at " + path);

                    // write file in folder
                    File.WriteAllBytes(path, request.downloadHandler.data);

                    // add entry in cache
                    avatarCache.Add(avatarName.ToLower(), path);
                }
                else
                {
                    Debug.LogError($"{request.result} : {request.error}");
                }
            }
        }

        /// <summary>
        /// Loads and returns the AssetBundle corresponding to the given avatar
        /// </summary>
        public static AssetBundle Get(string avatarName)
        {
            if (!Contains(avatarName))
                throw new KeyNotFoundException(avatarName);

            return AssetBundle.LoadFromFile(avatarCache[avatarName.ToLower()]);
        }
    }
}
