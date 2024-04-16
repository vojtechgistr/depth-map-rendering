using Intel.RealSense;
using System.Collections.ObjectModel;


class Program
{
    static void Main()
    {
        using Context context = new Context();

        DeviceList devices = context.QueryDevices();
        if (devices.Count == 0)
        {
            Console.WriteLine("No device found.");
            return;
        }



        Device selectedDevice = devices[0];
        ReadOnlyCollection<Sensor> sensors = selectedDevice.QuerySensors();
        Sensor depthSensor = sensors.Where(s => s.Info[CameraInfo.Name] == "Stereo Module")
                                    .First();

        Console.WriteLine("\nUsing device 0, an {0}", selectedDevice.Info[CameraInfo.Name]);
        Console.WriteLine("    Serial number: {0}", selectedDevice.Info[CameraInfo.SerialNumber]);
        Console.WriteLine("    Firmware version: {0}", selectedDevice.Info[CameraInfo.FirmwareVersion]);

        VideoStreamProfile depthSensorProfile = depthSensor.StreamProfiles
                            .Where(p => p.Stream == Intel.RealSense.Stream.Depth)
                            .OrderBy(p => p.Framerate)
                            .Select(p => p.As<VideoStreamProfile>())
                            .First();


        if (depthSensorProfile == null)
        {
            Console.WriteLine("No default profile found for sensor.");
            return;
        }

        depthSensor.Open(depthSensorProfile);
        
        int maxTreshold = (int)(1f / depthSensor.DepthScale);
        ushort[] depth = new ushort[depthSensorProfile.Width * depthSensorProfile.Height];
        char[] buffer = new char[(depthSensorProfile.Width / 10 + 1) * (depthSensorProfile.Height / 20)];
        int[] thresholdedFrame = new int[depthSensorProfile.Width / 10];
        depthSensor.Start(frame =>
        {

            Console.WriteLine("\nUsing device 0, an {0}", selectedDevice.Info[CameraInfo.Name]);
            Console.WriteLine("    Serial number: {0}", selectedDevice.Info[CameraInfo.SerialNumber]);
            Console.WriteLine("    Firmware version: {0}", selectedDevice.Info[CameraInfo.FirmwareVersion]);

            using VideoFrame videoFrame = frame.As<VideoFrame>();
            videoFrame.CopyTo(depth);

            int minWeight = depth.Where(depth => depth > 0).Min();
            int maxWeight = depth.Where(depth => depth < maxTreshold).Max();
            int bufferIndex = 0;
            for (int y = 0; y < depthSensorProfile.Height; y++)
            {
                if (y % 20 != 19) continue;

                for (int x = 0; x < depthSensorProfile.Width; x++)
                {
                    ushort depthValue = depth[x + y * depthSensorProfile.Width];
                    if (depthValue > 0 && depthValue < maxTreshold)
                    {
                        int position = x / 10;
                        thresholdedFrame[position] = depthValue;
                    }
                }

                for (int x = 0; x < thresholdedFrame.Length; x++)
                {
                    int rawDepthValue = thresholdedFrame[x];

                    string depthChars = " COFGEQfcbao!$-;:^'.";
                    int depthCharPointer = 0;

                    if (rawDepthValue > 0)
                    {
                        float weightSpan = maxWeight - minWeight;
                        float valuePositionInSpan = (rawDepthValue - minWeight) / weightSpan;
                        float charPointer = valuePositionInSpan * (depthChars.Length - 1);
                        depthCharPointer = (int)charPointer;
                    }

                    buffer[bufferIndex++] = depthChars[depthCharPointer];
                    thresholdedFrame[x] = 0;
                }

                buffer[bufferIndex++] = '\n';
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine();
            Console.Write(buffer);
        });

        AutoResetEvent stop = new AutoResetEvent(false);
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            stop.Set();
        };

        stop.WaitOne();
        depthSensor.Stop();
        depthSensor.Close();
    }
}
