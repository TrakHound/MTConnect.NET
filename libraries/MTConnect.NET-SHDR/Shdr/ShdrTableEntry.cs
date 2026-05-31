// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// SHDR-flavoured <see cref="TableEntry"/> that round-trips with the SHDR row syntax
    /// <c>rowKey={cell1=v1 cell2=v2 ...}</c>; a removed row is encoded as <c>rowKey=</c> with an
    /// empty value. The extra <see cref="IsSent"/> flag lets the adapter de-duplicate rows that
    /// have already been transmitted in this update cycle.
    /// </summary>
    public class ShdrTableEntry : TableEntry
    {
        /// <summary>Adapter-local flag set once the entry has been written to the SHDR socket; used to suppress duplicate transmissions.</summary>
        public bool IsSent { get; set; }


        /// <summary>Creates an empty entry for serialiser-driven construction.</summary>
        public ShdrTableEntry() { }

        /// <summary>Creates a populated entry with a row key and the ordered list of cells.</summary>
        public ShdrTableEntry(string key, IEnumerable<ITableCell> cells)
        {
            Key = key;
            Cells = cells;
        }

        /// <summary>Creates a tombstone entry that records the removal of the row identified by <paramref name="key"/>.</summary>
        public ShdrTableEntry(string key, bool removed)
        {
            Key = key;
            Removed = removed;
        }

        /// <summary>Clones the supplied <see cref="ITableEntry"/> into a new SHDR-flavoured entry.</summary>
        public ShdrTableEntry(ITableEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Cells = entry.Cells;
                Removed = entry.Removed;
            }
        }

        /// <summary>Serialises the entry to its SHDR <c>rowKey={cells}</c> textual form (or <c>rowKey=</c> when removed); returns the empty string when <see cref="TableEntry.Key"/> is empty.</summary>
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

        /// <summary>Parses a single SHDR <c>rowKey={cells}</c> segment back into an entry; returns <c>null</c> when <paramref name="segment"/> is empty or does not match the expected pattern.</summary>
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