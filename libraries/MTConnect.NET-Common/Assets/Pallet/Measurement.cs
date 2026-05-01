// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    public partial class Measurement
    {
        public string Type { get; set; }


        public Measurement() { }

        public Measurement(IMeasurement measurement)
        {
            if (measurement != null)
            {
                Value = measurement.Value;
                Nominal = measurement.Nominal;
                Minimum = measurement.Minimum;
                Maximum = measurement.Maximum;
                SignificantDigits = measurement.SignificantDigits;
                NativeUnits = measurement.NativeUnits;
                Units = measurement.Units;
            }
        }
    }
}
