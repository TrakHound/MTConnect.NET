// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.RegularExpressions;
using MTConnect.Observations;

namespace MTConnect.Adapters
{
    public class ShdrTableCell : TableCell
    {
        public ShdrTableCell() { }

        public ShdrTableCell(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        public ShdrTableCell(TableCell cell)
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
