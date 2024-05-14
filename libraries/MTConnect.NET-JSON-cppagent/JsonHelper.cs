using System.Collections.Generic;
using System.Linq;

namespace MTConnect
{
    internal static class JsonHelper
    {
        public static IEnumerable<double> ToJsonArray(this UnitVector3D vector)
        {
            if (vector != null)
            {
                var values = new List<double>();
                values.Add(vector.X);
                values.Add(vector.Y);
                values.Add(vector.Z);
                return values;
            }

            return null;
        }

        public static IEnumerable<double> ToJsonArray(this Degree3D vector)
        {
            if (vector != null)
            {
                var values = new List<double>();
                values.Add(vector.A);
                values.Add(vector.B);
                values.Add(vector.C);
                return values;
            }

            return null;
        }

        public static UnitVector3D ToUnitVector3D(IEnumerable<double> values)
        {
            if (!values.IsNullOrEmpty())
            {
                var a = values.ToArray();
                if (a.Length > 2)
                {
                    return new UnitVector3D(a[0], a[1], a[2]);
                }
                else
                {
                    return new UnitVector3D(a[0], a[0], a[0]);
                }
            }

            return null;
        }

        public static Degree3D ToDegree3D(IEnumerable<double> values)
        {
            if (!values.IsNullOrEmpty())
            {
                var a = values.ToArray();
                if (a.Length > 2)
                {
                    return new Degree3D(a[0], a[1], a[2]);
                }
            }

            return null;
        }
    }
}
