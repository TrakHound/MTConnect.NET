// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Samples
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
                var position = Position3D.FromString(CDATA);
                if (position != null)
                {
                    position.X = ConvertUnits(position.X, Units, NativeUnits);
                    position.Y = ConvertUnits(position.Y, Units, NativeUnits);
                    position.Z = ConvertUnits(position.Z, Units, NativeUnits);

                    return position;
                }

                return null;
            }
            set => CDATA = value.ToString();
        }

        public override Position3D NativeValue
        {
            get => Position3D.FromString(CDATA);
            set => CDATA = value.ToString();
        }


        public PathPositionValue(double nativeX, double nativeY, double nativeZ, string nativeUnits = Devices.Samples.PathPositionDataItem.DefaultUnits)
        {
            Value = new Position3D(nativeX, nativeY, nativeZ);
            _units = Devices.Samples.PathPositionDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }


        public override string ToString()
        {
            if (Value != null) return Value.ToString();
            return null;
        }
    }
}
