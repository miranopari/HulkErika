This tutorial is for Azure Kinect Sensor SDK version 1.4.1 and Azure Kinect Body Tracking SDK version 1.0.1

# Requirements:
###############
- Seventh Gen Intel (R) CoreTM i5 Processor (Quad Core 2.4 GHz or faster)
- 4 GB of memory
- NVIDIA GEFORCE GTX 1070 or better
- A dedicated USB3 port
- Visual Studio

The recommended minimum configuration assumes K4A_DEPTH_MODE_NFOV_UNBINNED depth mode at 30fps tracking 5 people. 
Lower end or older CPUs and NVIDIA GPUs may also work depending on your use-case.

###########################################
# 1. Install the Azure Kinect software on your PC
###########################################
You need to install the "Azure Kinect Sensor SDK" and the "Azure Kinect Body Tracking SDK". You can follow the official documentation to do so:
- https://docs.microsoft.com/en-us/azure/kinect-dk/sensor-sdk-download
- https://docs.microsoft.com/en-us/azure/kinect-dk/body-sdk-download

Please install the SDKs at the default location.
For further information, please refer to the Azure Kinect docs: https://docs.microsoft.com/en-us/azure/kinect-dk/

###########################################
# 2. Install Azure Kinect packages in Unity
###########################################
- Open the Visual Studio solution of the AvatarReady project. You can click on any script file in the Unity project to do so.
- In Visual Studio, Tools > NuGet Package Manager > Package Manager Console
- In the console, type the following command line:
	Install-Package Microsoft.Azure.Kinect.BodyTracking -Version 1.0.1

Assets/Packages should now contain the following packages:
	Microsoft.Azure.Kinect.BodyTracking.1.0.1
	Microsoft.Azure.Kinect.BodyTracking.Dependencies.0.9.1
	Microsoft.Azure.Kinect.BodyTracking.Dependencies.cuDNN.0.9.1
	Microsoft.Azure.Kinect.Sensor.1.3.0
	System.Buffers.4.4.0
	System.Memory.4.5.3
	System.Numerics.Vectors.4.5.0
	System.Reflection.Emit.Lightweight.4.6.0
	System.Runtime.CompilerServices.Unsafe.4.5.2

- Open the root of your Unity project in a file explorer
- Run the MoveLibraryFiles.bat file. It will copy the required DLLs into the Unity project.

The Assets/Plugins folder should now contain the following files (and their .meta):
	cublas64_11.dll
	cublasLt64_11.dll
	cudart64_110.dll
	depthengine_2_0.dll
	k4a.dll
	k4abt.dll
	k4arecord.dll
	Microsoft.Azure.Kinect.BodyTracking.deps.json
	Microsoft.Azure.Kinect.BodyTracking.dll
	Microsoft.Azure.Kinect.BodyTracking.pdb
	Microsoft.Azure.Kinect.BodyTracking.xml
	Microsoft.Azure.Kinect.Sensor.deps.json
	Microsoft.Azure.Kinect.Sensor.dll
	Microsoft.Azure.Kinect.Sensor.pdb
	Microsoft.Azure.Kinect.Sensor.xml
	onnxruntime.dll
	System.Buffers.dll
	System.Memory.dll
	System.Reflection.Emit.Lightweight.dll
	System.Runtime.CompilerServices.Unsafe.dll
	vcomp140.dll

Next to the Assets folder, there should also be:
	cublas64_11.dll
	cublasLt64_11.dll
	cudart64_110.dll
	cudnn_cnn_infer64_8.dll
	cudnn_ops_infer64_8.dll
	cudnn64_8.dll
	cufft64_10.dll
	dnn_model_2_0_op11.onnx
	onnxruntime.dll

If some of them are missing, please check for path errors in the MoveLibraryFiles.bat.
You can also copy the files manually from the Azure Kinect SDK installation folders.
They should be located at:
- C:\Program Files\Azure Kinect Body Tracking SDK
- C:\Program Files\Azure Kinect SDK v1.4.1
Missing files will result in tracking failure. 

###########################################
# 3. Run the AzureKinect test scene
###########################################
- Open the AzureKinect scene
- Press Play

You should see a stick figure copying your movements.

# Troubleshooting:
##################
- If no light is shining, unplug and replug the device and press Play again.
- If you get a K4A_RESULT_FAILED error or something alike in the Unity console, some SDK library files might have not been copied properly.
https://github.com/microsoft/Azure-Kinect-Sensor-SDK/issues/1600

You can open the Azure Kinect Viewer to check that the device is indeed detected by your operating system and functional.
- If no device is detected in the Viewer, please check the Azure Kinect known issues and troubleshooting in Microsoft Docs:
https://docs.microsoft.com/en-us/azure/kinect-dk/troubleshooting