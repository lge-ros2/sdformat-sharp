// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0
// Regression tests for revolute joints with unbounded ("-inf"/"inf") limits,
// as produced by URDF->SDF conversion for continuous joints.

using SDFormat;
using Xunit;

namespace SDFormat.Tests
{
    public class JointAxisTests
    {
        [Fact]
        public void RevoluteJoint_WithInfLimitLiterals_ParsesAxisLimitsAsInfinite()
        {
            var root = new Root();
            var errors = root.LoadSdfString(@"
<sdf version='1.9'>
  <world name='default'>
    <model name='m'>
      <link name='base_link'/>
      <link name='left_wheel_link'/>
      <joint name='left_wheel_joint' type='revolute'>
        <pose relative_to='base_link'>0.00005 0.203 0.049 0 0 0</pose>
        <parent>base_link</parent>
        <child>left_wheel_link</child>
        <axis>
          <xyz>0 1 0</xyz>
          <limit>
            <lower>-inf</lower>
            <upper>inf</upper>
            <effort>11.7</effort>
            <velocity>11.765</velocity>
          </limit>
          <dynamics>
            <damping>4.5</damping>
            <friction>0.01</friction>
            <spring_reference>0</spring_reference>
            <spring_stiffness>0</spring_stiffness>
          </dynamics>
        </axis>
      </joint>
    </model>
  </world>
</sdf>");

            Assert.Empty(errors);

            var joint = root.WorldByIndex(0)!.ModelByIndex(0)!.JointByIndex(0)!;
            var axis = joint.Axis!;

            Assert.Equal(double.NegativeInfinity, axis.Lower);
            Assert.Equal(double.PositiveInfinity, axis.Upper);

            // Regression guard: previously "-inf"/"inf" silently parsed to 0.0,
            // which made HasJointLimits() in CLOiSim treat the joint as
            // limited with a 0~0 range instead of a free/continuous revolute.
            Assert.NotEqual(0.0, axis.Lower);
            Assert.NotEqual(0.0, axis.Upper);
        }
    }
}
