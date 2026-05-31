// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace MTConnect
{
    /// <summary>
    /// A space-delimited, floating-point representation of the angular rotation in degrees around the X, Y, and Z axes relative
    /// to a cartesian coordinate system respectively in order as A, B, and C. If any of the rotations is not known, it MUST be zero (0).
    /// </summary>
    public class Degree3D
    {
        private static readonly Regex _regex = new Regex(@"([0-9\.]*) ([0-9\.]*) ([0-9\.]*)");


        /// <summary>
        /// The rotation in degrees about the X axis.
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// The rotation in degrees about the Y axis.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// The rotation in degrees about the Z axis.
        /// </summary>
        public double C { get; set; }


        /// <summary>
        /// Initializes an angular rotation from its three axis components in degrees.
        /// </summary>
        /// <param name="a">The rotation about the X axis.</param>
        /// <param name="b">The rotation about the Y axis.</param>
        /// <param name="c">The rotation about the Z axis.</param>
        public Degree3D(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Renders the rotation in the MTConnect space-separated "A B C" form.
        /// </summary>
        public override string ToString()
        {
            return $"{A} {B} {C}";
        }

        /// <summary>
        /// Parses a space-separated "A B C" string into an angular rotation; returns null when the input is empty or does not match the expected form.
        /// </summary>
        /// <param name="input">The space-separated rotation triple.</param>
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