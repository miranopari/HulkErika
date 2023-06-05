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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace kinect
{
    /* Class for tracking data management and storing */
    public abstract class BackgroundDataProvider : IDisposable
    {
        private BackgroundData m_frameBackgroundData = new BackgroundData();
        private bool m_latest = false;
        object m_lockObj = new object();
        public bool IsRunning { get; set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _token;

        public BackgroundDataProvider(int id)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting += OnEditorClose;
#endif
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            Task.Run(() => RunBackgroundThreadAsync(id, _token));
        }

        private void OnEditorClose()
        {
            Dispose();
        }

        protected abstract void RunBackgroundThreadAsync(int id, CancellationToken token);

        public void SetCurrentFrameData(ref BackgroundData currentFrameData)
        {
            lock (m_lockObj)
            {
                var temp = currentFrameData;
                currentFrameData = m_frameBackgroundData;
                m_frameBackgroundData = temp;
                m_latest = true;
            }
        }

        public bool GetCurrentFrameData(ref BackgroundData dataBuffer)
        {
            lock (m_lockObj)
            {
                var temp = dataBuffer;
                dataBuffer = m_frameBackgroundData;
                m_frameBackgroundData = temp;
                bool result = m_latest;
                m_latest = false;
                return result;
            }
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting -= OnEditorClose;
#endif
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
#endif