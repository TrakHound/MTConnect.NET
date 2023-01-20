// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
            get => Degree3D.FromString(Result);
            set => Result = value.ToString();
        }

        public override Degree3D NativeValue
        {
            get => Degree3D.FromString(Result);
            set => Result = value.ToString();
        }


        public OrientationValue(double nativeA, double nativeB, double nativeC, string nativeUnits = OrientationDataItem.DefaultUnits)
        {
            Value = new Degree3D(nativeA, nativeB, nativeC);
            _units = OrientationDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}