﻿
/*
 * This sample program is ported by C# from examples/tutorial_api_cpp/3_keypoints_from_image_configurable.cpp.
*/

using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using OpenPoseDotNet;

namespace KeyPointsFromImageConfigurable
{

    internal class Program
    {

        #region Fields

        private static string ImagePath;

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = nameof(KeyPointsFromImageConfigurable)
            };

            app.HelpOption("-h|--help");

            var disableMultiThreadArgument = app.Argument("disableMultiThread", "Disable MultiThread");
            var inputImageOption = app.Option("-i|--image", "Input image", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (disableMultiThreadArgument.Value != null)
                    Flags.DisableMultiThread = true;

                var path = inputImageOption.Value();
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    Console.WriteLine($"Argument 'image' is invalid or not found.");
                    app.ShowHelp();
                    return -1;
                }

                ImagePath = path;

                TutorialApiCpp3();

                return 0;
            });

            app.Execute(args);
        }

        #region Helpers

        private static void Display(StdSharedPtr<StdVector<Datum>> datumsPtr)
        {
            // User's displaying/saving/other processing here
            // datum.cvOutputData: rendered frame with pose or heatmaps
            // datum.poseKeypoints: Array<float> with the estimated pose
            if (datumsPtr != null && datumsPtr.TryGet(out var data) && !data.Empty)
            {
                // Display image
                var temp = data.ToArray();
                Cv.ImShow("User worker GUI", temp[0].CvOutputData);
                Cv.WaitKey();
            }
            else
            {
                OpenPose.Log("Nullptr or empty datumsPtr found.", Priority.High);
            }
        }

        private static void PrintKeypoints(StdSharedPtr<StdVector<Datum>> datumsPtr)
        {
            // Example: How to use the pose keypoints
            if (datumsPtr != null && datumsPtr.TryGet(out var data) && !data.Empty)
            {
                // Alternative 1
                var temp = data.ToArray();
                OpenPose.Log($"Body keypoints: {temp[0].PoseKeyPoints}");
                OpenPose.Log($"Face keypoints: {temp[0].FaceKeyPoints}");
                OpenPose.Log($"Left hand keypoints: {temp[0].HandKeyPoints[0]}");
                OpenPose.Log($"Right hand keypoints: {temp[0].HandKeyPoints[1]}");
            }
            else
            {
                OpenPose.Log("Nullptr or empty datumsPtr found.", Priority.High);
            }
        }

        private static int TutorialApiCpp3()
        {
            try
            {
                OpenPose.Log("Starting OpenPose demo...", Priority.High);

                // logging_level
                OpenPose.Check(0 <= Flags.LoggingLevel && Flags.LoggingLevel <= 255, "Wrong logging_level value.");
                ConfigureLog.PriorityThreshold = (Priority)Flags.LoggingLevel;
                Profiler.SetDefaultX((ulong)Flags.ProfileSpeed);

                // Applying user defined configuration - GFlags to program variables
                // outputSize
                var outputSize = OpenPose.FlagsToPoint(Flags.OutputResolution, "-1x-1");
                // netInputSize
                var netInputSize = OpenPose.FlagsToPoint(Flags.NetResolution, "-1x368");
                // faceNetInputSize
                var faceNetInputSize = OpenPose.FlagsToPoint(Flags.FaceNetResolution, "368x368 (multiples of 16)");
                // handNetInputSize
                var handNetInputSize = OpenPose.FlagsToPoint(Flags.HandNetResolution, "368x368 (multiples of 16)");
                // poseModel
                var poseModel = OpenPose.FlagsToPoseModel(Flags.ModelPose);
                // JSON saving
                if (!string.IsNullOrEmpty(Flags.WriteKeyPoint))
                    OpenPose.Log("Flag `write_keypoint` is deprecated and will eventually be removed. Please, use `write_json` instead.", Priority.Max);
                // keypointScale
                var keypointScale = OpenPose.FlagsToScaleMode(Flags.KeyPointScale);
                // heatmaps to add
                var heatMapTypes = OpenPose.FlagsToHeatMaps(Flags.HeatmapsAddParts, Flags.HeatmapsAddBackground, Flags.HeatmapsAddPAFs);
                var heatMapScale = OpenPose.FlagsToHeatMapScaleMode(Flags.HeatmapsScale);
                // >1 camera view?
                var multipleView = (Flags.Enable3D || Flags.Views3D > 1);
                // Enabling Google Logging
                const bool enableGoogleLogging = true;

                // Configuring OpenPose
                OpenPose.Log("Configuring OpenPose...", Priority.High);
                using (var opWrapper = new Wrapper<Datum>(ThreadManagerMode.Asynchronous))
                {
                    // Pose configuration (use WrapperStructPose{} for default and recommended configuration)
                    using (var pose = new WrapperStructPose(!Flags.BodyDisabled,
                                                            netInputSize,
                                                            outputSize,
                                                            keypointScale,
                                                            Flags.NumGpu,
                                                            Flags.NumGpuStart,
                                                            Flags.ScaleNumber,
                                                            (float)Flags.ScaleGap, 
                                                            OpenPose.FlagsToRenderMode(Flags.RenderPose, multipleView),
                                                            poseModel, 
                                                            !Flags.DisableBlending, 
                                                            (float)Flags.AlphaPose,
                                                            (float)Flags.AlphaHeatmap,
                                                            Flags.PartToShow, 
                                                            Flags.ModelFolder, 
                                                            heatMapTypes, 
                                                            heatMapScale, 
                                                            Flags.PartCandidates,
                                                            (float)Flags.RenderThreshold,
                                                            Flags.NumberPeopleMax, 
                                                            Flags.MaximizePositives, 
                                                            Flags.FpsMax,
                                                            enableGoogleLogging))
                    // Face configuration (use op::WrapperStructFace{} to disable it)
                    using (var face = new WrapperStructFace(Flags.Face, 
                                                            faceNetInputSize,
                                                            OpenPose.FlagsToRenderMode(Flags.FaceRender, multipleView, Flags.RenderPose),
                                                            (float)Flags.FaceAlphaPose,
                                                            (float)Flags.FaceAlphaHeatmap,
                                                            (float)Flags.FaceRenderThreshold))
                    // Hand configuration (use op::WrapperStructHand{} to disable it)
                    using (var hand = new WrapperStructHand(Flags.Hand, 
                                                            handNetInputSize, 
                                                            Flags.HandScaleNumber,
                                                            (float)Flags.HandScaleRange, Flags.HandTracking,
                                                            OpenPose.FlagsToRenderMode(Flags.HandRender, multipleView, Flags.RenderPose),
                                                            (float)Flags.HandAlphaPose,
                                                            (float)Flags.HandAlphaHeatmap,
                                                            (float)Flags.HandRenderThreshold))
                    // Extra functionality configuration (use op::WrapperStructExtra{} to disable it)
                    using (var extra = new WrapperStructExtra(Flags.Enable3D,
                                                              Flags.MinViews3D,
                                                              Flags.Identification,
                                                              Flags.Tracking,
                                                              Flags.IkThreads))
                    // Output (comment or use default argument to disable any output)
                    using (var output = new WrapperStructOutput(Flags.CliVerbose,
                                                                Flags.WriteKeyPoint,
                                                                OpenPose.StringToDataFormat(Flags.WriteKeyPointFormat),
                                                                Flags.WriteJson,
                                                                Flags.WriteCocoJson,
                                                                Flags.WriteCocoFootJson,
                                                                Flags.WriteCocoJsonVariant,
                                                                Flags.WriteImages,
                                                                Flags.WriteImagesFormat,
                                                                Flags.WriteVideo,
                                                                Flags.WriteVideoFps,
                                                                Flags.WriteHeatmaps,
                                                                Flags.WriteHeatmapsFormat,
                                                                Flags.WriteVideoAdam,
                                                                Flags.WriteBvh, 
                                                                Flags.UdpHost,
                                                                Flags.UdpPort))
                    {
                        opWrapper.Configure(pose);
                        opWrapper.Configure(face);
                        opWrapper.Configure(hand);
                        opWrapper.Configure(extra);
                        opWrapper.Configure(output);

                        // No GUI. Equivalent to: opWrapper.configure(op::WrapperStructGui{});
                        // Set to single-thread (for sequential processing and/or debugging and/or reducing latency)
                        if (Flags.DisableMultiThread)
                            opWrapper.DisableMultiThreading();

                        // Starting OpenPose
                        OpenPose.Log("Starting thread(s)...", Priority.High);
                        opWrapper.Start();

                        // Process and display image
                        using (var imageToProcess = Cv.ImRead(ImagePath))
                        using (var datumProcessed = opWrapper.EmplaceAndPop(imageToProcess))
                        {
                            if (datumProcessed != null)
                            {
                                PrintKeypoints(datumProcessed);
                                Display(datumProcessed);
                            }
                            else
                            {
                                OpenPose.Log("Image could not be processed.", Priority.High);
                            }
                        }
                    }
                }

                // Return successful message
                OpenPose.Log("Stopping OpenPose...", Priority.High);

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #endregion

    }

}