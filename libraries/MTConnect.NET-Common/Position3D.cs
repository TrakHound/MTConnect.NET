// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace MTConnect
{
    /// <summary>
    /// A Cartesian position in three dimensions, serialized as the MTConnect space-separated "X Y Z" string form.
    /// </summary>
    public class Position3D
    {
        private static readonly Regex _regex = new Regex(@"([0-9\.]*) ([0-9\.]*) ([0-9\.]*)");


        /// <summary>
        /// The X coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public double Z { get; set; }


        /// <summary>
        /// Initializes a position from its three coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public Position3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Renders the position in the MTConnect space-separated "X Y Z" form.
        /// </summary>
        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        /// <summary>
        /// Parses a space-separated "X Y Z" string into a position; returns null when the input is empty or does not match the expected form.
        /// </summary>
        /// <param name="input">The space-separated coordinate triple.</param>
        public static Position3D FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var match = _regex.Match(input);
                if (match.Success)
                {
                    double x = 0;
                    double y = 0;
                    double z = 0;

                    if (match.Groups[1].Success) x = match.Groups[1].Value.ToDouble();
                    if (match.Groups[2].Success) y = match.Groups[2].Value.ToDouble();
                    if (match.Groups[3].Success) z = match.Groups[3].Value.ToDouble();

                    return new Position3D(x, y, z);
                }
            }

            return null;
        }
    }
}