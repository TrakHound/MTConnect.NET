// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// A measured or calculated position of a control point reported by a piece of equipment expressed in WORK coordinates.
    /// </summary>
    public class PathPositionValue : SampleValue<Position3D>
    {
        public override Position3D Value
        {
            get
            {
                var position = Position3D.FromString(Result);
                if (position != null)
                {
                    position.X = Devices.Units.Convert(position.X, Units, NativeUnits);
                    position.Y = Devices.Units.Convert(position.Y, Units, NativeUnits);
                    position.Z = Devices.Units.Convert(position.Z, Units, NativeUnits);

                    return position;
                }

                return null;
            }
            set => Result = value.ToString();
        }

        public override Position3D NativeValue
        {
            get => Position3D.FromString(Result);
            set => Result = value.ToString();
        }


        public PathPositionValue(double nativeX, double nativeY, double nativeZ, string nativeUnits = PathPositionDataItem.DefaultUnits)
        {
            Value = new Position3D(nativeX, nativeY, nativeZ);
            _units = PathPositionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }


        public override string ToString()
        {
            if (Value != null) return Value.ToString();
            return null;
        }
    }
}