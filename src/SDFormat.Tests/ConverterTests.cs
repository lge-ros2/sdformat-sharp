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
