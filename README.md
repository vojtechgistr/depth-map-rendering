# Depth Map Rendering
A small console app that renders real-time depth data from the Intel Realsense camera.

*Tested on Intel Realsense D435*

## How To Run
- Download the `Program.cs` and `Intel.RealSensed.dll` library
- Create a new C# project, place the content of `Program.cs` to the main file
- Place `Intel.RealSensed.dll` to the bin folder, where all dependencies are (for exmaple: `bin/Debug/net8.0/`)
- Run it in **Debug** mode. If you want to do it in a **Release** mode, you will have to compile the Intel Realsense SDK for C# bindings. See more [here](https://github.com/IntelRealSense/librealsense).

![image](https://github.com/vojtechgistr/depth-map-rendering/assets/56306485/db43ba79-12e9-465d-915d-0a7895a7ddab)

![image](https://github.com/vojtechgistr/depth-map-rendering/assets/56306485/88a558b5-595e-4fb9-8cf2-1fb4bfd2957e)

## Dependencies Libraries and Products
#### [librealsense](https://github.com/IntelRealSense/librealsense)
> **License:** Apache License 2.0
>
> **Author:** IntelRealSense
>
> **Principal Use:** Communication with Intel Realsense D435 camera
