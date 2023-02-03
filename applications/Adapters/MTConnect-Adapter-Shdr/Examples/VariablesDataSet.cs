using MTConnect.Observations;
using MTConnect.Shdr;
using System.Collections.Generic;

namespace MTConnect.Applications
{
    internal static partial class Examples
    {
        public static ShdrDataSet VariablesDataSet()
        {
            var dataSetEntries = new List<DataSetEntry>();
            dataSetEntries.Add(new DataSetEntry("V1", 5));
            dataSetEntries.Add(new DataSetEntry("V2", 205));
            dataSetEntries.Add(new DataSetEntry("V3", "DUMMY"));
            dataSetEntries.Add(new DataSetEntry("V4", 147.1234));

            return new ShdrDataSet("vars", dataSetEntries);
        }
    }
}
