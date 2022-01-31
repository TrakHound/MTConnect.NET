// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.RegularExpressions;

namespace MTConnect.Streams
{

    public class Position3D
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }


        public Position3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        public Position3D FromString(string input)
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

                    return new Position3D(x, y, z);
                }
            }

            return null;
        }
    }
}
