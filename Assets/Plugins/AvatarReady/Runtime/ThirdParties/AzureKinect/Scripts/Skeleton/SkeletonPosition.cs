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
using System.Collections.Generic;
using UnityEngine;

namespace kinect
{
    /* SkeletonPosition class represents holder for joint positions in one frame. */
    [System.Serializable]
    public class SkeletonPosition
    {
        public SkeletonPosition(
            Body body,
            List<JointId> jointsMapper,
            Vector3 referentCameraPosition)
        {
            foreach (var entry in jointsMapper)
            {
                var point = body.JointPositions3D[(int)entry];
                Vector3 newDelta = new Vector3(point.X, -point.Y, point.Z);
                currentJointPositions[entry] = referentCameraPosition + newDelta;
            }
        }

        public SkeletonPosition()
        {
        }

        public float Timestamp { get; set; }

        public Dictionary<JointId, Vector3> currentJointPositions { get; set; } = new Dictionary<JointId, Vector3>();

#region overriden operators

        // Add two skeleton positions to create new.
        public static SkeletonPosition operator +(SkeletonPosition b, SkeletonPosition c)
        {
            SkeletonPosition a = new SkeletonPosition();
            foreach (var key in b.currentJointPositions.Keys)
            {
                a.currentJointPositions.Add(key, b.currentJointPositions[key] + c.currentJointPositions[key]);
            }
            return a;
        }

        // Subtract two skeleton positions to create new.
        public static SkeletonPosition operator -(SkeletonPosition b, SkeletonPosition c)
        {
            SkeletonPosition a = new SkeletonPosition();
            foreach (var key in b.currentJointPositions.Keys)
            {
                a.currentJointPositions.Add(key, b.currentJointPositions[key] - c.currentJointPositions[key]);
            }
            return a;
        }

        // Divide operator, used to find average value for given sequence.
        public static SkeletonPosition operator /(SkeletonPosition lhs, float rhs)
        {
            SkeletonPosition a = new SkeletonPosition();
            foreach (var key in lhs.currentJointPositions.Keys)
            {
                a.currentJointPositions.Add(key, lhs.currentJointPositions[key] / rhs);
            }
            return a;
        }

#endregion
    }
}
#endif