// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;

namespace MTConnect.Streams.Xml
{
    internal static class XmlObservation
    {
        public const string DataSetSuffix = "DataSet";
        public const string TableSuffix = "Table";
        public const string TimeSeriesSuffix = "TimeSeries";
        public const string XmlAttributeName = "XmlAttributeAttribute";


        public static bool IsDataSet(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(DataSetSuffix);
        }

        public static bool IsTable(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(TableSuffix);
        }

        public static bool IsTimeSeries(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(TimeSeriesSuffix);
        }

        public static string GetDataItemType(string elementName)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                var name = elementName;

                if (IsDataSet(name) && name.Length > DataSetSuffix.Length)
                {
                    // Remove the "DataSet" suffix from the Type
                    name = name.Substring(0, name.Length - DataSetSuffix.Length);
                }
                else if (IsTable(name) && name.Length > TableSuffix.Length)
                {
                    // Remove the "Table" suffix from the Type
                    name = name.Substring(0, name.Length - TableSuffix.Length);
                }
                else if (IsTimeSeries(name) && name.Length > TimeSeriesSuffix.Length)
                {
                    // Remove the "TimeSeries" suffix from the Type
                    name = name.Substring(0, name.Length - TimeSeriesSuffix.Length);
                }

                return DataItem.GetPascalCaseType(name);
            }

            return null;
        }

        public static DataItemRepresentation GetRepresentation(string elementName)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                if (elementName.EndsWith(DataSetSuffix))
                {
                    return DataItemRepresentation.DATA_SET;
                }
                else if (elementName.EndsWith(TableSuffix))
                {
                    return DataItemRepresentation.TABLE;
                }
                else if (elementName.EndsWith(TimeSeriesSuffix))
                {
                    return DataItemRepresentation.TIME_SERIES;
                }
            }

            return DataItemRepresentation.VALUE;
        }
    }
}
