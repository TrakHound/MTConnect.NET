// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
{
    /// <summary>
    /// A measured or calculated position of a control point reported by a piece of equipment expressed in WORK coordinates.
    /// </summary>
    public class PathPositionValue : SampleValue
    {
        protected override string MetricUnits => "MILLIMETER_3D";
        protected override string InchUnits => "MILLIMETER_3D";


        public PathPositionValue(double x, double y, double z)
        {
            Value = new Position3D(x, y, z).ToString();
        }
    }
}
