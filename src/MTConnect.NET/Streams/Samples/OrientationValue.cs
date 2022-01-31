// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// A measured or calculated orientation of a plane or vector relative to a cartesian coordinate system
    /// </summary>
    public class OrientationValue : SampleValue
    {
        protected override string MetricUnits => "DEGREE_3D";
        protected override string InchUnits => "DEGREE_3D";


        public OrientationValue(double degreesA, double degreesB, double degreesC)
        {
            Value = new Degree3D(degreesA, degreesB, degreesC).ToString();
        }
    }
}
