/*  MIT License
    Copyright (c) Microsoft Corporation. All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE */

#if AZUREKINECT
using System.IO;
using UnityEngine;

namespace kinect
{
    /* This class loads configuration paramaters (e.g. camera height, debug options, max values, etc.) 
     * from a JSON config file ("config.json) placed in Assets/StreamingAssets/AzureKinect. Open 
     * "AzureKinectConfig.js" for more details. */
    public class AzureKinectConfigLoader : MonoBehaviour
    {
        public static AzureKinectConfigLoader Instance { get; private set; }

        // Name of scene config file. Edit this if applying another config file.
        private const string gameDataFileName = "AzureKinect/config.json";
        public AzureKinectConfigs Configs { get; private set; } = new AzureKinectConfigs();

        // Init the config loader instance
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            LoadSceneSetup();
        }

        // Load the config file from streaming assets path
        private void LoadSceneSetup()
        {
            // Path.Combine combines strings into a file path.
            // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build.
            string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
            if (File.Exists(filePath))
            {
                // Read the json from the file into a string.
                string dataAsJson = File.ReadAllText(filePath);

                // Pass the json to JsonUtility, and tell it to create a Configs object from it.
                Configs = JsonUtility.FromJson<AzureKinectConfigs>(dataAsJson);

                UnityEngine.Debug.Log("Successfully loaded config file.");
            }
            else
            {
                Debug.LogError("Cannot load game data (config file).");
            }
        }
    }
}
#endif