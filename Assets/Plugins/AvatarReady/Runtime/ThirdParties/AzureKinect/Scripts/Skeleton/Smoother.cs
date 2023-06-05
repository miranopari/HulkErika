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
using System.Collections.Generic;

namespace kinect
{
    public class Smoother
    {
        // Max allowed size of history list.
        private int maxSize = 100;

        // In case smoother has enough frames in history to perform smooth action.
        private bool hasEnoughForSmoothing = false;

        // Number of the latest frames used to smooth current position; default 5.
        public int NumberSmoothingFrames { get; set; } = 5;

        // Holds received data about moves.
        private List<SkeletonPosition> rawData = new List<SkeletonPosition>();

        // Holds received which are smoothened a little bit.
        private List<SkeletonPosition> smoothenedData = new List<SkeletonPosition>();

        // Process skeleton position and sends back smoothened or raw based on passed parameter.
        public SkeletonPosition ReceiveNewSensorData(SkeletonPosition newData, bool smoothing)
        {
            // In case list is too big.
            if (rawData.Count > maxSize)
            {
                Resize();
            }

            // Add new frame data to raw data used for smoothing.
            rawData.Add(newData);

            // In case value for smoothing is invalid just return original raw frame.
            if (NumberSmoothingFrames <= 1)
            {
                return rawData[rawData.Count - 1];
            }

            // Mark that smoother has enough frames for smoothing.
            if (rawData.Count > NumberSmoothingFrames)
            {
                hasEnoughForSmoothing = true;
            }

            if (smoothenedData.Count == 0)
            {
                smoothenedData.Add(newData);
            }
            else
            {
                SkeletonPosition temp = smoothenedData[smoothenedData.Count - 1] + newData;
                if (hasEnoughForSmoothing)
                {
                    temp = temp - rawData[rawData.Count - NumberSmoothingFrames];
                }
                smoothenedData.Add(temp);
            }

            // Smoothened timestamp has the same timestamp as the latest received.
            smoothenedData[smoothenedData.Count - 1].Timestamp = rawData[rawData.Count - 1].Timestamp;

            return smoothing && hasEnoughForSmoothing
                ? smoothenedData[smoothenedData.Count - 1] / (float)NumberSmoothingFrames
                : rawData[rawData.Count - 1];
        }

        // Deletes old position data from list which do not have more impact on smoothing algorithm.
        public void Resize()
        {
            if (rawData.Count > NumberSmoothingFrames)
            {
                rawData.RemoveRange(0, rawData.Count - NumberSmoothingFrames);
            }
            if (smoothenedData.Count > NumberSmoothingFrames)
            {
                smoothenedData.RemoveRange(0, smoothenedData.Count - NumberSmoothingFrames);
            }
        }
    }
}
#endif