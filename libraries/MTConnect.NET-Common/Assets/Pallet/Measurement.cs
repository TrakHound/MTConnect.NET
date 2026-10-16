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


        /// <summary>
        /// Creates the measurement class for <paramref name="type"/> (a <see cref="WeightMeasurement"/> for "Weight",
        /// and so on), or a plain <see cref="Measurement"/> carrying the type name when the type has no class of its own.
        /// </summary>
        /// <param name="type">The measurement type name, as used for its element name.</param>
        public static Measurement Create(string type)
        {
            switch (type)
            {
                case WeightMeasurement.TypeId: return new WeightMeasurement();
                case HeightMeasurement.TypeId: return new HeightMeasurement();
                case WidthMeasurement.TypeId: return new WidthMeasurement();
                case LengthMeasurement.TypeId: return new LengthMeasurement();
                case SwingMeasurement.TypeId: return new SwingMeasurement();
                case LoadedWeightMeasurement.TypeId: return new LoadedWeightMeasurement();
                case LoadedHeightMeasurement.TypeId: return new LoadedHeightMeasurement();
                case LoadedWidthMeasurement.TypeId: return new LoadedWidthMeasurement();
                case LoadedLengthMeasurement.TypeId: return new LoadedLengthMeasurement();
                case LoadedSwingMeasurement.TypeId: return new LoadedSwingMeasurement();
                default: return new Measurement { Type = type };
            }
        }

        /// <summary>
        /// The type name of <paramref name="measurement"/>: its <see cref="Type"/>, or for another
        /// <see cref="IMeasurement"/> implementation its class name without the "Measurement" suffix.
        /// </summary>
        /// <param name="measurement">The measurement to name.</param>
        public static string GetTypeName(IMeasurement measurement)
        {
            if (measurement == null) return null;

            var type = (measurement as Measurement)?.Type;
            if (string.IsNullOrEmpty(type))
            {
                var name = measurement.GetType().Name;
                if (name != nameof(Measurement) && name.EndsWith(nameof(Measurement))) type = name.Substring(0, name.Length - nameof(Measurement).Length);
            }

            return type;
        }
    }
}
