// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Units that are defined as the standard unit of measure for each type of DataItem
    /// </summary>
    public static class UnitAbbreviations
    {
        public static string Get(string units)
        {
            switch (units)
            {
                case Units.AMPERE: return "Amps";
                case Units.CELSIUS: return "°C";
                case Units.COULOMB: return "C";
                case Units.COUNT: return "#";
                case Units.COUNT_PER_SECOND: return "#/s";
                case Units.CUBIC_MILLIMETER: return "mm^3";
                case Units.CUBIC_MILLIMETER_PER_SECOND: return "mm^3/s";
                case Units.CUBIC_MILLIMETER_PER_SECOND_SQUARED: return "mm^3/s^2";
                case Units.DECIBEL: return "dB";
                case Units.DEGREE: return "°";
                case Units.DEGREE_PER_SECOND: return "°/s";
                case Units.DEGREE_PER_SECOND_SQUARED: return "°/s^2";
                case Units.DEGREE_3D: return "° 3D";
                case Units.GRAM_PER_CUBIC_METER: return "g/m^3";
                case Units.HERTZ: return "Hz";
                case Units.JOULE: return "J";
                case Units.KILOGRAM: return "Kg";
                case Units.LITER: return "L";
                case Units.MICRO_RADIAN: return "μrad";
                case Units.MILLIGRAM: return "mg";
                case Units.MILLIGRAM_PER_CUBIC_MILLIMETER: return "mg/mm^3";
                case Units.MILLIMETER: return "mm";
                case Units.MILLIMETER_PER_REVOLUTION: return "mm/rev";
                case Units.MILLIMETER_PER_SECOND: return "mm/s";
                case Units.MILLIMETER_PER_SECOND_SQUARED: return "mm/s^2";
                case Units.MILLIMETER_3D: return "mm 3D";
                case Units.NEWTON: return "N";
                case Units.NEWTON_METER: return "Nm";
                case Units.OHM: return "Ω";
                case Units.PASCAL: return "Pa";
                case Units.PASCAL_PER_SECOND: return "Pa/s";
                case Units.PASCAL_SECOND: return "Pas";
                case Units.PERCENT: return "%";
                case Units.PH: return "pH";
                case Units.REVOLUTION_PER_MINUTE: return "RPM";
                case Units.SECOND: return "s";
                case Units.SIEMENS_PER_METER: return "S/m";
                case Units.UNIT_VECTOR_3D: return "3D Vector";
                case Units.VOLT: return "V";
                case Units.VOLT_AMPERE: return "VA";
                case Units.VOLT_AMPERE_REACTIVE: return "var";
                case Units.WATT: return "W";
                case Units.WATT_SECOND: return "Ws";
            }

            return "";
        }
    }
}