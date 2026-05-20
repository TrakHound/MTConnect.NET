// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    public partial class Measurement
    {
        /// <summary>
        /// The measurement type name, which selects the concrete measurement subtype and its element name in serialized documents.
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// Initializes an empty pallet measurement, typically for deserialization.
        /// </summary>
        public Measurement() { }

        /// <summary>
        /// Initializes a pallet measurement by copying the value, nominal, minimum, maximum, significant digits, native units, and units from an existing measurement; a null source leaves the fields default.
        /// </summary>
        /// <param name="measurement">The measurement to copy from.</param>
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
