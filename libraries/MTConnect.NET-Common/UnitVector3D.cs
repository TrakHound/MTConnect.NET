// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace MTConnect
{
    /// <summary>
    /// A 3D Unit Vector.
    /// </summary>
    public class UnitVector3D
    {
        /// <summary>
        /// The X component of the unit vector.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y component of the unit vector.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The Z component of the unit vector.
        /// </summary>
        public double Z { get; set; }


        /// <summary>
        /// Initializes a unit vector from its three components (the caller is responsible for supplying a normalized vector).
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public UnitVector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Renders the vector in the MTConnect space-separated "X Y Z" form.
        /// </summary>
        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        /// <summary>
        /// Parses a space-separated "X Y Z" string into a unit vector; returns null when the input is empty or does not match the expected form.
        /// </summary>
        /// <param name="input">The space-separated component triple.</param>
        public static UnitVector3D FromString(string input)
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