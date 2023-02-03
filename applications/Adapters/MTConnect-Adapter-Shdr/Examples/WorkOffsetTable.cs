using MTConnect.Observations;
using MTConnect.Shdr;
using System.Collections.Generic;

namespace MTConnect.Applications
{
    internal static partial class Examples
    {
        public static ShdrTable WorkOffsetTable()
        {
            var tableEntries = new List<TableEntry>();

            // G54
            var g54Cells = new List<TableCell>();
            g54Cells.Add(new TableCell("X", 7.123));
            g54Cells.Add(new TableCell("Y", 0.494));
            g54Cells.Add(new TableCell("Z", 0.35));
            tableEntries.Add(new TableEntry("G54", g54Cells));

            // G55
            var g55Cells = new List<TableCell>();
            g55Cells.Add(new TableCell("X", 7.123));
            g55Cells.Add(new TableCell("Y", 0.494));
            g55Cells.Add(new TableCell("Z", 0.35));
            tableEntries.Add(new TableEntry("G55", g55Cells));

            // G56
            var g56Cells = new List<TableCell>();
            g56Cells.Add(new TableCell("X", 7.123));
            g56Cells.Add(new TableCell("Y", 0.494));
            g56Cells.Add(new TableCell("Z", 0.35));
            tableEntries.Add(new TableEntry("G56", g56Cells));

            return new ShdrTable("workOffsetTable", tableEntries);
        }
    }
}
