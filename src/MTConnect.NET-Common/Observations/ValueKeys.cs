// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace MTConnect.Observations
{
    /// <summary>
    /// Tools to create Keys used to store and access Observation Values
    /// </summary>
    public class ValueKeys
    {
        public const string Result = "Result";
        public const string Level = "Level";
        public const string NativeCode = "NativeCode";
        public const string NativeSeverity = "NativeSeverity";
        public const string Qualifier = "Qualifier";
        public const string Message = "Message";
        public const string Statistic = "Statistic";
        public const string SampleRate = "SampleRate";
        public const string SampleCount = "SampleCount";
        public const string Count = "Count";
        public const string Duration = "Duration";
        public const string AssetType = "AssetType";
        public const string ResetTriggered = "ResetTriggered";
        public const string TimeSeriesPrefix = "TimeSeries";
        public const string DataSetPrefix = "DataSet";
        public const string TablePrefix = "Table";

        private static readonly ConcurrentDictionary<string, string> _pascalKeys = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _camelKeys = new ConcurrentDictionary<string, string>();


        #region "TimeSeries"

        public static string CreateTimeSeriesValueKey(int index) => $"{TimeSeriesPrefix}[{index.ToString("00000")}]";

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

        public static bool IsTimeSeriesKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                return valueKey.StartsWith(TimeSeriesPrefix);
            }

            return false;
        }

        #endregion

        #region "DataSet"

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

        public static bool IsDataSetKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                return valueKey.StartsWith(DataSetPrefix);
            }

            return false;
        }

        #endregion

        #region "Table"

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

        public static bool IsTableKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                return valueKey.StartsWith(TablePrefix);
            }

            return false;
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

        public static string GetTableCellKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                var match = new Regex($@"{TablePrefix}\[([^\[\]]*)\]\[([^\[\]]*)\]").Match(valueKey);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[2].Value;
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

        #endregion


        public static string GetPascalCaseKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string camelKey;
                _pascalKeys.TryGetValue(key, out camelKey);
                if (camelKey == null)
                {
                    camelKey = key.ToPascalCase();
                    _pascalKeys.TryAdd(key, camelKey);
                }
                return camelKey;
            }

            return null;
        }

        public static string GetCamelCaseKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string camelKey;
                _camelKeys.TryGetValue(key, out camelKey);
                if (camelKey == null)
                {
                    camelKey = key.ToCamelCase();
                    _camelKeys.TryAdd(key, camelKey);
                }
                return camelKey;
            }

            return null;
        }
    }
}