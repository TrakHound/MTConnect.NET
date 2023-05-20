// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.Devices
{
    /// <summary>
    /// The engineering units used by the information source
    /// </summary>
    public static class NativeUnits
    {
        /// <summary>
        /// Electric charge in ampere hour.
        /// </summary>
        public const string AMPERE_HOUR = "AMPERE_HOUR";

        /// <summary>
        /// Pressure in Bar
        /// </summary>
        public const string BAR = "BAR";

        /// <summary>
        /// A measure of Viscosity
        /// </summary>
        public const string CENTIPOISE = "CENTIPOISE";

        /// <summary>
        /// Rotational velocity in degrees per minute
        /// </summary>
        public const string DEGREE_PER_MINUTE = "DEGREE_PER_MINUTE";

        /// <summary>
        /// Temperature in Fahrenheit
        /// </summary>
        public const string FAHRENHEIT = "FAHRENHEIT";

        /// <summary>
        /// Feet
        /// </summary>
        public const string FOOT = "FOOT";

        /// <summary>
        /// Feet per minute
        /// </summary>
        public const string FOOT_PER_MINUTE = "FOOT_PER_MINUTE";

        /// <summary>
        /// Feet per second
        /// </summary>
        public const string FOOT_PER_SECOND = "FOOT_PER_SECOND";

        /// <summary>
        /// Acceleration in feet per second squared
        /// </summary>
        public const string FOOT_PER_SECOND_SQUARED = "FOOT_PER_SECOND_SQUARED";

        /// <summary>
        /// A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in feet.
        /// </summary>
        public const string FOOT_3D = "FOOT_3D";

        /// <summary>
        /// Gallons per minute
        /// </summary>
        public const string GALLON_PER_MINUTE = "GALLON_PER_MINUTE";

        /// <summary>
        /// Acceleration relative to earth’s gravity given in METER/SECOND^2.
        /// </summary>
        public const string GRAVITATIONAL_ACCELERATION = "GRAVITATIONAL_ACCELERATION";

        /// <summary>
        /// Force relative to earth’s gravity.
        /// </summary>
        public const string GRAVITATIONAL_FORCE = "GRAVITATIONAL_FORCE";

        /// <summary>
        /// A measurement of time in hours
        /// </summary>
        public const string HOUR = "HOUR";

        /// <summary>
        /// Inches
        /// </summary>
        public const string INCH = "INCH";

        /// <summary>
        /// Inches per minute
        /// </summary>
        public const string INCH_PER_MINUTE = "INCH_PER_MINUTE";

        /// <summary>
        /// Inches per second
        /// </summary>
        public const string INCH_PER_SECOND = "INCH_PER_SECOND";

        /// <summary>
        /// Acceleration in inches per second squared
        /// </summary>
        public const string INCH_PER_SECOND_SQUARED = "INCH_PER_SECOND_SQUARED";

        /// <summary>
        /// A point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in inches.
        /// </summary>
        public const string INCH_3D = "INCH_3D";

        /// <summary>
        /// A measure of torque in inch pounds.
        /// </summary>
        public const string INCH_POUND = "INCH_POUND";

        /// <summary>
        /// A measurement of temperature
        /// </summary>
        public const string KELVIN = "KELVIN";

        /// <summary>
        /// A measurement in kilowatt
        /// </summary>
        public const string KILOWATT = "KILOWATT";

        /// <summary>
        /// Kilowatt hours which is 3.6 mega joules
        /// </summary>
        public const string KILOWATT_HOUR = "KILOWATT_HOUR";

        /// <summary>
        /// Measurement of volume of a fluid
        /// </summary>
        public const string LITER = "LITER";

        /// <summary>
        /// Measurement of rate of flow of a fluid
        /// </summary>
        public const string LITER_PER_MINUTE = "LITER_PER_MINUTE";

        /// <summary>
        /// Velocity in millimeters per minute
        /// </summary>
        public const string MILLIMETER_PER_MINUTE = "MILLIMETER_PER_MINUTE";

        /// <summary>
        /// Pressure in Millimeter of Mercury (mmHg)
        /// </summary>
        public const string MILLIMETER_MERCURY = "MILLIMETER_MERCURY";

        /// <summary>
        /// A measurement of time in minutes
        /// </summary>
        public const string MINUTE = "MINUTE";

        /// <summary>
        /// Unsupported Units
        /// </summary>
        public const string OTHER = "OTHER";

        /// <summary>
        /// Pascal per minute
        /// </summary>
        public const string PASCAL_PER_MINUTE = "PASCAL_PER_MINUTE";

        /// <summary>
        /// US pounds
        /// </summary>
        public const string POUND = "POUND";

        /// <summary>
        /// Pressure in pounds per square inch (PSI)
        /// </summary>
        public const string POUND_PER_INCH_SQUARED = "POUND_PER_INCH_SQUARED";

        /// <summary>
        /// Andle in radians
        /// </summary>
        public const string RADIAN = "RADIAN";

        /// <summary>
        /// Velocity in radians per minute
        /// </summary>
        public const string RADIAN_PER_MINUTE = "RADIAN_PER_MINUTE";

        /// <summary>
        /// Rotational acceleration in radian per second squared
        /// </summary>
        public const string RADIAN_PER_SECOND = "RADIAN_PER_SECOND";

        /// <summary>
        /// Rotational acceleration in radian per second squared
        /// </summary>
        public const string RADIAN_PER_SECOND_SQUARED = "RADIAN_PER_SECOND_SQUARED";

        /// <summary>
        /// Rotational velocity in revolution per second
        /// </summary>
        public const string REVOLUTION_PER_SECOND = "REVOLUTION_PER_SECOND";

        /// <summary>
        /// Pressure in Torr.
        /// </summary>
        public const string TORR = "TORR";


        public static IEnumerable<string> Get()
        {
            return typeof(NativeUnits)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue())
            .ToList();
        }
    }
}