// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// Units that are defined as the standard unit of measure for each type of DataItem
    /// </summary>
    public static class Units
    {
        /// <summary>
        /// Amps
        /// </summary>
        public const string AMPERE = "AMPERE";

        /// <summary>
        /// Degress Celsius
        /// </summary>
        public const string CELSIUS = "CELSIUS";

        /// <summary>
        /// A count of something
        /// </summary>
        public const string COUNT = "COUNT";

        /// <summary>
        /// Count per second
        /// </summary>
        public const string COUNT_PER_SECOND = "COUNT/SECOND";

        /// <summary>
        /// Geometric volume in millimeters
        /// </summary>
        public const string CUBIC_MILLIMETER = "CUBIC_MILLIMETER";

        /// <summary>
        /// Change of geometric volume per second
        /// </summary>
        public const string CUBIC_MILLIMETER_PER_SECOND = "CUBIC_MILLIMETER/SECOND";

        /// <summary>
        /// Change in geometric volume per second squared
        /// </summary>
        public const string CUBIC_MILLIMETER_PER_SECOND_SQUARED = "CUBIC_MILLIMETER/SECOND^2";

        /// <summary>
        /// Sound level
        /// </summary>
        public const string DECIBEL = "DECIBEL";

        /// <summary>
        /// Angle in degrees
        /// </summary>
        public const string DEGREE = "DEGREE";

        /// <summary>
        /// Angular degrees per second
        /// </summary>
        public const string DEGREE_PER_SECOND = "DEGREE/SECOND";

        /// <summary>
        /// Angular acceleration in degrees per second squared
        /// </summary>
        public const string DEGREE_PER_SECOND_SQUARED = "DEGREE/SECOND^2";

        /// <summary>
        /// A space-delimited, floating-point representation of the angular rotation in degrees
        /// around the X, Y, and Z axes relative to a cartesian coordinate system respectively in order as A, B, and C.
        /// </summary>
        public const string DEGREE_3D = "DEGREE_3D";

        /// <summary>
        /// Gram per cubic meter
        /// </summary>
        public const string GRAM_PER_CUBIC_METER = "GRAM/CUBIC_METER";

        /// <summary>
        /// Frequency measured in cycles per second
        /// </summary>
        public const string HERTZ = "HERTZ";

        /// <summary>
        /// A measurement of energy
        /// </summary>
        public const string JOULE = "JOULE";

        /// <summary>
        /// Kilograms
        /// </summary>
        public const string KILOGRAM = "KILOGRAM";

        /// <summary>
        /// Measurement of volume of a fluid
        /// </summary>
        public const string LITER = "LITER";

        /// <summary>
        /// Liters per second
        /// </summary>
        public const string LITER_PER_SECOND = "LITER/SECOND";

        /// <summary>
        /// Measurement of Tilt
        /// </summary>
        public const string MICRO_RADIAN = "MICRO_RADIAN";

        /// <summary>
        /// Milligram
        /// </summary>
        public const string MILLIGRAM = "MILLIGRAM";

        /// <summary>
        /// Milligram per cubic millimeter
        /// </summary>
        public const string MILLIGRAM_PER_CUBIC_MILLIMETER = "MILLIGRAM/CUBIC_MILLIMETER";

        /// <summary>
        /// Milliliter
        /// </summary>
        public const string MILLILITER = "MILLILITER";

        /// <summary>
        /// Millimeters
        /// </summary>
        public const string MILLIMETER = "MILLIMETER";

        /// <summary>
        /// Millimeters per revolution
        /// </summary>
        public const string MILLIMETER_PER_REVOLUTION = "MILLIMETER/REVOLUTION";

        /// <summary>
        /// Millimeters per second
        /// </summary>
        public const string MILLIMETER_PER_SECOND = "MILLIMETER/SECOND";

        /// <summary>
        /// Acceleration in millimeters per second squared
        /// </summary>
        public const string MILLIMETER_PER_SECOND_SQUARED = "MILLIMETER/SECOND^2";

        /// <summary>
        /// A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in millimeters.
        /// </summary>
        public const string MILLIMETER_3D = "MILLIMETER_3D";

        /// <summary>
        /// Force in Newtons
        /// </summary>
        public const string NEWTON = "NEWTON";

        /// <summary>
        /// Torque, a unit for force times distance
        /// </summary>
        public const string NEWTON_METER = "NEWTON_METER";

        /// <summary>
        /// Measure of Electrical Resistance
        /// </summary>
        public const string OHM = "OHM";

        /// <summary>
        /// Pressure in Newtons per square meter
        /// </summary>
        public const string PASCAL = "PASCAL";

        /// <summary>
        /// Pascal per second
        /// </summary>
        public const string PASCAL_PER_SECOND = "PASCAL/SECOND";

        /// <summary>
        /// Measurement of Viscosity
        /// </summary>
        public const string PASCAL_SECOND = "PASCAL_SECOND";

        /// <summary>
        /// Percentage
        /// </summary>
        public const string PERCENT = "PERCENT";

        /// <summary>
        /// A measure of the acidity or alkalinity of a solution
        /// </summary>
        public const string PH = "PH";

        /// <summary>
        /// Revolutions per minute
        /// </summary>
        public const string REVOLUTION_PER_MINUTE = "REVOLUTION/MINUTE";

        /// <summary>
        /// Revolutions per second
        /// </summary>
        public const string REVOLUTION_PER_SECOND = "REVOLUTION/SECOND";

        /// <summary>
        /// Revolutions per second squared
        /// </summary>
        public const string REVOLUTION_PER_SECOND_SQUARED = "REVOLUTION/SECOND^2";

        /// <summary>
        /// A measurement of time
        /// </summary>
        public const string SECOND = "SECOND";

        /// <summary>
        /// A measurement of Electrical Conductivity
        /// </summary>
        public const string SIEMENS_PER_METER = "SIEMENS/METER";

        /// <summary>
        /// A 3D Unit Vector.
        /// </summary>
        public const string UNIT_VECTOR_3D = "UNIT_VECTOR_3D";

        /// <summary>
        /// Volts
        /// </summary>
        public const string VOLT = "VOLT";

        /// <summary>
        /// Volt-Ampere (VA)
        /// </summary>
        public const string VOLT_AMPERE = "VOLT_AMPERE";

        /// <summary>
        /// Volt-Ampere Reactive (VAR)
        /// </summary>
        public const string VOLT_AMPERE_REACTIVE = "VOLT_AMPERE_REACTIVE";

        /// <summary>
        /// Watts
        /// </summary>
        public const string WATT = "WATT";

        /// <summary>
        /// Measurement of electrical energy, equal to one Joule
        /// </summary>
        public const string WATT_SECOND = "WATT_SECOND";


        /// <summary>
        /// Convert the specified value from NativeUnits specified to the Units specified
        /// </summary>
        /// <param name="value">The Value to Convert</param>
        /// <param name="units">The Units to Convert To</param>
        /// <param name="nativeUnits">The NativeUnits to Convert From</param>
        /// <returns>The Converted Sample Value</returns>
        public static double Convert(double value, string units, string nativeUnits)
        {
            if (!string.IsNullOrEmpty(units) && !string.IsNullOrEmpty(nativeUnits))
            {
                switch (units)
                {
                    case NativeUnits.BAR:
                        switch (nativeUnits)
                        {
                            case Units.PASCAL: return value / 100000;
                            case NativeUnits.MILLIMETER_MERCURY: return value / 750.06375542;
                            case NativeUnits.POUND_PER_INCH_SQUARED: return value / 14.503773773;
                            case NativeUnits.TORR: return value / 750.0616827;
                        }
                        break;

                    case Units.CELSIUS:
                        switch (nativeUnits)
                        {
                            case NativeUnits.FAHRENHEIT:
                                double x = value - 32;
                                double y = (double)5 / (double)9;
                                double z = x * y;
                                return z;

                            case NativeUnits.KELVIN: return value / 274.15;
                        }
                        break;

                    case NativeUnits.CENTIPOISE:
                        switch (nativeUnits)
                        {
                            case Units.PASCAL_SECOND: return value * 1000;
                        }
                        break;

                    case Units.DEGREE:
                        switch (nativeUnits)
                        {
                            case NativeUnits.RADIAN: return value * 57.2957795;
                            case Units.MICRO_RADIAN: return value / 17453.2925;
                        }
                        break;

                    case NativeUnits.DEGREE_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case Units.DEGREE_PER_SECOND: return value * 60;
                            case NativeUnits.RADIAN_PER_MINUTE: return value * 57.2957795 / 60;
                            case NativeUnits.RADIAN_PER_SECOND: return value * 57.2957795;
                            case Units.REVOLUTION_PER_MINUTE: return value * 360 / 60;
                            case Units.REVOLUTION_PER_SECOND: return value * 360;
                        }
                        break;

                    case Units.DEGREE_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.DEGREE_PER_MINUTE: return value / 60;
                            case NativeUnits.RADIAN_PER_MINUTE: return value * 57.2957795 / 60;
                            case NativeUnits.RADIAN_PER_SECOND: return value * 57.2957795;
                            case Units.REVOLUTION_PER_MINUTE: return value * 360 / 60;
                            case Units.REVOLUTION_PER_SECOND: return value * 360;
                        }
                        break;

                    case Units.DEGREE_PER_SECOND_SQUARED:
                        switch (nativeUnits)
                        {
                            case NativeUnits.RADIAN_PER_SECOND_SQUARED: return value * 57.2957795;
                            case Units.REVOLUTION_PER_SECOND_SQUARED: return value * 360;
                        }
                        break;

                    case NativeUnits.FAHRENHEIT:
                        switch (nativeUnits)
                        {
                            case Units.CELSIUS: return (value * (9 / 5)) + 32;
                            case NativeUnits.KELVIN: return value / 255.927778;
                        }
                        break;


                    case NativeUnits.FOOT:
                        switch (nativeUnits)
                        {
                            case Units.MILLIMETER: return value / 25.4 / 12;
                            case NativeUnits.INCH: return value / 12;
                        }
                        break;

                    case NativeUnits.FOOT_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case Units.MILLIMETER_PER_SECOND: return value / 25.4 / 12 * 60;
                            case NativeUnits.FOOT_PER_SECOND: return value * 60;
                            case NativeUnits.INCH_PER_MINUTE: return value / 12;
                            case NativeUnits.INCH_PER_SECOND: return value / 12 * 60;
                        }
                        break;

                    case NativeUnits.FOOT_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case Units.MILLIMETER_PER_SECOND: return value / 25.4 / 12;
                            case NativeUnits.FOOT_PER_MINUTE: return value / 60;
                            case NativeUnits.INCH_PER_MINUTE: return value / 12 / 60;
                            case NativeUnits.INCH_PER_SECOND: return value / 12;
                        }
                        break;

                    case NativeUnits.FOOT_PER_SECOND_SQUARED:
                        switch (nativeUnits)
                        {
                            case Units.MILLIMETER_PER_SECOND_SQUARED: return value / 25.4 / 12;
                            case NativeUnits.INCH_PER_SECOND_SQUARED: return value / 12;
                        }
                        break;

                    case NativeUnits.GALLON_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case Units.LITER_PER_SECOND: return value / 3.78541178 * 60;
                        }
                        break;

                    case NativeUnits.HOUR:
                        switch (nativeUnits)
                        {
                            case NativeUnits.MINUTE: return value / 60;
                            case Units.SECOND: return value / 60 / 60;
                        }
                        break;

                    case NativeUnits.INCH:
                        switch (nativeUnits)
                        {
                            case Units.MILLIMETER: return value / 25.4;
                            case NativeUnits.FOOT: return value * 12;
                        }
                        break;

                    case NativeUnits.INCH_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case Units.MILLIMETER_PER_SECOND: return value / 25.4 * 60;
                            case NativeUnits.FOOT_PER_MINUTE: return value * 12;
                            case NativeUnits.FOOT_PER_SECOND: return value * 12 / 60;
                            case NativeUnits.INCH_PER_SECOND: return value * 60;
                        }
                        break;

                    case NativeUnits.INCH_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case Units.MILLIMETER_PER_SECOND: return value / 25.4;
                            case NativeUnits.FOOT_PER_MINUTE: return value * 12 / 60;
                            case NativeUnits.FOOT_PER_SECOND: return value * 12;
                            case NativeUnits.INCH_PER_MINUTE: return value / 60;
                        }
                        break;

                    case NativeUnits.INCH_POUND:
                        switch (nativeUnits)
                        {
                            case Units.NEWTON_METER: return value / 8.85075;
                        }
                        break;

                    case Units.JOULE:
                        switch (nativeUnits)
                        {
                            case NativeUnits.KILOWATT_HOUR: return value * 3.78541178 / 60;
                        }
                        break;

                    case NativeUnits.KELVIN:
                        switch (nativeUnits)
                        {
                            case Units.CELSIUS: return value * 274.15;
                            case NativeUnits.FAHRENHEIT: return value * 255.927778;
                        }
                        break;

                    case Units.KILOGRAM:
                        switch (nativeUnits)
                        {
                            case Units.MILLIGRAM: return value / 1000000;
                            case NativeUnits.POUND: return value / 2.20462262;
                        }
                        break;

                    case NativeUnits.KILOWATT:
                        switch (nativeUnits)
                        {
                            case Units.WATT: return value / 1000;
                        }
                        break;

                    case NativeUnits.KILOWATT_HOUR:
                        switch (nativeUnits)
                        {
                            case Units.WATT_SECOND: return value / 3600000;
                        }
                        break;

                    case Units.LITER:
                        switch (nativeUnits)
                        {
                            case Units.MILLILITER: return value / 1000;
                        }
                        break;

                    case NativeUnits.LITER_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case NativeUnits.GALLON_PER_MINUTE: return value * 3.78541178;
                            case Units.LITER_PER_SECOND: return value * 60;
                        }
                        break;

                    case Units.LITER_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.GALLON_PER_MINUTE: return value * 3.78541178 / 60;
                            case NativeUnits.LITER_PER_MINUTE: return value / 60;
                        }
                        break;

                    case Units.MILLIGRAM:
                        switch (nativeUnits)
                        {
                            case Units.KILOGRAM: return value * 1000000;
                            case NativeUnits.POUND: return value / 2.20462262 * 1000000;
                        }
                        break;

                    case Units.MILLILITER:
                        switch (nativeUnits)
                        {
                            case Units.LITER: return value * 1000;
                        }
                        break;

                    case Units.MILLIMETER:
                        switch (nativeUnits)
                        {
                            case NativeUnits.FOOT: return value * 25.4 * 12;
                            case NativeUnits.INCH: return value * 25.4;
                        }
                        break;

                    case NativeUnits.MILLIMETER_MERCURY:
                        switch (nativeUnits)
                        {
                            case NativeUnits.BAR: return value * 750.06375542;
                            case Units.PASCAL: return value / 133.322;
                            case NativeUnits.POUND_PER_INCH_SQUARED: return value * 51.71507548;
                            case NativeUnits.TORR: return value * 51.714932572;
                        }
                        break;

                    case Units.MILLIMETER_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.INCH_PER_MINUTE: return value * 25.4 / 60;
                            case NativeUnits.INCH_PER_SECOND: return value * 25.4;
                            case NativeUnits.FOOT_PER_MINUTE: return value * 25.4 * 12 / 60;
                            case NativeUnits.FOOT_PER_SECOND: return value * 25.4 * 12;
                            case NativeUnits.MILLIMETER_PER_MINUTE: return value / 60;
                        }
                        break;

                    case NativeUnits.MILLIMETER_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case NativeUnits.INCH_PER_MINUTE: return value * 25.4;
                            case NativeUnits.INCH_PER_SECOND: return value * 25.4 * 60;
                            case NativeUnits.FOOT_PER_MINUTE: return value * 25.4 * 12;
                            case NativeUnits.FOOT_PER_SECOND: return value * 25.4 * 12 * 60;
                            case Units.MILLIMETER_PER_SECOND: return value * 60;
                        }
                        break;

                    case Units.MILLIMETER_PER_SECOND_SQUARED:
                        switch (nativeUnits)
                        {
                            case NativeUnits.INCH_PER_SECOND_SQUARED: return value * 25.4;
                            case NativeUnits.FOOT_PER_SECOND_SQUARED: return value * 25.4 * 12;
                        }
                        break;

                    case NativeUnits.MINUTE:
                        switch (nativeUnits)
                        {
                            case NativeUnits.HOUR: return value * 60;
                            case Units.SECOND: return value / 60;
                        }
                        break;

                    case Units.NEWTON:
                        switch (nativeUnits)
                        {
                            case NativeUnits.POUND: return value * 4.448221615254;
                        }
                        break;

                    case Units.NEWTON_METER:
                        switch (nativeUnits)
                        {
                            case NativeUnits.INCH_POUND: return value * 8.85075;
                        }
                        break;

                    case Units.PASCAL:
                        switch (nativeUnits)
                        {
                            case NativeUnits.BAR: return value * 100000;
                            case NativeUnits.MILLIMETER_MERCURY: return value / 133.322;
                            case NativeUnits.POUND_PER_INCH_SQUARED: return value * 6894.75729;
                            case NativeUnits.TORR: return value * 133.322368;
                        }
                        break;

                    case Units.PASCAL_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.PASCAL_PER_MINUTE: return value / 60;
                        }
                        break;

                    case NativeUnits.PASCAL_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case Units.PASCAL_PER_SECOND: return value * 60;
                        }
                        break;

                    case Units.PASCAL_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.CENTIPOISE: return value / 1000;
                        }
                        break;

                    case NativeUnits.POUND:
                        switch (nativeUnits)
                        {
                            case Units.MILLIGRAM: return value / 453592;
                            case Units.KILOGRAM: return value * 2.20462262;
                        }
                        break;

                    case NativeUnits.POUND_PER_INCH_SQUARED:
                        switch (nativeUnits)
                        {
                            case NativeUnits.BAR: return value * 14.503773773;
                            case NativeUnits.MILLIMETER_MERCURY: return value / 51.71507548;
                            case Units.PASCAL: return value / 6894.7572932;
                            case NativeUnits.TORR: return value / 51.714932572;
                        }
                        break;

                    case NativeUnits.RADIAN:
                        switch (nativeUnits)
                        {
                            case Units.DEGREE: return value / 57.2957795;
                        }
                        break;

                    case NativeUnits.RADIAN_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case NativeUnits.DEGREE_PER_MINUTE: return value / 57.2957795;
                            case Units.DEGREE_PER_SECOND: return value / 57.2957795 * 60;
                            case NativeUnits.RADIAN_PER_SECOND: return value * 60;
                            case Units.REVOLUTION_PER_MINUTE: return value * 6.28318531;
                            case Units.REVOLUTION_PER_SECOND: return value * 6.28318531 * 60;
                        }
                        break;

                    case NativeUnits.RADIAN_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.DEGREE_PER_MINUTE: return value / 57.2957795 / 60;
                            case Units.DEGREE_PER_SECOND: return value / 57.2957795;
                            case NativeUnits.RADIAN_PER_MINUTE: return value / 60;
                            case Units.REVOLUTION_PER_MINUTE: return value * 6.28318531 / 60;
                            case Units.REVOLUTION_PER_SECOND: return value * 6.28318531;
                        }
                        break;

                    case NativeUnits.RADIAN_PER_SECOND_SQUARED:
                        switch (nativeUnits)
                        {
                            case Units.DEGREE_PER_SECOND_SQUARED: return value / 57.2957795;
                            case Units.REVOLUTION_PER_SECOND_SQUARED: return value * 6.28318531;
                        }
                        break;

                    case Units.REVOLUTION_PER_MINUTE:
                        switch (nativeUnits)
                        {
                            case NativeUnits.DEGREE_PER_MINUTE: return value / 360;
                            case Units.DEGREE_PER_SECOND: return value / 360 * 60;
                            case NativeUnits.RADIAN_PER_MINUTE: return value / 6.28318531;
                            case NativeUnits.RADIAN_PER_SECOND: return value / 6.28318531 * 60;
                            case Units.REVOLUTION_PER_SECOND: return value * 60;
                        }
                        break;

                    case Units.REVOLUTION_PER_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.DEGREE_PER_MINUTE: return value / 360 / 60;
                            case Units.DEGREE_PER_SECOND: return value / 360;
                            case NativeUnits.RADIAN_PER_MINUTE: return value * 57.2957795 * 360 / 60;
                            case NativeUnits.RADIAN_PER_SECOND: return value * 57.2957795 * 360;
                            case Units.REVOLUTION_PER_MINUTE: return value / 60;
                        }
                        break;

                    case Units.REVOLUTION_PER_SECOND_SQUARED:
                        switch (nativeUnits)
                        {
                            case Units.DEGREE_PER_SECOND_SQUARED: return value / 360;
                            case NativeUnits.RADIAN_PER_SECOND_SQUARED: return value / 6.28318531;
                        }
                        break;

                    case Units.SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.HOUR: return value * 60 * 60;
                            case NativeUnits.MINUTE: return value * 60;
                        }
                        break;

                    case NativeUnits.TORR:
                        switch (nativeUnits)
                        {
                            case NativeUnits.BAR: return value * 750.0616827;
                            case NativeUnits.MILLIMETER_MERCURY: return value / 1.0000027634;
                            case Units.PASCAL: return value / 133.32236842;
                            case NativeUnits.POUND_PER_INCH_SQUARED: return value * 51.714932572;
                        }
                        break;

                    case Units.WATT:
                        switch (nativeUnits)
                        {
                            case NativeUnits.KILOWATT: return value * 1000;
                        }
                        break;

                    case Units.WATT_SECOND:
                        switch (nativeUnits)
                        {
                            case NativeUnits.KILOWATT_HOUR: return value * 3600000;
                        }
                        break;
                }
            }

            return value;
        }
    }
}
