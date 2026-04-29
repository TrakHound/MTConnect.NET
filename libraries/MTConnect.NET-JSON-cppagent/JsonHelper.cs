using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MTConnect
{
    internal static class JsonHelper
    {
        public static IEnumerable<double> ToJsonArray(IAxis axis)
        {
            return ParseValues(axis?.Value);
        }

        public static IEnumerable<double> ToJsonArray(IOrigin origin)
        {
            return ParseValues(origin?.Value);
        }

        public static IEnumerable<double> ToJsonArray(IRotation rotation)
        {
            return ParseValues(rotation?.Value);
        }

        public static IEnumerable<double> ToJsonArray(IScale scale)
        {
            return ParseValues(scale?.Value);
        }

        public static IEnumerable<double> ToJsonArray(ITranslation translation)
        {
            return ParseValues(translation?.Value);
        }

        public static IAxis ToAxis(IEnumerable<double> values)
        {
            var text = JoinValues(values);
            if (text == null) return null;
            return new Axis { Value = text };
        }

        public static IOrigin ToOrigin(IEnumerable<double> values)
        {
            var text = JoinValues(values);
            if (text == null) return null;
            return new Origin { Value = text };
        }

        public static IRotation ToRotation(IEnumerable<double> values)
        {
            var text = JoinValues(values);
            if (text == null) return null;
            return new Rotation { Value = text };
        }

        public static IScale ToScale(IEnumerable<double> values)
        {
            var text = JoinValues(values);
            if (text == null) return null;
            return new Scale { Value = text };
        }

        public static ITranslation ToTranslation(IEnumerable<double> values)
        {
            var text = JoinValues(values);
            if (text == null) return null;
            return new Translation { Value = text };
        }

        private static IEnumerable<double> ParseValues(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var parts = text.Split(' ');
            var values = new List<double>();
            foreach (var p in parts)
            {
                if (double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                {
                    values.Add(v);
                }
            }
            return values.Count > 0 ? values : null;
        }

        private static string JoinValues(IEnumerable<double> values)
        {
            if (values.IsNullOrEmpty()) return null;
            return string.Join(" ", values.Select(v => v.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
