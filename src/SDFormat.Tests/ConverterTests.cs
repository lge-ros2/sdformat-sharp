// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Tests ported/adapted from upstream libsdformat Converter_TEST.cc.

using System.Linq;
using SDFormat;
using Xunit;

namespace SDFormat.Tests
{
    public class ConverterTests
    {
        private static Element Parse(string xml)
        {
            var (root, errors) = new SdfParser().Parse(xml);
            Assert.Empty(errors);
            Assert.NotNull(root);
            return root!;
        }

        [Fact]
        public void ConvertToLatest_SameVersion_IsNoOp()
        {
            var root = Parse("<sdf version='1.12'><world name='w'/></sdf>");
            var errors = Converter.ConvertToLatest(root);
            Assert.Empty(errors);
            Assert.Equal("1.12", root.GetAttribute("version")!.GetAsString());
        }

        [Fact]
        public void ConvertToLatest_StampsVersionToLatest()
        {
            var root = Parse("<sdf version='1.4'><world name='w'/></sdf>");
            Converter.ConvertToLatest(root);
            Assert.Equal(SdfDocument.DefaultVersion, root.GetAttribute("version")!.GetAsString());
        }

        [Fact]
        public void Rename_ActorStaticAttributeToElement_1_4_to_1_5()
        {
            var root = Parse(@"
<sdf version='1.4'>
  <world name='default'>
    <actor name='actor1' static='true'>
      <link name='link'/>
    </actor>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var actor = root.FindElement("world")!.FindElement("actor")!;
            Assert.Null(actor.GetAttribute("static"));
            var staticElem = actor.FindElement("static");
            Assert.NotNull(staticElem);
            Assert.Equal("true", staticElem!.Value!.GetAsString());
        }

        [Fact]
        public void Add_UseParentModelFrame_1_4_to_1_5()
        {
            var root = Parse(@"
<sdf version='1.4'>
  <world name='default'>
    <model name='m'>
      <joint name='j' type='revolute'>
        <axis>
          <xyz>0 0 1</xyz>
        </axis>
      </joint>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            // The 1.4->1.5 step adds <use_parent_model_frame>true</use_parent_model_frame>,
            // but converting all the way to latest also runs the 1.6->1.7 step, which
            // consumes that flag and turns it into axis/xyz/@expressed_in='__model__'
            // (and removes the flag) — so assert the fully-converged shape.
            var axis = root.FindElement("world")!.FindElement("model")!.FindElement("joint")!.FindElement("axis")!;
            Assert.Null(axis.FindElement("use_parent_model_frame"));
            Assert.Equal("__model__", axis.FindElement("xyz")!.GetAttribute("expressed_in")!.GetAsString());
        }

        [Fact]
        public void Move_GravityAndMagneticField_World_15_to_16()
        {
            var root = Parse(@"
<sdf version='1.5'>
  <world name='default'>
    <physics type='ode'>
      <gravity>1 0 -9.8</gravity>
      <magnetic_field>1 2 3</magnetic_field>
    </physics>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var world = root.FindElement("world")!;
            var physics = world.FindElement("physics")!;
            Assert.Null(physics.FindElement("gravity"));
            Assert.Null(physics.FindElement("magnetic_field"));

            var gravity = world.FindElement("gravity");
            Assert.NotNull(gravity);
            Assert.Equal("1 0 -9.8", gravity!.Value!.GetAsString());

            var magneticField = world.FindElement("magnetic_field");
            Assert.NotNull(magneticField);
            Assert.Equal("1 2 3", magneticField!.Value!.GetAsString());
        }

        [Fact]
        public void CopyAndMove_ImuNoise_15_to_16()
        {
            var root = Parse(@"
<sdf version='1.5'>
  <world name='default'>
    <model name='box_old_imu_noise'>
      <link name='link'>
        <sensor name='imu_sensor' type='imu'>
          <imu>
            <noise>
              <type>gaussian</type>
              <rate>
                <mean>0</mean>
                <stddev>0.0002</stddev>
                <bias_mean>7.5e-06</bias_mean>
                <bias_stddev>8e-07</bias_stddev>
              </rate>
              <accel>
                <mean>0</mean>
                <stddev>0.017</stddev>
                <bias_mean>0.1</bias_mean>
                <bias_stddev>0.001</bias_stddev>
              </accel>
            </noise>
          </imu>
        </sensor>
      </link>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var imu = root.FindElement("world")!.FindElement("model")!.FindElement("link")!
                .FindElement("sensor")!.FindElement("imu")!;

            Assert.Null(imu.FindElement("noise"));

            var angVel = imu.FindElement("angular_velocity")!;
            var linAcc = imu.FindElement("linear_acceleration")!;

            foreach (var axis in new[] { "x", "y", "z" })
            {
                var angNoise = angVel.FindElement(axis)!.FindElement("noise")!;
                var linNoise = linAcc.FindElement(axis)!.FindElement("noise")!;

                Assert.Equal("gaussian", angNoise.GetAttribute("type")!.GetAsString());
                Assert.Equal("gaussian", linNoise.GetAttribute("type")!.GetAsString());

                Assert.Equal("0", angNoise.FindElement("mean")!.Value!.GetAsString());
                Assert.Equal("0", linNoise.FindElement("mean")!.Value!.GetAsString());

                Assert.Equal("0.0002", angNoise.FindElement("stddev")!.Value!.GetAsString());
                Assert.Equal("0.017", linNoise.FindElement("stddev")!.Value!.GetAsString());

                Assert.Equal("7.5e-06", angNoise.FindElement("bias_mean")!.Value!.GetAsString());
                Assert.Equal("0.1", linNoise.FindElement("bias_mean")!.Value!.GetAsString());

                Assert.Equal("8e-07", angNoise.FindElement("bias_stddev")!.Value!.GetAsString());
                Assert.Equal("0.001", linNoise.FindElement("bias_stddev")!.Value!.GetAsString());
            }
        }

        [Fact]
        public void Move_PoseFrameToRelativeTo_16_to_17()
        {
            var root = Parse(@"
<sdf version='1.6'>
  <world name='default'>
    <model name='model'>
      <pose frame='world'>0 0 0 0 0 0</pose>
      <link name='parent'/>
      <link name='child'>
        <pose frame='joint'>0 0 0 0 0 0</pose>
      </link>
      <joint name='joint' type='fixed'>
        <parent>parent</parent>
        <child>child</child>
        <pose frame='parent'>0 0 0 0 0 0</pose>
      </joint>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var model = root.FindElement("world")!.FindElement("model")!;

            var modelPose = model.FindElement("pose")!;
            Assert.Null(modelPose.GetAttribute("frame"));
            Assert.Equal("world", modelPose.GetAttribute("relative_to")!.GetAsString());

            var childLink = model.FindElement("child") ?? model.Children.First(c => c.Name == "link" && c.GetAttribute("name")!.GetAsString() == "child");
            var childPose = childLink.FindElement("pose")!;
            Assert.Null(childPose.GetAttribute("frame"));
            Assert.Equal("joint", childPose.GetAttribute("relative_to")!.GetAsString());

            var joint = model.FindElement("joint")!;
            var jointPose = joint.FindElement("pose")!;
            Assert.Null(jointPose.GetAttribute("frame"));
            Assert.Equal("parent", jointPose.GetAttribute("relative_to")!.GetAsString());
        }

        [Fact]
        public void Map_UseParentModelFrameToExpressedIn_16_to_17()
        {
            var root = Parse(@"
<sdf version='1.6'>
  <world name='default'>
    <model name='m'>
      <joint name='j' type='revolute'>
        <axis>
          <xyz>0 0 1</xyz>
          <use_parent_model_frame>true</use_parent_model_frame>
        </axis>
      </joint>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var axis = root.FindElement("world")!.FindElement("model")!.FindElement("joint")!.FindElement("axis")!;
            Assert.Null(axis.FindElement("use_parent_model_frame"));
            var xyz = axis.FindElement("xyz")!;
            Assert.Equal("__model__", xyz.GetAttribute("expressed_in")!.GetAsString());
        }

        [Fact]
        public void Unflatten_ModelFrameBecomesCanonicalLinkAndPose_17_to_18()
        {
            var root = Parse(@"
<sdf version='1.7'>
  <world name='default'>
    <model name='include_links'>
      <frame name='A::__model__' attached_to='A::B::C'>
        <pose relative_to='__model__'>1 0 0 0 0 0</pose>
      </frame>
      <frame name='A::B::__model__' attached_to='A::B::C'>
        <pose relative_to='A::__model__'>0 1 0 0 0 0</pose>
      </frame>
      <link name='A::B::C'>
        <pose relative_to='A::B::__model__'>0 0 1 0 0 0</pose>
      </link>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var outer = root.FindElement("world")!.FindElement("model")!; // include_links
            var modelA = outer.FindElement("model")!;
            Assert.Equal("A", modelA.GetAttribute("name")!.GetAsString());
            Assert.Equal("B::C", modelA.GetAttribute("canonical_link")!.GetAsString());
            Assert.Equal("__model__", modelA.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());

            var modelB = modelA.FindElement("model")!;
            Assert.Equal("B", modelB.GetAttribute("name")!.GetAsString());
            Assert.Equal("C", modelB.GetAttribute("canonical_link")!.GetAsString());
            Assert.Equal("__model__", modelB.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());

            var linkC = modelB.FindElement("link")!;
            Assert.Equal("C", linkC.GetAttribute("name")!.GetAsString());
            Assert.Equal("__model__", linkC.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());
        }

        [Fact]
        public void Unflatten_MultipleLevelSubmodels_17_to_18()
        {
            var root = Parse(@"
<sdf version='1.7'>
  <world name='default'>
    <model name='ParentModel'>
      <link name='ChildModel::L1'/>
      <model name='A::B'/>
      <model name='C'>
        <link name='D::E'/>
        <link name='F::G::H'/>
      </model>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var parent = root.FindElement("world")!.FindElement("model")!;
            Assert.Equal("ParentModel", parent.GetAttribute("name")!.GetAsString());

            var modelC = parent.Children.First(c => c.Name == "model" && c.GetAttribute("name")!.GetAsString() == "C");
            var modelD = modelC.Children.First(c => c.Name == "model" && c.GetAttribute("name")!.GetAsString() == "D");
            Assert.Equal("E", modelD.FindElement("link")!.GetAttribute("name")!.GetAsString());

            var modelF = modelC.Children.First(c => c.Name == "model" && c.GetAttribute("name")!.GetAsString() == "F");
            var modelG = modelF.FindElement("model")!;
            Assert.Equal("G", modelG.GetAttribute("name")!.GetAsString());
            Assert.Equal("H", modelG.FindElement("link")!.GetAttribute("name")!.GetAsString());

            var modelChild = parent.Children.First(c => c.Name == "model" && c.GetAttribute("name")!.GetAsString() == "ChildModel");
            Assert.Equal("L1", modelChild.FindElement("link")!.GetAttribute("name")!.GetAsString());

            var modelA = parent.Children.First(c => c.Name == "model" && c.GetAttribute("name")!.GetAsString() == "A");
            Assert.Equal("B", modelA.FindElement("model")!.GetAttribute("name")!.GetAsString());
        }

        [Fact]
        public void RemoveEmpty_InertialPoseRelativeTo_17_to_18()
        {
            var root = Parse(@"
<sdf version='1.7'>
  <world name='default'>
    <model name='m'>
      <link name='l'>
        <inertial>
          <pose relative_to=''>0 0 0 0 0 0</pose>
          <mass>1.0</mass>
        </inertial>
      </link>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var inertial = root.FindElement("world")!.FindElement("model")!.FindElement("link")!.FindElement("inertial")!;
            var pose = inertial.FindElement("pose")!;
            Assert.Null(pose.GetAttribute("relative_to"));
        }

        [Fact]
        public void RemoveEmpty_KeepsNonEmptyRelativeTo_17_to_18()
        {
            var root = Parse(@"
<sdf version='1.7'>
  <world name='default'>
    <model name='m'>
      <link name='l'>
        <inertial>
          <pose relative_to='some_frame'>0 0 0 0 0 0</pose>
          <mass>1.0</mass>
        </inertial>
      </link>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var inertial = root.FindElement("world")!.FindElement("model")!.FindElement("link")!.FindElement("inertial")!;
            var pose = inertial.FindElement("pose")!;
            Assert.Equal("some_frame", pose.GetAttribute("relative_to")!.GetAsString());
        }

        [Fact]
        public void Unflatten_JointAxisSensorCameraGripper_17_to_18()
        {
            // Mirrors upstream Converter_TEST.cc World_17_to_18's second fixture:
            // exercises joint parent/child prefix stripping, axis/axis2 xyz
            // expressed_in, joint/sensor/pose and nested sensor/camera/pose
            // relative_to stripping, link/sensor/camera/pose stripping, and
            // both the with-prefix and without-prefix <gripper> cases.
            var root = Parse(@"
<sdf version='1.7'>
  <world name='default'>
    <model name='ParentModel'>
      <frame name='ChildModel::__model__' attached_to='ChildModel::L1'>
        <pose relative_to='__model__'>1 0 1 0 0 0</pose>
      </frame>
      <frame name='ChildModel::NewFrame' attached_to='ChildModel::L1'>
        <pose relative_to='ChildModel::Something'>1 0 1 0 0 0</pose>
      </frame>
      <link name='ChildModel::L1'>
        <pose relative_to='ChildModel::__model__'>0 1 0 0 0 0</pose>
        <sensor name='s1'>
          <camera name='c1' type='camera'>
            <pose relative_to='ChildModel::__model__'>0 0 1 0 0 0</pose>
          </camera>
        </sensor>
      </link>
      <link name='ChildModel::L2'>
        <pose relative_to='ChildModel::__model__'>0 0 0 0 0 0</pose>
      </link>
      <joint name='ChildModel::J1' type='revolute'>
        <parent>ChildModel::L1</parent>
        <child>ChildModel::L2</child>
      </joint>
      <joint name='ChildModel::J2' type='revolute'>
        <pose relative_to='ChildModel::__model__'>0 0 0 0 0 0</pose>
        <parent>ChildModel::L1</parent>
        <child>ChildModel::L2</child>
        <axis>
          <xyz expressed_in='ChildModel::NewFrame'>0 0 1</xyz>
        </axis>
        <axis2>
          <xyz expressed_in='ChildModel::NewFrame'>0 0 1</xyz>
        </axis2>
        <sensor name='camera' type='camera'>
          <pose relative_to='ChildModel::NewFrame'>1 0 0 0 0 0</pose>
          <camera name='c2'>
            <pose relative_to='ChildModel::NewFrame'>0 0 1 0 0 0</pose>
          </camera>
        </sensor>
      </joint>
      <gripper name='gripper'>
        <gripper_link>ChildModel::L1</gripper_link>
        <palm_link>ChildModel::L2</palm_link>
      </gripper>
      <gripper name='ChildModel::gripper2'>
        <gripper_link>ChildModel::L1</gripper_link>
        <palm_link>ChildModel::L2</palm_link>
      </gripper>
    </model>
  </world>
</sdf>");
            Converter.ConvertToLatest(root);

            var parent = root.FindElement("world")!.FindElement("model")!;
            Assert.Equal("ParentModel", parent.GetAttribute("name")!.GetAsString());

            var childModel = parent.FindElement("model")!;
            Assert.Equal("ChildModel", childModel.GetAttribute("name")!.GetAsString());
            Assert.Equal("L1", childModel.GetAttribute("canonical_link")!.GetAsString());
            Assert.Equal("__model__", childModel.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());

            var newFrame = childModel.FindElement("frame")!;
            Assert.Equal("NewFrame", newFrame.GetAttribute("name")!.GetAsString());
            Assert.Equal("L1", newFrame.GetAttribute("attached_to")!.GetAsString());
            Assert.Equal("Something", newFrame.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());

            var l1 = childModel.Children.First(c => c.Name == "link" && c.GetAttribute("name")!.GetAsString() == "L1");
            Assert.Equal("__model__", l1.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());
            var camera1 = l1.FindElement("sensor")!.FindElement("camera")!;
            Assert.Equal("__model__", camera1.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());

            var l2 = childModel.Children.First(c => c.Name == "link" && c.GetAttribute("name")!.GetAsString() == "L2");
            Assert.Equal("__model__", l2.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());

            var j1 = childModel.Children.First(c => c.Name == "joint" && c.GetAttribute("name")!.GetAsString() == "J1");
            Assert.Equal("L1", j1.FindElement("parent")!.Value!.GetAsString());
            Assert.Equal("L2", j1.FindElement("child")!.Value!.GetAsString());

            var j2 = childModel.Children.First(c => c.Name == "joint" && c.GetAttribute("name")!.GetAsString() == "J2");
            Assert.Equal("__model__", j2.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());
            Assert.Equal("L1", j2.FindElement("parent")!.Value!.GetAsString());
            Assert.Equal("L2", j2.FindElement("child")!.Value!.GetAsString());
            Assert.Equal("NewFrame", j2.FindElement("axis")!.FindElement("xyz")!.GetAttribute("expressed_in")!.GetAsString());
            Assert.Equal("NewFrame", j2.FindElement("axis2")!.FindElement("xyz")!.GetAttribute("expressed_in")!.GetAsString());

            var j2Sensor = j2.FindElement("sensor")!;
            Assert.Equal("NewFrame", j2Sensor.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());
            var j2Camera = j2Sensor.FindElement("camera")!;
            Assert.Equal("NewFrame", j2Camera.FindElement("pose")!.GetAttribute("relative_to")!.GetAsString());

            var gripper1 = childModel.Children.First(c => c.Name == "gripper" && c.GetAttribute("name")!.GetAsString() == "gripper");
            Assert.Equal("L1", gripper1.FindElement("gripper_link")!.Value!.GetAsString());
            Assert.Equal("L2", gripper1.FindElement("palm_link")!.Value!.GetAsString());

            var gripper2 = childModel.Children.First(c => c.Name == "gripper" && c.GetAttribute("name")!.GetAsString() == "gripper2");
            Assert.Equal("L1", gripper2.FindElement("gripper_link")!.Value!.GetAsString());
            Assert.Equal("L2", gripper2.FindElement("palm_link")!.Value!.GetAsString());
        }

        [Fact]
        public void RootLoadSdfString_OldVersion_ConvertsAndParses()
        {
            var root = new Root();
            var errors = root.LoadSdfString(@"
<sdf version='1.5'>
  <world name='default'>
    <physics type='ode'>
      <gravity>0 0 -9.8</gravity>
    </physics>
    <model name='m'>
      <link name='l'>
        <joint name='j' type='revolute'/>
      </link>
    </model>
  </world>
</sdf>");

            Assert.Empty(errors);
            Assert.Equal("1.12", root.Version);

            var world = root.WorldByIndex(0)!;
            Assert.NotNull(world.Element!.FindElement("gravity"));
        }
    }
}
