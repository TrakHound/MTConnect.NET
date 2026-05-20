// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// SHDR-flavoured <see cref="TableCell"/> that adds round-tripping to and from the
    /// <c>key=value</c> token form used inside SHDR table-row segments.
    /// </summary>
    public class ShdrTableCell : TableCell
    {
        /// <summary>Creates an empty cell for serialiser-driven construction.</summary>
        public ShdrTableCell() { }

        /// <summary>Creates a cell with an explicit key and value; <paramref name="value"/> is rendered via <see cref="object.ToString"/>.</summary>
        public ShdrTableCell(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        /// <summary>Clones the supplied <see cref="ITableCell"/> into a new SHDR-flavoured cell.</summary>
        public ShdrTableCell(ITableCell cell)
        {
            if (cell != null)
            {
                Key = cell.Key;
                Value = cell.Value;
            }
        }


        /// <summary>Serialises the cell to its SHDR <c>key=value</c> textual form; returns the empty string when <see cref="TableCell.Key"/> is empty.</summary>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                return $"{Key}={Value}";
            }

            return "";
        }

        /// <summary>Parses a single SHDR <c>key=value</c> segment back into a cell; returns <c>null</c> when <paramref name="segment"/> is empty or does not match the expected pattern.</summary>
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