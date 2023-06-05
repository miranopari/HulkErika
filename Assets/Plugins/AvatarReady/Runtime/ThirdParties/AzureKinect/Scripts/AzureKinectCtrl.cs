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
using UnityEngine;

namespace kinect
{

    public class AzureKinectCtrl : MonoBehaviour
    {
        // Handler for SkeletalTracking thread.
        public GameObject tracker;
        private SkeletalTrackingProvider m_skeletalTrackingProvider;
        public BackgroundData m_lastFrameData = new BackgroundData();

        void Start()
        {
            //tracker ids needed for when there are two trackers
            const int TRACKER_ID = 0;
            m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
        }

        void Update()
        {
            if (m_skeletalTrackingProvider.IsRunning)
            {
                if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
                {
                    if (m_lastFrameData.NumOfBodies != 0)
                    {
                        tracker.GetComponent<AzureKinectTrackerHandler>().updateTracker(m_lastFrameData);
                    }
                }
            }
        }

        void OnApplicationQuit()
        {
            if (m_skeletalTrackingProvider != null)
            {
                m_skeletalTrackingProvider.Dispose();
            }
        }
    }
}
#endif