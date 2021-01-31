using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class SkeletalTrackingProvider : BackgroundDataProvider
{
    bool readFirstFrame = false;
    TimeSpan initialTimestamp;

    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter { get; set; } = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    public Stream RawDataLoggingFile = null;

    ColorCameraView colorCameraView;

    FPS frameRate;
    DepthMode depthMode;
    WiredSyncMode wiredSyncMode;

    UserMessages userMessages;

    public SkeletalTrackingProvider(ColorCameraView colorCameraView, FPS frameRate, DepthMode depthMode, WiredSyncMode wiredSyncMode, UserMessages userMessages)
    {
        this.colorCameraView = colorCameraView;

        this.frameRate = frameRate;
        this.depthMode = depthMode;
        this.wiredSyncMode = wiredSyncMode;

        this.userMessages = userMessages;
    }
    
    protected override void RunBackgroundThreadAsync(int id)
    {
        try
        {
            UnityEngine.Debug.Log("Starting body tracker background thread.");

            // Buffer allocations.
            BackgroundData currentFrameData = new BackgroundData();
            // Open device.
            using (Device device = Device.Open(id))
            {
                device.StartCameras(new DeviceConfiguration()
                {
                    CameraFPS = frameRate,
                    ColorResolution = ColorResolution.R720p,
                    DepthMode = depthMode,
                    WiredSyncMode = wiredSyncMode,
                    ColorFormat = ImageFormat.ColorBGRA32,
                });

                UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum );
                userMessages.queueMessage("Azure Kinect device opened.");

                var deviceCalibration = device.GetCalibration();

                using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
                {
                    UnityEngine.Debug.Log("Body tracker created.");
                    userMessages.queueMessage("Body tracker created.");
                    while (m_runBackgroundThread)
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
                                UnityEngine.Debug.Log("Pop result from tracker timeout!");
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

                                // Get color image and update color camera view UI
                                Image colorImage = bodyFrameCapture.Color;
                                colorCameraView.setImage(colorImage);

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
                                    byte b = (byte)(depthFrame[it] / (ConfigLoader.Instance.Configs.SkeletalTracking.MaximumDisplayedDepthInMillimeters) * 255);
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
            UnityEngine.Debug.LogError(e.Message);
            userMessages.queueMessage("Getting Azure Kinect data failed.");
        }
    }
}
