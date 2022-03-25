// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.RegularExpressions;

namespace MTConnect.Observations
{
    /// <summary>
    /// Tools to create Keys used to store and access Observation Values
    /// </summary>
    public class ValueKeys
    {
        public const string CDATA = "CDATA";
        public const string Level = "Level";
        public const string NativeCode = "NativeCode";
        public const string NativeSeverity = "NativeSeverity";
        public const string Qualifier = "Qualifier";
        public const string Statistic = "Statistic";
        public const string SampleRate = "SampleRate";
        public const string Count = "Count";
        public const string Duration = "Duration";
        public const string AssetType = "AssetType";
        public const string ResetTriggered = "ResetTriggered";
        public const string TimeSeriesPrefix = "TimeSeries";
        public const string DataSetPrefix = "DataSet";
        public const string TablePrefix = "Table";


        public static string CreateTimeSeriesValueKey(int index) => $"{TimeSeriesPrefix}[{index}]";

        public static int GetTimeSeriesIndex(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                var match = new Regex($@"{TimeSeriesPrefix}\[(\d*)\]").Match(valueKey);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value.ToInt();
                }
            }

            return -1;
        }

        public static string CreateDataSetValueKey(string key) => $"{DataSetPrefix}[{key}]";

        public static string GetDataSetKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                var match = new Regex($@"{DataSetPrefix}\[(.*)\]").Match(valueKey);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        public static string CreateTableValueKey(string key, string cellKey = null)
        {
            if (!string.IsNullOrEmpty(cellKey))
            {
                return $"{TablePrefix}[{key}][{cellKey}]";
            }
            else
            {
                return $"{TablePrefix}[{key}]";
            }
        }

        public static string GetTableKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                var match = new Regex($@"{TablePrefix}\[([^\[\]]*)\](?:\[[^\[\]]*\])?").Match(valueKey);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        public static string GetTableValue(string valueKey, string cellKey)
        {
            if (!string.IsNullOrEmpty(valueKey) && !string.IsNullOrEmpty(cellKey))
            {
                var match = new Regex($@"{TablePrefix}\[.*\]\[(.*)\]").Match(valueKey);
                if (match.Success && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }
    }
}