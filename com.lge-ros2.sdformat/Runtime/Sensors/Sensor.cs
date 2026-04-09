// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Ported from libsdformat (C++) - Sensor.hh + sensor sub-types

#nullable enable

using System;
using System.Collections.Generic;
using SDFormat.Math;

namespace SDFormat
{
    // ---- Sensor sub-type classes ----

    /// <summary>Altimeter sensor properties.</summary>
    public class AltimeterSensor : SdfElement
    {
        public Noise VerticalPositionNoise { get; set; } = new();
        public Noise VerticalVelocityNoise { get; set; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var vp = sdf.FindElement("vertical_position");
            if (vp != null && vp.HasElement("noise"))
                errors.AddRange(VerticalPositionNoise.Load(vp.FindElement("noise")!));

            var vv = sdf.FindElement("vertical_velocity");
            if (vv != null && vv.HasElement("noise"))
                errors.AddRange(VerticalVelocityNoise.Load(vv.FindElement("noise")!));

            return errors;
        }
    }

    /// <summary>Air pressure sensor properties.</summary>
    public class AirPressureSensor : SdfElement
    {
        public double ReferenceAltitude { get; set; }
        public Noise PressureNoise { get; set; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var refAlt = sdf.FindElement("reference_altitude");
            if (refAlt?.Value != null) ReferenceAltitude = refAlt.Value.DoubleValue;

            var noise = sdf.FindElement("noise");
            if (noise != null)
                errors.AddRange(PressureNoise.Load(noise));

            return errors;
        }
    }

    /// <summary>Air speed sensor properties.</summary>
    public class AirSpeedSensor : SdfElement
    {
        public Noise PressureNoise { get; set; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var noise = sdf.FindElement("noise");
            if (noise != null)
                errors.AddRange(PressureNoise.Load(noise));

            return errors;
        }
    }

    /// <summary>Logical camera sensor properties.</summary>
    public class LogicalCameraSensor : SdfElement
    {
        public double Near { get; set; }
        public double Far { get; set; } = 1;
        public double AspectRatio { get; set; } = 1;
        public double HorizontalFov { get; set; } = 1;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var near = sdf.FindElement("near");
            if (near?.Value != null) Near = near.Value.DoubleValue;
            var far = sdf.FindElement("far");
            if (far?.Value != null) Far = far.Value.DoubleValue;
            var aspect = sdf.FindElement("aspect_ratio");
            if (aspect?.Value != null) AspectRatio = aspect.Value.DoubleValue;
            var hfov = sdf.FindElement("horizontal_fov");
            if (hfov?.Value != null) HorizontalFov = hfov.Value.DoubleValue;

            return errors;
        }
    }

    /// <summary>Sonar sensor properties.</summary>
    public class SonarSensor : SdfElement
    {
        public string Geometry { get; set; } = "cone";
        public double Min { get; set; }
        public double Max { get; set; } = 1;
        public double Radius { get; set; } = 0.5;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var geom = sdf.FindElement("geometry");
            if (geom?.Value != null) Geometry = geom.Value.GetAsString();
            var min = sdf.FindElement("min");
            if (min?.Value != null) Min = min.Value.DoubleValue;
            var max = sdf.FindElement("max");
            if (max?.Value != null) Max = max.Value.DoubleValue;
            var radius = sdf.FindElement("radius");
            if (radius?.Value != null) Radius = radius.Value.DoubleValue;

            return errors;
        }
    }

    /// <summary>Camera sensor properties.</summary>
    public class CameraSensor : SdfElement
    {
        public string Name { get; set; } = string.Empty;
        public double HorizontalFov { get; set; } = 1.047;
        public uint ImageWidth { get; set; } = 320;
        public uint ImageHeight { get; set; } = 240;
        public string ImageFormat { get; set; } = "R8G8B8";
        public double NearClip { get; set; } = 0.1;
        public double FarClip { get; set; } = 100.0;
        public bool SaveFrames { get; set; }
        public string SavePath { get; set; } = string.Empty;
        public double DepthNearClip { get; set; } = 0.1;
        public double DepthFarClip { get; set; } = 10.0;
        public Noise ImageNoise { get; set; } = new();
        public uint AntiAliasingValue { get; set; } = 4;
        public uint VisibilityMask { get; set; } = 0xFFFFFFFF;
        public bool HasTriggeredCamera { get; set; }
        public string TriggerTopic { get; set; } = string.Empty;
        public string CameraInfoTopic { get; set; } = string.Empty;
        public string OpticalFrameId { get; set; } = string.Empty;
        public string SegmentationType { get; set; } = "semantic";
        public string BoxType { get; set; } = "2d";

        // Distortion parameters
        public double DistortionK1 { get; set; }
        public double DistortionK2 { get; set; }
        public double DistortionK3 { get; set; }
        public double DistortionP1 { get; set; }
        public double DistortionP2 { get; set; }
        public Vector2d DistortionCenter { get; set; } = new(0.5, 0.5);

        // Lens parameters
        public string LensType { get; set; } = "stereographic";
        public bool LensScaleToHfov { get; set; } = true;
        public double LensCutoffAngle { get; set; } = 1.5707;
        public int LensEnvTextureSize { get; set; } = 256;
        public double LensIntrinsicsFx { get; set; } = 277;
        public double LensIntrinsicsFy { get; set; } = 277;
        public double LensIntrinsicsCx { get; set; } = 160;
        public double LensIntrinsicsCy { get; set; } = 120;
        public double LensIntrinsicsS { get; set; }

        // Lens custom_function parameters
        public double LensCustomC1 { get; set; } = 1;
        public double LensCustomC2 { get; set; } = 1;
        public double LensCustomC3 { get; set; }
        public double LensCustomF { get; set; } = 1;
        public string LensCustomFun { get; set; } = "tan";

        // Lens projection parameters
        public double LensProjectionPFx { get; set; } = 277;
        public double LensProjectionPFy { get; set; } = 277;
        public double LensProjectionPCx { get; set; } = 160;
        public double LensProjectionPCy { get; set; } = 120;
        public double LensProjectionTx { get; set; }
        public double LensProjectionTy { get; set; }

        // Depth camera
        public string DepthCameraOutput { get; set; } = "depths";

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var hfov = sdf.FindElement("horizontal_fov");
            if (hfov?.Value != null) HorizontalFov = hfov.Value.DoubleValue;

            var image = sdf.FindElement("image");
            if (image != null)
            {
                var w = image.FindElement("width");
                if (w?.Value != null) ImageWidth = (uint)w.Value.IntValue;
                var h = image.FindElement("height");
                if (h?.Value != null) ImageHeight = (uint)h.Value.IntValue;
                var fmt = image.FindElement("format");
                if (fmt?.Value != null) ImageFormat = fmt.Value.GetAsString();
            }

            var clip = sdf.FindElement("clip");
            if (clip != null)
            {
                var near = clip.FindElement("near");
                if (near?.Value != null) NearClip = near.Value.DoubleValue;
                var far = clip.FindElement("far");
                if (far?.Value != null) FarClip = far.Value.DoubleValue;
            }

            // Camera name attribute
            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            // Depth camera clip
            var depthCam = sdf.FindElement("depth_camera");
            if (depthCam != null)
            {
                var depthOutput = depthCam.FindElement("output");
                if (depthOutput?.Value != null) DepthCameraOutput = depthOutput.Value.GetAsString();
                var depthClip = depthCam.FindElement("clip");
                if (depthClip != null)
                {
                    var dNear = depthClip.FindElement("near");
                    if (dNear?.Value != null) DepthNearClip = dNear.Value.DoubleValue;
                    var dFar = depthClip.FindElement("far");
                    if (dFar?.Value != null) DepthFarClip = dFar.Value.DoubleValue;
                }
            }

            // Save frames
            var save = sdf.FindElement("save");
            if (save != null)
            {
                var enabledAttr = save.GetAttribute("enabled");
                if (enabledAttr != null) SaveFrames = enabledAttr.BoolValue;
                var path = save.FindElement("path");
                if (path?.Value != null) SavePath = path.Value.GetAsString();
            }

            // Noise
            var noise = sdf.FindElement("noise");
            if (noise != null)
                errors.AddRange(ImageNoise.Load(noise));

            // Visibility mask
            var visMask = sdf.FindElement("visibility_mask");
            if (visMask?.Value != null) VisibilityMask = (uint)visMask.Value.IntValue;

            // Anti-aliasing
            if (image != null)
            {
                var aa = image.FindElement("anti_aliasing");
                if (aa?.Value != null) AntiAliasingValue = (uint)aa.Value.IntValue;
            }

            // Triggered camera
            var triggered = sdf.FindElement("triggered");
            if (triggered?.Value != null) HasTriggeredCamera = triggered.Value.BoolValue;
            var triggerTopic = sdf.FindElement("trigger_topic");
            if (triggerTopic?.Value != null) TriggerTopic = triggerTopic.Value.GetAsString();
            var cameraInfoTopic = sdf.FindElement("camera_info_topic");
            if (cameraInfoTopic?.Value != null) CameraInfoTopic = cameraInfoTopic.Value.GetAsString();
            var opticalFrameId = sdf.FindElement("optical_frame_id");
            if (opticalFrameId?.Value != null) OpticalFrameId = opticalFrameId.Value.GetAsString();
            var segType = sdf.FindElement("segmentation_type");
            if (segType?.Value != null) SegmentationType = segType.Value.GetAsString();
            var boxType = sdf.FindElement("box_type");
            if (boxType?.Value != null) BoxType = boxType.Value.GetAsString();

            // Distortion
            var distortion = sdf.FindElement("distortion");
            if (distortion != null)
            {
                var k1 = distortion.FindElement("k1");
                if (k1?.Value != null) DistortionK1 = k1.Value.DoubleValue;
                var k2 = distortion.FindElement("k2");
                if (k2?.Value != null) DistortionK2 = k2.Value.DoubleValue;
                var k3 = distortion.FindElement("k3");
                if (k3?.Value != null) DistortionK3 = k3.Value.DoubleValue;
                var p1 = distortion.FindElement("p1");
                if (p1?.Value != null) DistortionP1 = p1.Value.DoubleValue;
                var p2 = distortion.FindElement("p2");
                if (p2?.Value != null) DistortionP2 = p2.Value.DoubleValue;
                var center = distortion.FindElement("center");
                if (center?.Value != null)
                {
                    var parts = center.Value.GetAsString().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 &&
                        double.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var cx) &&
                        double.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var cy))
                        DistortionCenter = new Vector2d(cx, cy);
                }
            }

            // Lens
            var lens = sdf.FindElement("lens");
            if (lens != null)
            {
                var lType = lens.FindElement("type");
                if (lType?.Value != null) LensType = lType.Value.GetAsString();
                var scaleHfov = lens.FindElement("scale_to_hfov");
                if (scaleHfov?.Value != null) LensScaleToHfov = scaleHfov.Value.BoolValue;
                var cutoff = lens.FindElement("cutoff_angle");
                if (cutoff?.Value != null) LensCutoffAngle = cutoff.Value.DoubleValue;
                var envTex = lens.FindElement("env_texture_size");
                if (envTex?.Value != null) LensEnvTextureSize = envTex.Value.IntValue;

                var intrinsics = lens.FindElement("intrinsics");
                if (intrinsics != null)
                {
                    var fx = intrinsics.FindElement("fx");
                    if (fx?.Value != null) LensIntrinsicsFx = fx.Value.DoubleValue;
                    var fy = intrinsics.FindElement("fy");
                    if (fy?.Value != null) LensIntrinsicsFy = fy.Value.DoubleValue;
                    var cx = intrinsics.FindElement("cx");
                    if (cx?.Value != null) LensIntrinsicsCx = cx.Value.DoubleValue;
                    var cy = intrinsics.FindElement("cy");
                    if (cy?.Value != null) LensIntrinsicsCy = cy.Value.DoubleValue;
                    var s = intrinsics.FindElement("s");
                    if (s?.Value != null) LensIntrinsicsS = s.Value.DoubleValue;
                }

                var customFunc = lens.FindElement("custom_function");
                if (customFunc != null)
                {
                    var c1 = customFunc.FindElement("c1");
                    if (c1?.Value != null) LensCustomC1 = c1.Value.DoubleValue;
                    var c2 = customFunc.FindElement("c2");
                    if (c2?.Value != null) LensCustomC2 = c2.Value.DoubleValue;
                    var c3 = customFunc.FindElement("c3");
                    if (c3?.Value != null) LensCustomC3 = c3.Value.DoubleValue;
                    var f = customFunc.FindElement("f");
                    if (f?.Value != null) LensCustomF = f.Value.DoubleValue;
                    var fun = customFunc.FindElement("fun");
                    if (fun?.Value != null) LensCustomFun = fun.Value.GetAsString();
                }

                var projection = lens.FindElement("projection");
                if (projection != null)
                {
                    var pfx = projection.FindElement("p_fx");
                    if (pfx?.Value != null) LensProjectionPFx = pfx.Value.DoubleValue;
                    var pfy = projection.FindElement("p_fy");
                    if (pfy?.Value != null) LensProjectionPFy = pfy.Value.DoubleValue;
                    var pcx = projection.FindElement("p_cx");
                    if (pcx?.Value != null) LensProjectionPCx = pcx.Value.DoubleValue;
                    var pcy = projection.FindElement("p_cy");
                    if (pcy?.Value != null) LensProjectionPCy = pcy.Value.DoubleValue;
                    var tx = projection.FindElement("tx");
                    if (tx?.Value != null) LensProjectionTx = tx.Value.DoubleValue;
                    var ty = projection.FindElement("ty");
                    if (ty?.Value != null) LensProjectionTy = ty.Value.DoubleValue;
                }
            }

            return errors;
        }
    }

    /// <summary>Force-torque sensor properties.</summary>
    public class ForceTorqueSensor : SdfElement
    {
        public enum FrameType { Child, Parent, Sensor }
        public enum MeasureDirectionType { ChildToParent, ParentToChild }

        public FrameType Frame { get; set; } = FrameType.Child;
        public MeasureDirectionType MeasureDirection { get; set; } = MeasureDirectionType.ChildToParent;
        public Noise ForceXNoise { get; set; } = new();
        public Noise ForceYNoise { get; set; } = new();
        public Noise ForceZNoise { get; set; } = new();
        public Noise TorqueXNoise { get; set; } = new();
        public Noise TorqueYNoise { get; set; } = new();
        public Noise TorqueZNoise { get; set; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var frameElem = sdf.FindElement("frame");
            if (frameElem?.Value != null)
            {
                Frame = frameElem.Value.GetAsString().ToLowerInvariant() switch
                {
                    "parent" => FrameType.Parent,
                    "sensor" => FrameType.Sensor,
                    _ => FrameType.Child,
                };
            }

            var dirElem = sdf.FindElement("measure_direction");
            if (dirElem?.Value != null)
            {
                MeasureDirection = dirElem.Value.GetAsString().ToLowerInvariant() switch
                {
                    "parent_to_child" => MeasureDirectionType.ParentToChild,
                    _ => MeasureDirectionType.ChildToParent,
                };
            }

            var force = sdf.FindElement("force");
            if (force != null)
            {
                var fx = force.FindElement("x");
                if (fx != null && fx.HasElement("noise"))
                    errors.AddRange(ForceXNoise.Load(fx.FindElement("noise")!));
                var fy = force.FindElement("y");
                if (fy != null && fy.HasElement("noise"))
                    errors.AddRange(ForceYNoise.Load(fy.FindElement("noise")!));
                var fz = force.FindElement("z");
                if (fz != null && fz.HasElement("noise"))
                    errors.AddRange(ForceZNoise.Load(fz.FindElement("noise")!));
            }

            var torque = sdf.FindElement("torque");
            if (torque != null)
            {
                var tx = torque.FindElement("x");
                if (tx != null && tx.HasElement("noise"))
                    errors.AddRange(TorqueXNoise.Load(tx.FindElement("noise")!));
                var ty = torque.FindElement("y");
                if (ty != null && ty.HasElement("noise"))
                    errors.AddRange(TorqueYNoise.Load(ty.FindElement("noise")!));
                var tz = torque.FindElement("z");
                if (tz != null && tz.HasElement("noise"))
                    errors.AddRange(TorqueZNoise.Load(tz.FindElement("noise")!));
            }

            return errors;
        }
    }

    /// <summary>IMU sensor properties.</summary>
    public class ImuSensor : SdfElement
    {
        public Noise LinearAccelerationXNoise { get; set; } = new();
        public Noise LinearAccelerationYNoise { get; set; } = new();
        public Noise LinearAccelerationZNoise { get; set; } = new();
        public Noise AngularVelocityXNoise { get; set; } = new();
        public Noise AngularVelocityYNoise { get; set; } = new();
        public Noise AngularVelocityZNoise { get; set; } = new();
        public Vector3d GravityDirX { get; set; } = Vector3d.UnitX;
        public string GravityDirXParentFrame { get; set; } = string.Empty;
        public Vector3d CustomRpy { get; set; } = Vector3d.Zero;
        public string CustomRpyParentFrame { get; set; } = string.Empty;
        public string Localization { get; set; } = "CUSTOM";
        public bool OrientationEnabled { get; set; } = true;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var linAccel = sdf.FindElement("linear_acceleration");
            if (linAccel != null)
            {
                var x = linAccel.FindElement("x");
                if (x != null && x.HasElement("noise"))
                    errors.AddRange(LinearAccelerationXNoise.Load(x.FindElement("noise")!));
                var y = linAccel.FindElement("y");
                if (y != null && y.HasElement("noise"))
                    errors.AddRange(LinearAccelerationYNoise.Load(y.FindElement("noise")!));
                var z = linAccel.FindElement("z");
                if (z != null && z.HasElement("noise"))
                    errors.AddRange(LinearAccelerationZNoise.Load(z.FindElement("noise")!));
            }

            var angVel = sdf.FindElement("angular_velocity");
            if (angVel != null)
            {
                var x = angVel.FindElement("x");
                if (x != null && x.HasElement("noise"))
                    errors.AddRange(AngularVelocityXNoise.Load(x.FindElement("noise")!));
                var y = angVel.FindElement("y");
                if (y != null && y.HasElement("noise"))
                    errors.AddRange(AngularVelocityYNoise.Load(y.FindElement("noise")!));
                var z = angVel.FindElement("z");
                if (z != null && z.HasElement("noise"))
                    errors.AddRange(AngularVelocityZNoise.Load(z.FindElement("noise")!));
            }

            var orientEnabled = sdf.FindElement("enable_orientation");
            if (orientEnabled?.Value != null) OrientationEnabled = orientEnabled.Value.BoolValue;

            // Orientation reference frame
            var orientRefFrame = sdf.FindElement("orientation_reference_frame");
            if (orientRefFrame != null)
            {
                var localization = orientRefFrame.FindElement("localization");
                if (localization?.Value != null) Localization = localization.Value.GetAsString();

                var customRpy = orientRefFrame.FindElement("custom_rpy");
                if (customRpy?.Value != null)
                {
                    CustomRpy = customRpy.Value.Vector3dValue;
                    var parentFrame = customRpy.GetAttribute("parent_frame");
                    if (parentFrame != null) CustomRpyParentFrame = parentFrame.GetAsString();
                }

                var gravDirX = orientRefFrame.FindElement("grav_dir_x");
                if (gravDirX?.Value != null)
                {
                    GravityDirX = gravDirX.Value.Vector3dValue;
                    var parentFrame = gravDirX.GetAttribute("parent_frame");
                    if (parentFrame != null) GravityDirXParentFrame = parentFrame.GetAsString();
                }
            }

            return errors;
        }
    }

    /// <summary>Lidar (ray) sensor properties.</summary>
    public class LidarSensor : SdfElement
    {
        public uint HorizontalScanSamples { get; set; } = 640;
        public double HorizontalScanResolution { get; set; } = 1.0;
        public Angle HorizontalScanMinAngle { get; set; } = Angle.Zero;
        public Angle HorizontalScanMaxAngle { get; set; } = Angle.Zero;
        public uint VerticalScanSamples { get; set; } = 1;
        public double VerticalScanResolution { get; set; } = 1.0;
        public Angle VerticalScanMinAngle { get; set; } = Angle.Zero;
        public Angle VerticalScanMaxAngle { get; set; } = Angle.Zero;
        public double RangeMin { get; set; } = 0.0;
        public double RangeMax { get; set; } = 0.0;
        public double RangeResolution { get; set; } = 0.0;
        public Noise RangeNoise { get; set; } = new();
        public uint VisibilityMask { get; set; } = 0xFFFFFFFF;

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var scan = sdf.FindElement("scan");
            if (scan != null)
            {
                var horiz = scan.FindElement("horizontal");
                if (horiz != null)
                {
                    var samples = horiz.FindElement("samples");
                    if (samples?.Value != null) HorizontalScanSamples = (uint)samples.Value.IntValue;
                    var res = horiz.FindElement("resolution");
                    if (res?.Value != null) HorizontalScanResolution = res.Value.DoubleValue;
                    var minA = horiz.FindElement("min_angle");
                    if (minA?.Value != null) HorizontalScanMinAngle = new Angle(minA.Value.DoubleValue);
                    var maxA = horiz.FindElement("max_angle");
                    if (maxA?.Value != null) HorizontalScanMaxAngle = new Angle(maxA.Value.DoubleValue);
                }

                var vert = scan.FindElement("vertical");
                if (vert != null)
                {
                    var samples = vert.FindElement("samples");
                    if (samples?.Value != null) VerticalScanSamples = (uint)samples.Value.IntValue;
                    var res = vert.FindElement("resolution");
                    if (res?.Value != null) VerticalScanResolution = res.Value.DoubleValue;
                    var minA = vert.FindElement("min_angle");
                    if (minA?.Value != null) VerticalScanMinAngle = new Angle(minA.Value.DoubleValue);
                    var maxA = vert.FindElement("max_angle");
                    if (maxA?.Value != null) VerticalScanMaxAngle = new Angle(maxA.Value.DoubleValue);
                }
            }

            var range = sdf.FindElement("range");
            if (range != null)
            {
                var min = range.FindElement("min");
                if (min?.Value != null) RangeMin = min.Value.DoubleValue;
                var max = range.FindElement("max");
                if (max?.Value != null) RangeMax = max.Value.DoubleValue;
                var res = range.FindElement("resolution");
                if (res?.Value != null) RangeResolution = res.Value.DoubleValue;
            }

            var noise = sdf.FindElement("noise");
            if (noise != null)
                errors.AddRange(RangeNoise.Load(noise));

            var visMask = sdf.FindElement("visibility_mask");
            if (visMask?.Value != null) VisibilityMask = (uint)visMask.Value.IntValue;

            return errors;
        }
    }

    /// <summary>Magnetometer sensor properties.</summary>
    public class MagnetometerSensor : SdfElement
    {
        public Noise XNoise { get; set; } = new();
        public Noise YNoise { get; set; } = new();
        public Noise ZNoise { get; set; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var x = sdf.FindElement("x");
            if (x != null && x.HasElement("noise"))
                errors.AddRange(XNoise.Load(x.FindElement("noise")!));
            var y = sdf.FindElement("y");
            if (y != null && y.HasElement("noise"))
                errors.AddRange(YNoise.Load(y.FindElement("noise")!));
            var z = sdf.FindElement("z");
            if (z != null && z.HasElement("noise"))
                errors.AddRange(ZNoise.Load(z.FindElement("noise")!));

            return errors;
        }
    }

    /// <summary>NavSat (GPS) sensor properties.</summary>
    public class NavSatSensor : SdfElement
    {
        public Noise HorizontalPositionNoise { get; set; } = new();
        public Noise VerticalPositionNoise { get; set; } = new();
        public Noise HorizontalVelocityNoise { get; set; } = new();
        public Noise VerticalVelocityNoise { get; set; } = new();

        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var posSensing = sdf.FindElement("position_sensing");
            if (posSensing != null)
            {
                var horiz = posSensing.FindElement("horizontal");
                if (horiz != null && horiz.HasElement("noise"))
                    errors.AddRange(HorizontalPositionNoise.Load(horiz.FindElement("noise")!));
                var vert = posSensing.FindElement("vertical");
                if (vert != null && vert.HasElement("noise"))
                    errors.AddRange(VerticalPositionNoise.Load(vert.FindElement("noise")!));
            }

            var velSensing = sdf.FindElement("velocity_sensing");
            if (velSensing != null)
            {
                var horiz = velSensing.FindElement("horizontal");
                if (horiz != null && horiz.HasElement("noise"))
                    errors.AddRange(HorizontalVelocityNoise.Load(horiz.FindElement("noise")!));
                var vert = velSensing.FindElement("vertical");
                if (vert != null && vert.HasElement("noise"))
                    errors.AddRange(VerticalVelocityNoise.Load(vert.FindElement("noise")!));
            }

            return errors;
        }
    }

    // ---- Main Sensor class ----

    /// <summary>
    /// Information about an SDF sensor. A sensor can be attached to a link or joint.
    /// </summary>
    public class Sensor : SdfNamedPosedElement
    {
        /// <summary>Sensor type.</summary>
        public SensorType Type { get; set; } = SensorType.None;

        /// <summary>Topic for sensor data publishing.</summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>Frame ID.</summary>
        public string FrameId { get; set; } = string.Empty;

        /// <summary>Whether metrics are enabled.</summary>
        public bool EnableMetrics { get; set; }

        /// <summary>Whether sensor is always on.</summary>
        public bool AlwaysOn { get; set; }

        /// <summary>Whether sensor is visualized in GUI.</summary>
        public bool Visualize { get; set; }

        /// <summary>Update rate in Hz.</summary>
        public double UpdateRate { get; set; }

        // Sensor-specific data
        public AltimeterSensor? Altimeter { get; set; }
        public AirPressureSensor? AirPressure { get; set; }
        public AirSpeedSensor? AirSpeed { get; set; }
        public CameraSensor? Camera { get; set; }
        public ForceTorqueSensor? ForceTorque { get; set; }
        public ImuSensor? Imu { get; set; }
        public LidarSensor? Lidar { get; set; }
        public MagnetometerSensor? Magnetometer { get; set; }
        public NavSatSensor? NavSat { get; set; }
        public LogicalCameraSensor? LogicalCamera { get; set; }
        public SonarSensor? Sonar { get; set; }

        /// <summary>Plugins.</summary>
        public List<Plugin> Plugins { get; } = new();

        /// <summary>Set the sensor type from a string.</summary>
        public bool SetType(string typeStr)
        {
            Type = typeStr.ToLowerInvariant() switch
            {
                "altimeter" => SensorType.Altimeter,
                "camera" => SensorType.Camera,
                "contact" => SensorType.Contact,
                "depth" or "depth_camera" => SensorType.DepthCamera,
                "force_torque" => SensorType.ForceTorque,
                "gps" => SensorType.Gps,
                "gpu_lidar" => SensorType.GpuLidar,
                "imu" => SensorType.Imu,
                "logical_camera" => SensorType.LogicalCamera,
                "magnetometer" => SensorType.Magnetometer,
                "multicamera" => SensorType.Multicamera,
                "lidar" or "ray" or "gpu_ray" => SensorType.Lidar,
                "rfid" => SensorType.Rfid,
                "rfidtag" => SensorType.RfidTag,
                "sonar" => SensorType.Sonar,
                "wireless_receiver" => SensorType.WirelessReceiver,
                "wireless_transmitter" => SensorType.WirelessTransmitter,
                "air_pressure" => SensorType.AirPressure,
                "rgbd_camera" or "rgbd" => SensorType.RgbdCamera,
                "thermal_camera" or "thermal" => SensorType.ThermalCamera,
                "navsat" => SensorType.NavSat,
                "segmentation" or "segmentation_camera" => SensorType.SegmentationCamera,
                "boundingbox_camera" or "boundingbox" => SensorType.BoundingBoxCamera,
                "custom" => SensorType.Custom,
                "wide_angle_camera" or "wideanglecamera" => SensorType.WideAngleCamera,
                "air_speed" => SensorType.AirSpeed,
                _ => SensorType.None,
            };
            return Type != SensorType.None;
        }

        /// <summary>Get the type as a string.</summary>
        public string TypeStr => Type switch
        {
            SensorType.Altimeter => "altimeter",
            SensorType.Camera => "camera",
            SensorType.Contact => "contact",
            SensorType.DepthCamera => "depth_camera",
            SensorType.ForceTorque => "force_torque",
            SensorType.Gps => "gps",
            SensorType.GpuLidar => "gpu_lidar",
            SensorType.Imu => "imu",
            SensorType.LogicalCamera => "logical_camera",
            SensorType.Magnetometer => "magnetometer",
            SensorType.Multicamera => "multicamera",
            SensorType.Lidar => "lidar",
            SensorType.Rfid => "rfid",
            SensorType.RfidTag => "rfidtag",
            SensorType.Sonar => "sonar",
            SensorType.WirelessReceiver => "wireless_receiver",
            SensorType.WirelessTransmitter => "wireless_transmitter",
            SensorType.AirPressure => "air_pressure",
            SensorType.RgbdCamera => "rgbd_camera",
            SensorType.ThermalCamera => "thermal_camera",
            SensorType.NavSat => "navsat",
            SensorType.SegmentationCamera => "segmentation_camera",
            SensorType.BoundingBoxCamera => "boundingbox_camera",
            SensorType.Custom => "custom",
            SensorType.WideAngleCamera => "wide_angle_camera",
            SensorType.AirSpeed => "air_speed",
            _ => "none",
        };

        /// <summary>Load from an SDF element.</summary>
        public List<SdfError> Load(Element sdf)
        {
            var errors = new List<SdfError>();
            Element = sdf;

            var nameAttr = sdf.GetAttribute("name");
            if (nameAttr != null) Name = nameAttr.GetAsString();

            var typeAttr = sdf.GetAttribute("type");
            if (typeAttr != null) SetType(typeAttr.GetAsString());

            var poseElem = sdf.FindElement("pose");
            if (poseElem?.Value != null)
            {
                RawPose = poseElem.Value.Pose3dValue;
                var relTo = poseElem.GetAttribute("relative_to");
                if (relTo != null) PoseRelativeTo = relTo.GetAsString();
            }

            var topicElem = sdf.FindElement("topic");
            if (topicElem?.Value != null) Topic = topicElem.Value.GetAsString();

            var updateRate = sdf.FindElement("update_rate");
            if (updateRate?.Value != null) UpdateRate = updateRate.Value.DoubleValue;

            var frameIdElem = sdf.FindElement("frame_id");
            if (frameIdElem?.Value != null) FrameId = frameIdElem.Value.GetAsString();

            var enableMetricsElem = sdf.FindElement("enable_metrics");
            if (enableMetricsElem?.Value != null) EnableMetrics = enableMetricsElem.Value.BoolValue;

            var alwaysOnElem = sdf.FindElement("always_on");
            if (alwaysOnElem?.Value != null) AlwaysOn = alwaysOnElem.Value.BoolValue;

            var visualizeElem = sdf.FindElement("visualize");
            if (visualizeElem?.Value != null) Visualize = visualizeElem.Value.BoolValue;

            // Load sensor-specific data
            switch (Type)
            {
                case SensorType.Altimeter:
                    if (sdf.HasElement("altimeter"))
                    {
                        Altimeter = new AltimeterSensor();
                        errors.AddRange(Altimeter.Load(sdf.FindElement("altimeter")!));
                    }
                    break;
                case SensorType.Camera:
                case SensorType.DepthCamera:
                case SensorType.RgbdCamera:
                case SensorType.ThermalCamera:
                case SensorType.SegmentationCamera:
                case SensorType.BoundingBoxCamera:
                case SensorType.WideAngleCamera:
                    if (sdf.HasElement("camera"))
                    {
                        Camera = new CameraSensor();
                        errors.AddRange(Camera.Load(sdf.FindElement("camera")!));
                    }
                    break;
                case SensorType.ForceTorque:
                    if (sdf.HasElement("force_torque"))
                    {
                        ForceTorque = new ForceTorqueSensor();
                        errors.AddRange(ForceTorque.Load(sdf.FindElement("force_torque")!));
                    }
                    break;
                case SensorType.Imu:
                    if (sdf.HasElement("imu"))
                    {
                        Imu = new ImuSensor();
                        errors.AddRange(Imu.Load(sdf.FindElement("imu")!));
                    }
                    break;
                case SensorType.Lidar:
                case SensorType.GpuLidar:
                    if (sdf.HasElement("lidar") || sdf.HasElement("ray"))
                    {
                        Lidar = new LidarSensor();
                        var lidarElem = sdf.FindElement("lidar") ?? sdf.FindElement("ray");
                        if (lidarElem != null)
                            errors.AddRange(Lidar.Load(lidarElem));
                    }
                    break;
                case SensorType.Magnetometer:
                    if (sdf.HasElement("magnetometer"))
                    {
                        Magnetometer = new MagnetometerSensor();
                        errors.AddRange(Magnetometer.Load(sdf.FindElement("magnetometer")!));
                    }
                    break;
                case SensorType.AirPressure:
                    if (sdf.HasElement("air_pressure"))
                    {
                        AirPressure = new AirPressureSensor();
                        errors.AddRange(AirPressure.Load(sdf.FindElement("air_pressure")!));
                    }
                    break;
                case SensorType.NavSat:
                    if (sdf.HasElement("navsat"))
                    {
                        NavSat = new NavSatSensor();
                        errors.AddRange(NavSat.Load(sdf.FindElement("navsat")!));
                    }
                    break;
                case SensorType.AirSpeed:
                    if (sdf.HasElement("air_speed"))
                    {
                        AirSpeed = new AirSpeedSensor();
                        errors.AddRange(AirSpeed.Load(sdf.FindElement("air_speed")!));
                    }
                    break;
                case SensorType.LogicalCamera:
                    if (sdf.HasElement("logical_camera"))
                    {
                        LogicalCamera = new LogicalCameraSensor();
                        errors.AddRange(LogicalCamera.Load(sdf.FindElement("logical_camera")!));
                    }
                    break;
                case SensorType.Sonar:
                    if (sdf.HasElement("sonar"))
                    {
                        Sonar = new SonarSensor();
                        errors.AddRange(Sonar.Load(sdf.FindElement("sonar")!));
                    }
                    break;
            }

            // Load plugins
            var pluginElem = sdf.FindElement("plugin");
            while (pluginElem != null)
            {
                var plugin = new Plugin();
                errors.AddRange(plugin.Load(pluginElem));
                Plugins.Add(plugin);
                pluginElem = pluginElem.GetNextElement("plugin");
            }

            return errors;
        }

        /// <summary>Convert to an SDF element.</summary>
        public Element ToElement()
        {
            var elem = new Element { Name = "sensor" };
            elem.AddAttribute("name", "string", "", true);
            elem.GetAttribute("name")!.SetFromString(Name);
            elem.AddAttribute("type", "string", "", true);
            elem.GetAttribute("type")!.SetFromString(TypeStr);
            return elem;
        }

        public void ClearPlugins() => Plugins.Clear();
        public void AddPlugin(Plugin plugin) => Plugins.Add(plugin);
    }
}
