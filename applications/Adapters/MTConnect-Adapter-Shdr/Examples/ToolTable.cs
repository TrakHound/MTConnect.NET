using MTConnect.Observations;
using MTConnect.Shdr;
using System.Collections.Generic;

namespace MTConnect.Applications
{
    internal static partial class Examples
    {
        public static ShdrTable ToolTable()
        {
            var tableEntries = new List<TableEntry>();

            // Tool 1
            var t1Cells = new List<TableCell>();
            t1Cells.Add(new TableCell("LENGTH", 7.123));
            t1Cells.Add(new TableCell("DIAMETER", 0.494));
            t1Cells.Add(new TableCell("TOOL_LIFE", 0.35));
            tableEntries.Add(new TableEntry("T1", t1Cells));

            // Tool 2
            var t2Cells = new List<TableCell>();
            t2Cells.Add(new TableCell("LENGTH", 10.456));
            t2Cells.Add(new TableCell("DIAMETER", 0.125));
            t2Cells.Add(new TableCell("TOOL_LIFE", 1));
            tableEntries.Add(new TableEntry("T2", t2Cells));

            // Tool 3
            var t3Cells = new List<TableCell>();
            t3Cells.Add(new TableCell("LENGTH", 6.251));
            t3Cells.Add(new TableCell("DIAMETER", 1.249));
            t3Cells.Add(new TableCell("TOOL_LIFE", 0.93));
            tableEntries.Add(new TableEntry("T3", t3Cells));

            return new ShdrTable("L2p1ToolTable", tableEntries);
        }
    }
}
