// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static partial class Units
    {
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
