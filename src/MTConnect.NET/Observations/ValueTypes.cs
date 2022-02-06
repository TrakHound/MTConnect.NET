// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.RegularExpressions;

namespace MTConnect.Observations
{
    public class ValueTypes
    {
        public const string CDATA = "CDATA";
        public const string Level = "Level";
        public const string NativeCode = "NativeCode";
        public const string NativeSeverity = "NativeSeverity";
        public const string Qualifier = "Qualifier";
        public const string SampleRate = "SampleRate";
        public const string Count = "Count";
        public const string Duration = "Duration";
        public const string ResetTriggered = "ResetTriggered";
        public const string TimeSeriesPrefix = "TimeSeries";
        public const string DataSetPrefix = "DataSet";
        public const string TablePrefix = "Table";


        public static string CreateTimeSeriesValueType(int index) => $"{TimeSeriesPrefix}[{index}]";

        public static int GetTimeSeriesIndex(string valueType)
        {
            if (!string.IsNullOrEmpty(valueType))
            {
                var match = new Regex($@"{TimeSeriesPrefix}\[(\d*)\]").Match(valueType);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value.ToInt();
                }
            }

            return -1;
        }

        public static string CreateDataSetValueType(string key) => $"{DataSetPrefix}[{key}]";

        public static string GetDataSetKey(string valueType)
        {
            if (!string.IsNullOrEmpty(valueType))
            {
                var match = new Regex($@"{DataSetPrefix}\[(.*)\]").Match(valueType);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        public static string CreateTableValueType(string key, string cell) => $"{TablePrefix}[{key}][{cell}]";

        public static string GetTableKey(string valueType)
        {
            if (!string.IsNullOrEmpty(valueType))
            {
                var match = new Regex($@"{TablePrefix}\[(.*)\]\[.*\]").Match(valueType);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        public static string GetTableValue(string valueType, string key)
        {
            if (!string.IsNullOrEmpty(valueType))
            {
                var match = new Regex($@"{TablePrefix}\[.*\]\[(.*)\]").Match(valueType);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }
    }
}