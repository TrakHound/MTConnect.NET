// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace MTConnect
{
    /// <summary>
    /// A three-dimensional displacement expressed in millimetres, serialized as the MTConnect space-separated "X Y Z" string form.
    /// </summary>
    public class Millimeter3D
    {
        private static readonly Regex _regex = new Regex(@"([0-9\.]*) ([0-9\.]*) ([0-9\.]*)");


        /// <summary>
        /// The X displacement in millimetres.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y displacement in millimetres.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The Z displacement in millimetres.
        /// </summary>
        public double Z { get; set; }


        /// <summary>
        /// Initializes a millimetre displacement from its three components.
        /// </summary>
        /// <param name="x">The X displacement in millimetres.</param>
        /// <param name="y">The Y displacement in millimetres.</param>
        /// <param name="z">The Z displacement in millimetres.</param>
        public Millimeter3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Renders the displacement in the MTConnect space-separated "X Y Z" form.
        /// </summary>
        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        /// <summary>
        /// Parses a space-separated "X Y Z" string into a millimetre displacement; returns null when the input is empty or does not match the expected form.
        /// </summary>
        /// <param name="input">The space-separated component triple.</param>
        public static Millimeter3D FromString(string input)
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

                    return new Millimeter3D(x, y, z);
                }
            }

            return null;
        }
    }
}