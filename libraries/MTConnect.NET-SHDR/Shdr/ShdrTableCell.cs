// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    public class ShdrTableCell : TableCell
    {
        public ShdrTableCell() { }

        public ShdrTableCell(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        public ShdrTableCell(ITableCell cell)
        {
            if (cell != null)
            {
                Key = cell.Key;
                Value = cell.Value;
            }
        }


        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                return $"{Key}={Value}";
            }

            return "";
        }

        public static ShdrTableCell FromString(string segment)
        {
            if (!string.IsNullOrEmpty(segment))
            {
                var regex = new Regex("(.*)=(.*)");
                var match = regex.Match(segment);
                if (match.Success && match.Groups.Count > 2)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;

                    return new ShdrTableCell(key, value);
                }
            }

            return null;
        }
    }
}