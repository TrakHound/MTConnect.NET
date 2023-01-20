// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// The engineering units used by the information source
    /// </summary>
    public static class NativeUnitAbbreviations
    {
        public static string Get(string units)
        {
            switch (units)
            {
                case NativeUnits.BAR: return "bar";
                case NativeUnits.DEGREE_PER_MINUTE: return "°/min";
                case NativeUnits.FAHRENHEIT: return "°F";
                case NativeUnits.FOOT: return "ft";
                case NativeUnits.FOOT_PER_MINUTE: return "fpm";
                case NativeUnits.FOOT_PER_SECOND: return "ft/s";
                case NativeUnits.FOOT_PER_SECOND_SQUARED: return "ft/s^2";
                case NativeUnits.FOOT_3D: return "ft 3D";
                case NativeUnits.GALLON_PER_MINUTE: return "GPM";
                case NativeUnits.HOUR: return "hr";
                case NativeUnits.INCH: return "in";
                case NativeUnits.INCH_PER_MINUTE: return "ipm";
                case NativeUnits.INCH_PER_SECOND: return "in/s";
                case NativeUnits.INCH_PER_SECOND_SQUARED: return "in/s^2";
                case NativeUnits.INCH_3D: return "in 3D";
                case NativeUnits.INCH_POUND: return "in-lb";
                case NativeUnits.KELVIN: return "K";
                case NativeUnits.KILOWATT: return "kW";
                case NativeUnits.KILOWATT_HOUR: return "kWh";
                case NativeUnits.LITER: return "L";
                case NativeUnits.MINUTE: return "min";
                case NativeUnits.PASCAL_PER_MINUTE: return "Pas/min";
                case NativeUnits.POUND: return "lb";
                case NativeUnits.POUND_PER_INCH_SQUARED: return "PSI";
                case NativeUnits.RADIAN: return "rad";
                case NativeUnits.RADIAN_PER_MINUTE: return "rad/min";
                case NativeUnits.RADIAN_PER_SECOND: return "rad/s";
                case NativeUnits.RADIAN_PER_SECOND_SQUARED: return "rad/s^2";
                case NativeUnits.TORR: return "Torr";
            }

            return "";
        }
    }
}