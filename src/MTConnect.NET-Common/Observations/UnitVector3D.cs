// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace MTConnect.Observations
{
    /// <summary>
    /// A 3D Unit Vector.
    /// </summary>
    public class UnitVector3D
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }


        public UnitVector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        public UnitVector3D FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var regex = new Regex(@"([0-9\.]*) ([0-9\.]*) ([0-9\.]*)");
                var match = regex.Match(input);
                if (match.Success)
                {
                    double x = 0;
                    double y = 0;
                    double z = 0;

                    if (match.Groups[1].Success) x = match.Groups[1].Value.ToDouble();
                    if (match.Groups[2].Success) y = match.Groups[2].Value.ToDouble();
                    if (match.Groups[3].Success) z = match.Groups[3].Value.ToDouble();

                    return new UnitVector3D(x, y, z);
                }
            }

            return null;
        }
    }
}