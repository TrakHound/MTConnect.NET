// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class NativeUnitsDescriptions
    {
        /// <summary>
        /// Electric charge in ampere hour.
        /// </summary>
        public const string AMPERE_HOUR = "Electric charge in ampere hour.";
        
        /// <summary>
        /// Pressure in Bar.
        /// </summary>
        public const string BAR = "Pressure in Bar.";
        
        /// <summary>
        /// Measure of viscosity.
        /// </summary>
        public const string CENTIPOISE = "Measure of viscosity.";
        
        /// <summary>
        /// Rotational velocity in degrees per minute.
        /// </summary>
        public const string DEGREE_PER_MINUTE = "Rotational velocity in degrees per minute.";
        
        /// <summary>
        /// Temperature in Fahrenheit.
        /// </summary>
        public const string FAHRENHEIT = "Temperature in Fahrenheit.";
        
        /// <summary>
        /// Feet.
        /// </summary>
        public const string FOOT = "Feet.";
        
        /// <summary>
        /// Point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in feet.
        /// </summary>
        public const string FOOT_3D = "Point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in feet.";
        
        /// <summary>
        /// Feet per minute.
        /// </summary>
        public const string FOOT_PER_MINUTE = "Feet per minute.";
        
        /// <summary>
        /// Feet per second.
        /// </summary>
        public const string FOOT_PER_SECOND = "Feet per second.";
        
        /// <summary>
        /// Acceleration in feet per second squared.
        /// </summary>
        public const string FOOT_PER_SECOND_SQUARED = "Acceleration in feet per second squared.";
        
        /// <summary>
        /// Gallons per minute.
        /// </summary>
        public const string GALLON_PER_MINUTE = "Gallons per minute.";
        
        /// <summary>
        /// Acceleration relative to earth's gravity given in `METER/SECOND^2`.> Note 1 to entry: At different points on Earth's surface, the free-fall acceleration ranges from 9.764 to 9.834 m/s2 (Wikipedia: Gravitational Acceleration). The constant can be customized depending on the location in the universe.> Note 2 to entry: In the standard, it is assumed that Earth's average value of gravitational acceleration is 9.90665 m/s2.
        /// </summary>
        public const string GRAVITATIONAL_ACCELERATION = "Acceleration relative to earth's gravity given in `METER/SECOND^2`.> Note 1 to entry: At different points on Earth's surface, the free-fall acceleration ranges from 9.764 to 9.834 m/s2 (Wikipedia: Gravitational Acceleration). The constant can be customized depending on the location in the universe.> Note 2 to entry: In the standard, it is assumed that Earth's average value of gravitational acceleration is 9.90665 m/s2.";
        
        /// <summary>
        /// $$mass\times GRAVITATIONAL_ACCELERATION$$ (g) given in `METER/SECOND^2`.
        /// </summary>
        public const string GRAVITATIONAL_FORCE = "$$mass\times GRAVITATIONAL_ACCELERATION$$ (g) given in `METER/SECOND^2`.";
        
        /// <summary>
        /// Measurement of time in hours.
        /// </summary>
        public const string HOUR = "Measurement of time in hours.";
        
        /// <summary>
        /// Inches.
        /// </summary>
        public const string INCH = "Inches.";
        
        /// <summary>
        /// Point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in inches.
        /// </summary>
        public const string INCH_3D = "Point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in inches.";
        
        /// <summary>
        /// Measure of torque in inch pounds.
        /// </summary>
        public const string INCH_POUND = "Measure of torque in inch pounds.";
        
        /// <summary>
        /// Inches per minute.
        /// </summary>
        public const string INCH_PER_MINUTE = "Inches per minute.";
        
        /// <summary>
        /// Inches per second.
        /// </summary>
        public const string INCH_PER_SECOND = "Inches per second.";
        
        /// <summary>
        /// Acceleration in inches per second squared.
        /// </summary>
        public const string INCH_PER_SECOND_SQUARED = "Acceleration in inches per second squared.";
        
        /// <summary>
        /// Measurement of temperature.
        /// </summary>
        public const string KELVIN = "Measurement of temperature.";
        
        /// <summary>
        /// Measurement in kilowatt.
        /// </summary>
        public const string KILOWATT = "Measurement in kilowatt.";
        
        /// <summary>
        /// Kilowatt hours which is 3.6 mega joules.
        /// </summary>
        public const string KILOWATT_HOUR = "Kilowatt hours which is 3.6 mega joules.";
        
        /// <summary>
        /// Measurement of rate of flow of a fluid.
        /// </summary>
        public const string LITER_PER_MINUTE = "Measurement of rate of flow of a fluid.";
        
        /// <summary>
        /// Pressure in Millimeter of Mercury (mmHg).
        /// </summary>
        public const string MILLIMETER_MERCURY = "Pressure in Millimeter of Mercury (mmHg).";
        
        /// <summary>
        /// Velocity in millimeters per minute.
        /// </summary>
        public const string MILLIMETER_PER_MINUTE = "Velocity in millimeters per minute.";
        
        /// <summary>
        /// Measurement of time in minutes.
        /// </summary>
        public const string MINUTE = "Measurement of time in minutes.";
        
        /// <summary>
        /// Unsupported unit.
        /// </summary>
        public const string OTHER = "Unsupported unit.";
        
        /// <summary>
        /// Pascal per minute.
        /// </summary>
        public const string PASCAL_PER_MINUTE = "Pascal per minute.";
        
        /// <summary>
        /// Us pounds.
        /// </summary>
        public const string POUND = "Us pounds.";
        
        /// <summary>
        /// Pressure in pounds per square inch (PSI).
        /// </summary>
        public const string POUND_PER_INCH_SQUARED = "Pressure in pounds per square inch (PSI).";
        
        /// <summary>
        /// Angle in radians.
        /// </summary>
        public const string RADIAN = "Angle in radians.";
        
        /// <summary>
        /// Velocity in radians per minute.
        /// </summary>
        public const string RADIAN_PER_MINUTE = "Velocity in radians per minute.";
        
        /// <summary>
        /// Rotational acceleration in radian per second squared.
        /// </summary>
        public const string RADIAN_PER_SECOND = "Rotational acceleration in radian per second squared.";
        
        /// <summary>
        /// Rotational acceleration in radian per second squared.
        /// </summary>
        public const string RADIAN_PER_SECOND_SQUARED = "Rotational acceleration in radian per second squared.";
        
        /// <summary>
        /// Pressure in Torr.
        /// </summary>
        public const string TORR = "Pressure in Torr.";


        public static string Get(string value)
        {
            switch (value)
            {
                case NativeUnits.AMPERE_HOUR: return "Electric charge in ampere hour.";
                case NativeUnits.BAR: return "Pressure in Bar.";
                case NativeUnits.CENTIPOISE: return "Measure of viscosity.";
                case NativeUnits.DEGREE_PER_MINUTE: return "Rotational velocity in degrees per minute.";
                case NativeUnits.FAHRENHEIT: return "Temperature in Fahrenheit.";
                case NativeUnits.FOOT: return "Feet.";
                case NativeUnits.FOOT_3D: return "Point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in feet.";
                case NativeUnits.FOOT_PER_MINUTE: return "Feet per minute.";
                case NativeUnits.FOOT_PER_SECOND: return "Feet per second.";
                case NativeUnits.FOOT_PER_SECOND_SQUARED: return "Acceleration in feet per second squared.";
                case NativeUnits.GALLON_PER_MINUTE: return "Gallons per minute.";
                case NativeUnits.GRAVITATIONAL_ACCELERATION: return "Acceleration relative to earth's gravity given in `METER/SECOND^2`.> Note 1 to entry: At different points on Earth's surface, the free-fall acceleration ranges from 9.764 to 9.834 m/s2 (Wikipedia: Gravitational Acceleration). The constant can be customized depending on the location in the universe.> Note 2 to entry: In the standard, it is assumed that Earth's average value of gravitational acceleration is 9.90665 m/s2.";
                case NativeUnits.GRAVITATIONAL_FORCE: return "$$mass\times GRAVITATIONAL_ACCELERATION$$ (g) given in `METER/SECOND^2`.";
                case NativeUnits.HOUR: return "Measurement of time in hours.";
                case NativeUnits.INCH: return "Inches.";
                case NativeUnits.INCH_3D: return "Point in space identified by X, Y, and Z positions and represented by a space-delimited set of numbers each expressed in inches.";
                case NativeUnits.INCH_POUND: return "Measure of torque in inch pounds.";
                case NativeUnits.INCH_PER_MINUTE: return "Inches per minute.";
                case NativeUnits.INCH_PER_SECOND: return "Inches per second.";
                case NativeUnits.INCH_PER_SECOND_SQUARED: return "Acceleration in inches per second squared.";
                case NativeUnits.KELVIN: return "Measurement of temperature.";
                case NativeUnits.KILOWATT: return "Measurement in kilowatt.";
                case NativeUnits.KILOWATT_HOUR: return "Kilowatt hours which is 3.6 mega joules.";
                case NativeUnits.LITER_PER_MINUTE: return "Measurement of rate of flow of a fluid.";
                case NativeUnits.MILLIMETER_MERCURY: return "Pressure in Millimeter of Mercury (mmHg).";
                case NativeUnits.MILLIMETER_PER_MINUTE: return "Velocity in millimeters per minute.";
                case NativeUnits.MINUTE: return "Measurement of time in minutes.";
                case NativeUnits.OTHER: return "Unsupported unit.";
                case NativeUnits.PASCAL_PER_MINUTE: return "Pascal per minute.";
                case NativeUnits.POUND: return "Us pounds.";
                case NativeUnits.POUND_PER_INCH_SQUARED: return "Pressure in pounds per square inch (PSI).";
                case NativeUnits.RADIAN: return "Angle in radians.";
                case NativeUnits.RADIAN_PER_MINUTE: return "Velocity in radians per minute.";
                case NativeUnits.RADIAN_PER_SECOND: return "Rotational acceleration in radian per second squared.";
                case NativeUnits.RADIAN_PER_SECOND_SQUARED: return "Rotational acceleration in radian per second squared.";
                case NativeUnits.TORR: return "Pressure in Torr.";
            }

            return null;
        }
    }
}