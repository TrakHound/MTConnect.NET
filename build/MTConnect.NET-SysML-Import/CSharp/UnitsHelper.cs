namespace MTConnect.SysML.CSharp
{
    internal static class UnitsHelper
    {
        public static string Get(string units)
        {
            if (units != null)
            {
                units = units.Replace("/", "_PER_");
                units = units.Replace("^2", "_SQUARED");
                //if (units.EndsWith("^3")) units = units.Replace("^3", "_CUBED");
            }

            return units;
        }
    }
}
