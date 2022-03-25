// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// The engineering units used by the information source
    /// </summary>
    public static class NativeUnitDescriptions
    {
        /// <summary>
        /// Pressure in Bar
        /// </summary>
        public const string BAR = "Pressure in Bar";

        /// <summary>
        /// A measure of Viscosity
        /// </summary>
        public const string CENTIPOISE = "A measure of Viscosity";

        /// <summary>
        /// Rotational velocity in degrees per minute
        /// </summary>
        public const string DEGREE_PER_MINUTE = "Rotational velocity in degrees per minute";

        /// <summary>
        /// Temperature in Fahrenheit
        /// </summary>
        public const string FAHRENHEIT = "Temperature in Fahrenheit";

        /// <summary>
        /// Feet
        /// </summary>
        public const string FOOT = "Feet";

        /// <summary>
        /// Feet per minute
        /// </summary>
        public const string FOOT_PER_MINUTE = "Feet per minute";

        /// <summary>
        /// Feet per second
        /// </summary>
        public const string FOOT_PER_SECOND = "Feet per second";

        /// <summary>
        /// Acceleration in feet per second squared
        /// </summary>
        public const string FOOT_PER_SECOND_SQUARED = "Acceleration in feet per second squared";

        /// <summary>
        /// A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in feet.
        /// </summary>
        public const string FOOT_3D = "A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in feet.";

        /// <summary>
        /// Gallons per minute
        /// </summary>
        public const string GALLON_PER_MINUTE = "Gallons per minute";

        /// <summary>
        /// A measurement of time in hours
        /// </summary>
        public const string HOUR = "A measurement of time in hours";

        /// <summary>
        /// Inches
        /// </summary>
        public const string INCH = "Inches";

        /// <summary>
        /// Inches per minute
        /// </summary>
        public const string INCH_PER_MINUTE = "Inches per minute";

        /// <summary>
        /// Inches per second
        /// </summary>
        public const string INCH_PER_SECOND = "Inches per second";

        /// <summary>
        /// Acceleration in inches per second squared
        /// </summary>
        public const string INCH_PER_SECOND_SQUARED = "Acceleration in inches per second squared";

        /// <summary>
        /// A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in inches.
        /// </summary>
        public const string INCH_3D = "A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in inches.";

        /// <summary>
        /// A measure of torque in inch pounds.
        /// </summary>
        public const string INCH_POUND = "A measure of torque in inch pounds.";

        /// <summary>
        /// A measurement of temperature
        /// </summary>
        public const string KELVIN = "A measurement of temperature";

        /// <summary>
        /// A measurement in kilowatt
        /// </summary>
        public const string KILOWATT = "A measurement in kilowatt";

        /// <summary>
        /// Kilowatt hours which is 3.6 mega joules
        /// </summary>
        public const string KILOWATT_HOUR = "Kilowatt hours which is 3.6 mega joules";

        /// <summary>
        /// Measurement of volume of a fluid
        /// </summary>
        public const string LITER = "Measurement of volume of a fluid";

        /// <summary>
        /// Measurement of rate of flow of a fluid
        /// </summary>
        public const string LITER_PER_MINUTE = "Measurement of rate of flow of a fluid";

        /// <summary>
        /// Velocity in millimeters per minute
        /// </summary>
        public const string MILLIMETER_PER_MINUTE = "Velocity in millimeters per minute";

        /// <summary>
        /// Pressure in Millimeter of Mercury (mmHg)
        /// </summary>
        public const string MILLIMETER_MERCURY = "Pressure in Millimeter of Mercury (mmHg)";

        /// <summary>
        /// A measurement of time in minutes
        /// </summary>
        public const string MINUTE = "A measurement of time in minutes";

        /// <summary>
        /// Unsupported Units
        /// </summary>
        public const string OTHER = "Unsupported Units";

        /// <summary>
        /// Pascal per minute
        /// </summary>
        public const string PASCAL_PER_MINUTE = "Pascal per minute";

        /// <summary>
        /// US pounds
        /// </summary>
        public const string POUND = "US pounds";

        /// <summary>
        /// Pressure in pounds per square inch (PSI)
        /// </summary>
        public const string POUND_PER_INCH_SQUARED = "Pressure in pounds per square inch (PSI)";

        /// <summary>
        /// Angle in radians
        /// </summary>
        public const string RADIAN = "Angle in radians";

        /// <summary>
        /// Velocity in radians per minute
        /// </summary>
        public const string RADIAN_PER_MINUTE = "Velocity in radians per minute";

        /// <summary>
        /// Rotational acceleration in radian per second squared
        /// </summary>
        public const string RADIAN_PER_SECOND = "Rotational acceleration in radian per second squared";

        /// <summary>
        /// Rotational acceleration in radian per second squared
        /// </summary>
        public const string RADIAN_PER_SECOND_SQUARED = "Rotational acceleration in radian per second squared";

        /// <summary>
        /// Rotational velocity in revolution per second
        /// </summary>
        public const string REVOLUTION_PER_SECOND = "Rotational velocity in revolution per second";

        /// <summary>
        /// Pressure in Torr.
        /// </summary>
        public const string TORR = "Pressure in Torr";


        public static string Get(string units)
        {
            switch (units)
            {
                case NativeUnits.BAR: return BAR;
                case NativeUnits.DEGREE_PER_MINUTE: return DEGREE_PER_MINUTE;
                case NativeUnits.FAHRENHEIT: return FAHRENHEIT;
                case NativeUnits.FOOT: return FOOT;
                case NativeUnits.FOOT_PER_MINUTE: return FOOT_PER_MINUTE;
                case NativeUnits.FOOT_PER_SECOND: return FOOT_PER_SECOND;
                case NativeUnits.FOOT_PER_SECOND_SQUARED: return FOOT_PER_SECOND_SQUARED;
                case NativeUnits.FOOT_3D: return FOOT_3D;
                case NativeUnits.GALLON_PER_MINUTE: return GALLON_PER_MINUTE;
                case NativeUnits.HOUR: return HOUR;
                case NativeUnits.INCH: return INCH;
                case NativeUnits.INCH_PER_MINUTE: return INCH_PER_MINUTE;
                case NativeUnits.INCH_PER_SECOND: return INCH_PER_SECOND;
                case NativeUnits.INCH_PER_SECOND_SQUARED: return INCH_PER_SECOND_SQUARED;
                case NativeUnits.INCH_3D: return INCH_3D;
                case NativeUnits.INCH_POUND: return INCH_POUND;
                case NativeUnits.KELVIN: return KELVIN;
                case NativeUnits.KILOWATT: return KILOWATT;
                case NativeUnits.KILOWATT_HOUR: return KILOWATT_HOUR;
                case NativeUnits.LITER: return LITER;
                case NativeUnits.MINUTE: return MINUTE;
                case NativeUnits.PASCAL_PER_MINUTE: return PASCAL_PER_MINUTE;
                case NativeUnits.POUND: return POUND;
                case NativeUnits.POUND_PER_INCH_SQUARED: return POUND_PER_INCH_SQUARED;
                case NativeUnits.RADIAN: return RADIAN;
                case NativeUnits.RADIAN_PER_MINUTE: return RADIAN_PER_MINUTE;
                case NativeUnits.RADIAN_PER_SECOND: return RADIAN_PER_SECOND;
                case NativeUnits.RADIAN_PER_SECOND_SQUARED: return RADIAN_PER_SECOND_SQUARED;
                case NativeUnits.TORR: return TORR;
            }

            return "";
        }
    }
}
