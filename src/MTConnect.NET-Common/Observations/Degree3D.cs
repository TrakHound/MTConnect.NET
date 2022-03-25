// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.RegularExpressions;

namespace MTConnect.Observations
{
    /// <summary>
    /// A space-delimited, floating-point representation of the angular rotation in degrees around the X, Y, and Z axes relative
    /// to a cartesian coordinate system respectively in order as A, B, and C. If any of the rotations is not known, it MUST be zero (0).
    /// </summary>
    public class Degree3D
    {
        private static readonly Regex _regex = new Regex(@"([0-9\.]*) ([0-9\.]*) ([0-9\.]*)");


        public double A { get; set; }

        public double B { get; set; }

        public double C { get; set; }


        public Degree3D(double a, double b, double c)
        {
            A = a;
            B = b;  
            C = c;
        }

        public override string ToString()
        {
            return $"{A} {B} {C}";
        }

        public static Degree3D FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var match = _regex.Match(input);
                if (match.Success)
                {
                    double a = 0;
                    double b = 0;
                    double c = 0;

                    if (match.Groups[1].Success) a = match.Groups[1].Value.ToDouble();
                    if (match.Groups[2].Success) b = match.Groups[2].Value.ToDouble();
                    if (match.Groups[3].Success) c = match.Groups[3].Value.ToDouble();

                    return new Degree3D(a, b, c);
                }
            }

            return null;
        }
    }
}
