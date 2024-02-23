// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
        public const string ConditionId = "ConditionId";
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
        public const string DeviceType = "DeviceType";
        public const string Hash = "Hash";
        public const string ResetTriggered = "ResetTriggered";
        public const string TimeSeriesPrefix = "TimeSeries";
        public const string DataSetPrefix = "DataSet";
        public const string TablePrefix = "Table";

        private static readonly ConcurrentDictionary<string, string> _pascalKeys = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _camelKeys = new ConcurrentDictionary<string, string>();

        private static readonly Regex _timeseriesIndexRegex = new Regex($@"{TimeSeriesPrefix}\[(\d*)\]", RegexOptions.Compiled);


        #region "TimeSeries"

        public static string CreateTimeSeriesValueKey(int index) => $"{TimeSeriesPrefix}[{index.ToString("00000")}]";

        public static int GetTimeSeriesIndex(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                var match = _timeseriesIndexRegex.Match(valueKey);
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
            if (!string.IsNullOrEmpty(valueKey) && valueKey.Length > DataSetPrefix.Length)
            {
                var s = 0;
                var e = 0;

                for (var i = DataSetPrefix.Length; i < valueKey.Length; i++)
                {
                    if (valueKey[i] == '[') s = i + 1;
                    if (valueKey[i] == ']')
                    {
                        e = i;
                        break;
                    }
                }

                if (s > 0 && e > s)
                {
                    return valueKey.Substring(s, e - s);
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
            if (!string.IsNullOrEmpty(valueKey) && valueKey.Length > TablePrefix.Length)
            {
                var s = 0;
                var e = 0;

                for (var i = TablePrefix.Length; i < valueKey.Length; i++)
                {
                    if (valueKey[i] == '[') s = i + 1;
                    if (valueKey[i] == ']')
                    {
                        e = i;
                        break;
                    }
                }

                if (s > 0 && e > s)
                {
                    return valueKey.Substring(s, e - s);
                }
            }

            return null;
        }

        public static string GetTableCellKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey) && valueKey.Length > TablePrefix.Length)
            {
                var s = 0;
                var e = 0;
                var keyRead = false;

                for (var i = TablePrefix.Length; i < valueKey.Length; i++)
                {
                    if (valueKey[i] == '[') s = i + 1;
                    if (valueKey[i] == ']')
                    {
                        e = i;
                        if (keyRead) break;
                        else keyRead = true;
                    }
                }

                if (s > 0 && e > s)
                {
                    return valueKey.Substring(s, e - s);
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