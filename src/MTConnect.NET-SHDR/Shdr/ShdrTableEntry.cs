// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    public class ShdrTableEntry : TableEntry
    {
        public bool IsSent { get; set; }


        public ShdrTableEntry() { }

        public ShdrTableEntry(string key, IEnumerable<ITableCell> cells)
        {
            Key = key;
            Cells = cells;
        }

        public ShdrTableEntry(string key, bool removed)
        {
            Key = key;
            Removed = removed;
        }

        public ShdrTableEntry(ITableEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Cells = entry.Cells;
                Removed = entry.Removed;
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                if (!Removed) return $"{Key}={{{PrintCells(Cells)}}}";
                else return $"{Key}=";
            }

            return "";
        }

        private static string PrintCells(IEnumerable<ITableCell> cells)
        {
            if (!cells.IsNullOrEmpty())
            {
                var pairs = new List<string>();

                foreach (var cell in cells)
                {
                    pairs.Add(new ShdrTableCell(cell).ToString());
                }

                return string.Join(" ", pairs);
            }

            return "";
        }

        public static ShdrTableEntry FromString(string segment)
        {
            if (!string.IsNullOrEmpty(segment))
            {
                var regex = new Regex(@"(.*)=\{(.*)\}");
                var match = regex.Match(segment);
                if (match.Success && match.Groups.Count > 2)
                {
                    var key = match.Groups[1].Value;
                    var values = match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(values))
                    {
                        var cellSegments = values.Split(' ');
                        if (!cellSegments.IsNullOrEmpty())
                        {
                            var cells = new List<TableCell>();

                            foreach (var cellSegment in cellSegments)
                            {
                                var cell = ShdrTableCell.FromString(cellSegment);
                                if (cell != null)
                                {
                                    cells.Add(cell);
                                }
                            }

                            if (!cells.IsNullOrEmpty())
                            {
                                return new ShdrTableEntry(key, cells);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
