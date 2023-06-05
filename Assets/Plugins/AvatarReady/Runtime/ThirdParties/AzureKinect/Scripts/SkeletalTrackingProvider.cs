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
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace kinect
{
    /* This class manages the opening of the Azure Kinect and its frame processing.
     * It extracts the tracking data at each frame and passes it to BackgroundDataProvider */
    public class SkeletalTrackingProvider : BackgroundDataProvider
    {
        bool readFirstFrame = false;
        TimeSpan initialTimestamp;

        public SkeletalTrackingProvider(int id): base(id)
        {
            //Debug.Log("in the skeleton provider constructor");
        }

        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter { get; set; } = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        public Stream RawDataLoggingFile = null;

        protected override void RunBackgroundThreadAsync(int id, CancellationToken token)
        {
            //Starting body tracker background thread.
            try
            {
                // Buffer allocations.
                BackgroundData currentFrameData = new BackgroundData();
                // Open device.
                using (Device device = Device.Open(id))
                {
                    device.StartCameras(new DeviceConfiguration()
                    {
                        CameraFPS = FPS.FPS30,
                        ColorResolution = ColorResolution.Off,
                        DepthMode = DepthMode.NFOV_Unbinned,
                        WiredSyncMode = WiredSyncMode.Standalone,
                    });

                    UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum );

                    var deviceCalibration = device.GetCalibration();

                    using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
                    {
                        // Body tracker is created
                        while (!token.IsCancellationRequested)
                        {
                            using (Capture sensorCapture = device.GetCapture())
                            {
                                // Queue latest frame from the sensor.
                                tracker.EnqueueCapture(sensorCapture);
                            }

                            // Try getting latest tracker frame.
                            using (Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                            {
                                if (frame == null)
                                {
                                    UnityEngine.Debug.Log("Kinect tracker timeout");
                                }
                                else
                                {
                                    IsRunning = true;
                                    // Get number of bodies in the current frame.
                                    currentFrameData.NumOfBodies = frame.NumberOfBodies;

                                    // Copy bodies.
                                    for (uint i = 0; i < currentFrameData.NumOfBodies; i++)
                                    {
                                        currentFrameData.Bodies[i].CopyFromBodyTrackingSdk(frame.GetBody(i), deviceCalibration);
                                    }

                                    // Store depth image.
                                    Capture bodyFrameCapture = frame.Capture;
                                    Image depthImage = bodyFrameCapture.Depth;
                                    if (!readFirstFrame)
                                    {
                                        readFirstFrame = true;
                                        initialTimestamp = depthImage.DeviceTimestamp;
                                    }
                                    currentFrameData.TimestampInMs = (float)(depthImage.DeviceTimestamp - initialTimestamp).TotalMilliseconds;
                                    currentFrameData.DepthImageWidth = depthImage.WidthPixels;
                                    currentFrameData.DepthImageHeight = depthImage.HeightPixels;

                                    // Read image data from the SDK.
                                    var depthFrame = MemoryMarshal.Cast<byte, ushort>(depthImage.Memory.Span);

                                    // Repack data and store image data.
                                    int byteCounter = 0;
                                    currentFrameData.DepthImageSize = currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight * 3;

                                    for (int it = currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight - 1; it > 0; it--)
                                    {
                                        byte b = (byte)(depthFrame[it] / (AzureKinectConfigLoader.Instance.Configs.SkeletalTracking.MaximumDisplayedDepthInMillimeters) * 255);
                                        currentFrameData.DepthImage[byteCounter++] = b;
                                        currentFrameData.DepthImage[byteCounter++] = b;
                                        currentFrameData.DepthImage[byteCounter++] = b;
                                    }

                                    if (RawDataLoggingFile != null && RawDataLoggingFile.CanWrite)
                                    {
                                        binaryFormatter.Serialize(RawDataLoggingFile, currentFrameData);
                                    }

                                    // Update data variable that is being read in the UI thread.
                                    SetCurrentFrameData(ref currentFrameData);
                                }

                            }
                        }
                        tracker.Dispose();
                    }
                    device.Dispose();
                }
                if (RawDataLoggingFile != null)
                {
                    RawDataLoggingFile.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log($"catching exception for background thread {e.Message}");
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
#endif