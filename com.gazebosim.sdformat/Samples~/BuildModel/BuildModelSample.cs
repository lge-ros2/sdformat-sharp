// SdFormat Unity Sample — Build a robot model purely from code.
//
// Usage:
//   1. Attach this MonoBehaviour to any GameObject.
//   2. Press Play — a differential-drive robot will be spawned.

using UnityEngine;
using SdFormat;
using SdFormat.Math;
using SdFormat.Unity;

namespace SdFormat.Samples
{
    public class BuildModelSample : MonoBehaviour
    {
        void Start()
        {
            // ── Build the model in SDF DOM ──

            var model = new Model { Name = "my_robot", Static = false };
            model.RawPose = new Pose3d(0, 0, 0.15, 0, 0, 0);

            // Chassis
            var chassis = new Link { Name = "chassis" };
            chassis.Inertial.Mass = 5.0;

            var chassisVis = new Visual { Name = "body_visual" };
            chassisVis.Geom.Type = GeometryType.Box;
            chassisVis.Geom.BoxShape = new Box
            {
                Size = new Vector3d(0.6, 0.4, 0.2)
            };
            chassisVis.MaterialInfo = new Material
            {
                Diffuse = new SdFormat.Math.Color(0.2f, 0.2f, 0.9f)
            };
            chassis.AddVisual(chassisVis);

            var chassisCol = new Collision { Name = "body_collision" };
            chassisCol.Geom.Type = GeometryType.Box;
            chassisCol.Geom.BoxShape = new Box
            {
                Size = new Vector3d(0.6, 0.4, 0.2)
            };
            chassis.AddCollision(chassisCol);
            model.AddLink(chassis);

            // Left wheel
            var leftWheel = new Link { Name = "left_wheel" };
            leftWheel.RawPose = new Pose3d(-0.15, 0.25, -0.05,
                -System.Math.PI / 2, 0, 0);
            leftWheel.Inertial.Mass = 0.5;
            var lwVis = new Visual { Name = "visual" };
            lwVis.Geom.Type = GeometryType.Cylinder;
            lwVis.Geom.CylinderShape = new Cylinder { Radius = 0.1, Length = 0.04 };
            leftWheel.AddVisual(lwVis);
            model.AddLink(leftWheel);

            // Right wheel
            var rightWheel = new Link { Name = "right_wheel" };
            rightWheel.RawPose = new Pose3d(-0.15, -0.25, -0.05,
                -System.Math.PI / 2, 0, 0);
            rightWheel.Inertial.Mass = 0.5;
            var rwVis = new Visual { Name = "visual" };
            rwVis.Geom.Type = GeometryType.Cylinder;
            rwVis.Geom.CylinderShape = new Cylinder { Radius = 0.1, Length = 0.04 };
            rightWheel.AddVisual(rwVis);
            model.AddLink(rightWheel);

            // Joints
            model.AddJoint(new Joint
            {
                Name = "left_wheel_joint",
                Type = JointType.Revolute,
                ParentName = "chassis",
                ChildName = "left_wheel",
                Axis = new JointAxis { Xyz = Vector3d.UnitZ }
            });
            model.AddJoint(new Joint
            {
                Name = "right_wheel_joint",
                Type = JointType.Revolute,
                ParentName = "chassis",
                ChildName = "right_wheel",
                Axis = new JointAxis { Xyz = Vector3d.UnitZ }
            });

            // ── Serialize to SDF XML and log ──
            var root = new Root();
            root.Version = "1.12";
            var world = new World { Name = "default" };
            world.AddModel(model);
            root.AddWorld(world);

            var xml = root.ToElement().ToString("");
            Debug.Log($"[SDF XML]\n{xml}");

            // ── Spawn into Unity scene ──
            SdfSpawner.Spawn(root, transform);
        }
    }
}
