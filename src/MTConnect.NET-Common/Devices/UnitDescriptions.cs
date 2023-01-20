// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// Units that are defined as the standard unit of measure for each type of DataItem
    /// </summary>
    public static class UnitDescriptions
    {
        /// <summary>
        /// Amps
        /// </summary>
        public const string AMPERE = "Amps";

        /// <summary>
        /// Degress Celsius
        /// </summary>
        public const string CELSIUS = "Degress Celsius";

        /// <summary>
        /// A count of something
        /// </summary>
        public const string COUNT = " A count of something";

        /// <summary>
        /// Count per second
        /// </summary>
        public const string COUNT_PER_SECOND = "Count per second";

        /// <summary>
        /// Geometric volume in millimeters
        /// </summary>
        public const string CUBIC_MILLIMETER = "Geometric volume in millimeters";

        /// <summary>
        /// Change of geometric volume per second
        /// </summary>
        public const string CUBIC_MILLIMETER_PER_SECOND = "Change of geometric volume per second";

        /// <summary>
        /// Change in geometric volume per second squared
        /// </summary>
        public const string CUBIC_MILLIMETER_PER_SECOND_SQUARED = "Change in geometric volume per second squared";

        /// <summary>
        /// Sound level
        /// </summary>
        public const string DECIBEL = "Sound level";

        /// <summary>
        /// Angle in degrees
        /// </summary>
        public const string DEGREE = "Angle in degrees";

        /// <summary>
        /// Angular degrees per second
        /// </summary>
        public const string DEGREE_PER_SECOND = "Angular degrees per second";

        /// <summary>
        /// Angular acceleration in degrees per second squared
        /// </summary>
        public const string DEGREE_PER_SECOND_SQUARED = "Angular acceleration in degrees per second squared";

        /// <summary>
        /// A space-delimited, floating-point representation of the angular rotation in degrees
        /// around the X, Y, and Z axes relative to a cartesian coordinate system respectively in order as A, B, and C.
        /// </summary>
        public const string DEGREE_3D = "A space-delimited, floating-point representation of the angular rotation in degrees around the X, Y, and Z axes relative to a cartesian coordinate system respectively in order as A, B, and C.";

        /// <summary>
        /// Gram per cubic meter
        /// </summary>
        public const string GRAM_PER_CUBIC_METER = "Gram per cubic meter";

        /// <summary>
        /// Frequency measured in cycles per second
        /// </summary>
        public const string HERTZ = "Frequency measured in cycles per second";

        /// <summary>
        /// A measurement of energy
        /// </summary>
        public const string JOULE = "A measurement of energy";

        /// <summary>
        /// Kilograms
        /// </summary>
        public const string KILOGRAM = "Kilograms";

        /// <summary>
        /// Measurement of volume of a fluid
        /// </summary>
        public const string LITER = "Measurement of volume of a fluid";

        /// <summary>
        /// Liters per second
        /// </summary>
        public const string LITER_PER_SECOND = "Liters per second";

        /// <summary>
        /// Measurement of Tilt
        /// </summary>
        public const string MICRO_RADIAN = "Measurement of Tilt";

        /// <summary>
        /// Milligram
        /// </summary>
        public const string MILLIGRAM = "Milligram";

        /// <summary>
        /// Milligram per cubic millimeter
        /// </summary>
        public const string MILLIGRAM_PER_CUBIC_MILLIMETER = "Milligram per cubic millimeter";

        /// <summary>
        /// Milliliter
        /// </summary>
        public const string MILLILITER = "Milliliter";

        /// <summary>
        /// Millimeters
        /// </summary>
        public const string MILLIMETER = "Millimeters";

        /// <summary>
        /// Millimeters per revolution
        /// </summary>
        public const string MILLIMETER_PER_REVOLUTION = "Millimeters per revolution";

        /// <summary>
        /// Millimeters per second
        /// </summary>
        public const string MILLIMETER_PER_SECOND = "Millimeters per second";

        /// <summary>
        /// Acceleration in millimeters per second squared
        /// </summary>
        public const string MILLIMETER_PER_SECOND_SQUARED = "Acceleration in millimeters per second squared";

        /// <summary>
        /// A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in millimeters.
        /// </summary>
        public const string MILLIMETER_3D = "A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in millimeters.";

        /// <summary>
        /// Force in Newtons
        /// </summary>
        public const string NEWTON = "Force in Newtons";

        /// <summary>
        /// Torque, a unit for force times distance
        /// </summary>
        public const string NEWTON_METER = "Torque, a unit for force times distance";

        /// <summary>
        /// Measure of Electrical Resistance
        /// </summary>
        public const string OHM = "Measure of Electrical Resistance";

        /// <summary>
        /// Pressure in Newtons per square meter
        /// </summary>
        public const string PASCAL = "Pressure in Newtons per square meter";

        /// <summary>
        /// Pascal per second
        /// </summary>
        public const string PASCAL_PER_SECOND = "Pascal per second";

        /// <summary>
        /// Measurement of Viscosity
        /// </summary>
        public const string PASCAL_SECOND = "Measurement of Viscosity";

        /// <summary>
        /// Percentage
        /// </summary>
        public const string PERCENT = "Percentage";

        /// <summary>
        /// A measure of the acidity or alkalinity of a solution
        /// </summary>
        public const string PH = "A measure of the acidity or alkalinity of a solution";

        /// <summary>
        /// Revolutions per minute
        /// </summary>
        public const string REVOLUTION_PER_MINUTE = "Revolutions per minute";

        /// <summary>
        /// Revolutions per second
        /// </summary>
        public const string REVOLUTION_PER_SECOND = "Revolutions per second";

        /// <summary>
        /// Revolutions per second squared
        /// </summary>
        public const string REVOLUTION_PER_SECOND_SQUARED = "Revolutions per second squared";

        /// <summary>
        /// A measurement of time
        /// </summary>
        public const string SECOND = "A measurement of time";

        /// <summary>
        /// A measurement of Electrical Conductivity
        /// </summary>
        public const string SIEMENS_PER_METER = "A measurement of Electrical Conductivity";

        /// <summary>
        /// A 3D Unit Vector.
        /// </summary>
        public const string UNIT_VECTOR_3D = "A 3D Unit Vector.";

        /// <summary>
        /// Volts
        /// </summary>
        public const string VOLT = "Volts";

        /// <summary>
        /// Volt-Ampere (VA)
        /// </summary>
        public const string VOLT_AMPERE = "Volt-Ampere (VA)";

        /// <summary>
        /// Volt-Ampere Reactive (VAR)
        /// </summary>
        public const string VOLT_AMPERE_REACTIVE = "Volt-Ampere Reactive (VAR)";

        /// <summary>
        /// Watts
        /// </summary>
        public const string WATT = "Watts";

        /// <summary>
        /// Measurement of electrical energy, equal to one Joule
        /// </summary>
        public const string WATT_SECOND = "Measurement of electrical energy, equal to one Joule";


        public static string Get(string units)
        {
            switch (units)
            {
                case Units.AMPERE: return AMPERE;
                case Units.CELSIUS: return CELSIUS;
                case Units.COUNT: return COUNT;
                case Units.COUNT_PER_SECOND: return COUNT_PER_SECOND;
                case Units.CUBIC_MILLIMETER: return CUBIC_MILLIMETER;
                case Units.CUBIC_MILLIMETER_PER_SECOND: return CUBIC_MILLIMETER_PER_SECOND;
                case Units.CUBIC_MILLIMETER_PER_SECOND_SQUARED: return CUBIC_MILLIMETER_PER_SECOND_SQUARED;
                case Units.DECIBEL: return DECIBEL;
                case Units.DEGREE: return DEGREE;
                case Units.DEGREE_PER_SECOND: return DEGREE_PER_SECOND;
                case Units.DEGREE_PER_SECOND_SQUARED: return DEGREE_PER_SECOND_SQUARED;
                case Units.DEGREE_3D: return DEGREE_3D;
                case Units.GRAM_PER_CUBIC_METER: return GRAM_PER_CUBIC_METER;
                case Units.HERTZ: return HERTZ;
                case Units.JOULE: return JOULE;
                case Units.KILOGRAM: return KILOGRAM;
                case Units.LITER: return LITER;
                case Units.MICRO_RADIAN: return MICRO_RADIAN;
                case Units.MILLIGRAM: return MILLIGRAM;
                case Units.MILLIGRAM_PER_CUBIC_MILLIMETER: return MILLIGRAM_PER_CUBIC_MILLIMETER;
                case Units.MILLIMETER: return MILLIMETER;
                case Units.MILLIMETER_PER_REVOLUTION: return MILLIMETER_PER_REVOLUTION;
                case Units.MILLIMETER_PER_SECOND: return MILLIMETER_PER_SECOND;
                case Units.MILLIMETER_PER_SECOND_SQUARED: return MILLIMETER_PER_SECOND_SQUARED;
                case Units.MILLIMETER_3D: return MILLIMETER_3D;
                case Units.NEWTON: return NEWTON;
                case Units.NEWTON_METER: return NEWTON_METER;
                case Units.OHM: return OHM;
                case Units.PASCAL: return PASCAL;
                case Units.PASCAL_PER_SECOND: return PASCAL_PER_SECOND;
                case Units.PASCAL_SECOND: return PASCAL_SECOND;
                case Units.PERCENT: return PERCENT;
                case Units.PH: return PH;
                case Units.REVOLUTION_PER_MINUTE: return REVOLUTION_PER_MINUTE;
                case Units.SECOND: return SECOND;
                case Units.SIEMENS_PER_METER: return SIEMENS_PER_METER;
                case Units.UNIT_VECTOR_3D: return UNIT_VECTOR_3D;
                case Units.VOLT: return VOLT;
                case Units.VOLT_AMPERE: return VOLT_AMPERE;
                case Units.VOLT_AMPERE_REACTIVE: return VOLT_AMPERE_REACTIVE;
                case Units.WATT: return WATT;
                case Units.WATT_SECOND: return WATT_SECOND;
            }

            return "";
        }
    }
}
