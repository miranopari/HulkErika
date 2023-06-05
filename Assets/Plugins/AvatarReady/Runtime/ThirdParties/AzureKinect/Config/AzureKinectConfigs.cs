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

namespace kinect
{
    [System.Serializable]
    public class InitialAndEndStability
    {
        public float StableTimeInMs = 200.0f;
        public float InitialAllowedMovementDeviationInMeters = 0.05f;
    }

    [System.Serializable]
    public class MovementDisplacement
    {
        public float HorizontalThresholdInMeters = 0.15f;
        public float VerticalThresholdInMeters = 0.05f;
    }

    [System.Serializable]
    public class HandsDisplacement
    {
        public int FrameCountThreshold = 5;
        public float DistanceThresholdInMeters = 0.3f;
        public float AllowedStandardDeviationInMeters = 0.03f;
    }

    [System.Serializable]
    public class AngleDisplacement
    {
        public float KneeAngleThresholdInDeg = 150;
        public float KneeAngleAllowedDeviation = 30;
        public float TorsoAngleThresholdInDeg = 170;
        public float TorsongleAllowedDeviation = 30;
    }

    [System.Serializable]
    public class Height
    {
        public float MaximalPossibleValueInMeters = 0.8f;
    }

    [System.Serializable]
    public class JumpValidity
    {
        public InitialAndEndStability InitialAndEndStability = new InitialAndEndStability();
        public MovementDisplacement MovementDisplacement = new MovementDisplacement();
        public HandsDisplacement HandsDisplacement = new HandsDisplacement();
        public AngleDisplacement AngleDisplacement = new AngleDisplacement();
        public Height Height = new Height();
    }

    [System.Serializable]
    public class BodyFiltering
    {
        public float DistanceThresholdInMeters = 1;
        public float CircleCenterXInMeters = 0;
        public float CircleCenterZIntMeters = 2.5f;
        public bool RejectPiruetteInRagDollVisualization = true;
        public bool ShowFlippedPiruetteInRagDoll = true;
    }

    [System.Serializable]
    public class SkeletalTracking
    {
        public float MaximumDisplayedDepthInMillimeters = 5000;
    }

    [System.Serializable]
    public class DebugOpt
    {
        // 0 - Show none, 1 - Show errors only, 2 - Show warnings and errors, 3 - Show all
        public int LogLevel = 1;
        public int MaxMessages = 10;
    }

    // Physical parameters of demo setup.
    [System.Serializable]
    public class DemoSceneConfig
    {
        // Scene setup, distance of camera from marked area.
        public float DistanceFromCamera = 2.6f;

        // Scene setup, distance camera from floor.
        public float CameraHeight = 1.0f;

        // Length of square which is monitored.
        public float MonitoringSquareWidth = 1.4f;
        public float MonitoringSquareHeight = 1.4f;

        public float ShoesSize = 0.2f;
        public bool DrawMonitoringArea = false;
        public int NumberSmoothingFrames = 5;
    }

    [System.Serializable]
    public class Plotting
    {
        public bool IsMovingAverageOn = true;
        public int MovingAverageWindowWidth = 5;
    }

    [System.Serializable]
    public class RawDataCollection
    {
        // Logic for outputing and reading from raw data:
        //
        // if (ReadFromFile)
        //   read_raw_data_from(FileName)
        // else
        //   read_raw_data_from_k4a()
        //   if (Logging)
        //     log_raw_data_to(LoggingFileNamePrefix + Timestamp)
        // endif
        public bool Logging = false;
        public string LoggingFileNamePrefix = "log";
        public bool ReadFromFile = false;
        public string FileName = "";
    }

    [System.Serializable]
    public class AzureKinectConfigs
    {
        public int CenterOfMassJointId = (int)JointId.Pelvis;

        public JumpValidity JumpValidity = new JumpValidity();
        public BodyFiltering BodyFiltering = new BodyFiltering();
        public SkeletalTracking SkeletalTracking = new SkeletalTracking();
        public DemoSceneConfig DemoSceneConfig = new DemoSceneConfig();
        public DebugOpt DebugOpt = new DebugOpt();
        public Plotting Plotting = new Plotting();
        public RawDataCollection RawDataCollection = new RawDataCollection();
        public bool LoggingToZion = false;
        public bool DrawOnlyMainSkeleton = true;
        public bool IsFpsDisplayActive = false;
    }
}
#endif