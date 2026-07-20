// Copyright (C) 2025 Open Source Robotics Foundation
// SPDX-License-Identifier: Apache-2.0

using SDFormat;
using Xunit;

namespace SDFormat.Tests
{
    public class ParamTests
    {
        private static Param MakeDoubleParam(string value)
        {
            var param = new Param("x", "double", "0", false);
            param.SetFromString(value);
            return param;
        }

        [Theory]
        [InlineData("inf")]
        [InlineData("+inf")]
        [InlineData("INF")]
        [InlineData(" inf ")]
        [InlineData("Infinity")]
        public void DoubleValue_PositiveInfinityLiterals_ParseAsPositiveInfinity(string text)
        {
            var param = MakeDoubleParam(text);
            Assert.Equal(double.PositiveInfinity, param.DoubleValue);
        }

        [Theory]
        [InlineData("-inf")]
        [InlineData("-INF")]
        [InlineData(" -inf ")]
        [InlineData("-Infinity")]
        public void DoubleValue_NegativeInfinityLiterals_ParseAsNegativeInfinity(string text)
        {
            var param = MakeDoubleParam(text);
            Assert.Equal(double.NegativeInfinity, param.DoubleValue);
        }

        [Theory]
        [InlineData("0", 0.0)]
        [InlineData("1.5", 1.5)]
        [InlineData("-3.25", -3.25)]
        [InlineData("1e10", 1e10)]
        public void DoubleValue_FiniteLiterals_ParseAsExpectedDouble(string text, double expected)
        {
            var param = MakeDoubleParam(text);
            Assert.Equal(expected, param.DoubleValue);
        }

        [Fact]
        public void DoubleValue_UnparsableText_FallsBackToZero()
        {
            var param = MakeDoubleParam("not-a-number");
            Assert.Equal(0.0, param.DoubleValue);
        }
    }
}
