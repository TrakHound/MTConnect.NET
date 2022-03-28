// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// A measured or calculated orientation of a plane or vector relative to a cartesian coordinate system
    /// </summary>
    public class OrientationValue : SampleValue<Degree3D>
    {
        public override Degree3D Value
        {
            get => Degree3D.FromString(CDATA);
            set => CDATA = value.ToString();
        }

        public override Degree3D NativeValue
        {
            get => Degree3D.FromString(CDATA);
            set => CDATA = value.ToString();
        }


        public OrientationValue(double nativeA, double nativeB, double nativeC, string nativeUnits = OrientationDataItem.DefaultUnits)
        {
            Value = new Degree3D(nativeA, nativeB, nativeC);
            _units = OrientationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
