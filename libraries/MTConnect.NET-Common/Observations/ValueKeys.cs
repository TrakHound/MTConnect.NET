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
        /// <summary>The value key holding an Observation's primary result value.</summary>
        public const string Result = "Result";

        /// <summary>The value key holding a Condition Observation's level (Normal, Warning, Fault, Unavailable).</summary>
        public const string Level = "Level";

        /// <summary>The value key holding the identifier that correlates related Condition Observations.</summary>
        public const string ConditionId = "ConditionId";

        /// <summary>The value key holding a device-native code reported alongside a Condition.</summary>
        public const string NativeCode = "NativeCode";

        /// <summary>The value key holding a device-native severity reported alongside a Condition.</summary>
        public const string NativeSeverity = "NativeSeverity";

        /// <summary>The value key holding a Condition's qualifier (for example High or Low).</summary>
        public const string Qualifier = "Qualifier";

        /// <summary>The value key holding the human-readable message text of an Observation.</summary>
        public const string Message = "Message";

        /// <summary>The value key holding the statistical operation applied to a Sample value.</summary>
        public const string Statistic = "Statistic";

        /// <summary>The value key holding the reporting rate of a Sample Observation.</summary>
        public const string SampleRate = "SampleRate";

        /// <summary>The value key holding the number of samples carried by a Time Series Observation.</summary>
        public const string SampleCount = "SampleCount";

        /// <summary>The value key holding the entry count of a Data Set or Table Observation.</summary>
        public const string Count = "Count";

        /// <summary>The value key holding the time span an Observation covers.</summary>
        public const string Duration = "Duration";

        /// <summary>The value key holding the asset type associated with an asset-related Observation.</summary>
        public const string AssetType = "AssetType";

        /// <summary>The value key holding the device type associated with the Observation.</summary>
        public const string DeviceType = "DeviceType";

        /// <summary>The value key holding the content hash used to detect Observation changes.</summary>
        public const string Hash = "Hash";

        /// <summary>The value key flagging that a Data Set or Table Observation reset its accumulated entries.</summary>
        public const string ResetTriggered = "ResetTriggered";

        /// <summary>The key prefix used to compose indexed Time Series value keys.</summary>
        public const string TimeSeriesPrefix = "TimeSeries";

        /// <summary>The key prefix used to compose keyed Data Set value keys.</summary>
        public const string DataSetPrefix = "DataSet";

        /// <summary>The key prefix used to compose keyed Table value keys.</summary>
        public const string TablePrefix = "Table";

        private static readonly ConcurrentDictionary<string, string> _pascalKeys = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _camelKeys = new ConcurrentDictionary<string, string>();

        private static readonly Regex _timeseriesIndexRegex = new Regex($@"{TimeSeriesPrefix}\[(\d*)\]", RegexOptions.Compiled);


        #region "TimeSeries"

        /// <summary>
        /// Builds the value key for the sample at the given position within a Time Series Observation.
        /// </summary>
        /// <param name="index">The zero-based sample position.</param>
        /// <returns>The composed Time Series value key.</returns>
        public static string CreateTimeSeriesValueKey(int index) => $"{TimeSeriesPrefix}[{index.ToString("00000")}]";

        /// <summary>
        /// Extracts the sample position encoded in a Time Series value key.
        /// </summary>
        /// <param name="valueKey">The Time Series value key to parse.</param>
        /// <returns>The zero-based sample position, or -1 when the key is not a Time Series key.</returns>
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

        /// <summary>
        /// Determines whether the given value key identifies a Time Series sample.
        /// </summary>
        /// <param name="valueKey">The value key to test.</param>
        /// <returns>True when the key uses the Time Series prefix; otherwise false.</returns>
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

        /// <summary>
        /// Builds the value key for the entry stored under the given key in a Data Set Observation.
        /// </summary>
        /// <param name="key">The Data Set entry key.</param>
        /// <returns>The composed Data Set value key.</returns>
        public static string CreateDataSetValueKey(string key) => $"{DataSetPrefix}[{key}]";

        /// <summary>
        /// Extracts the entry key encoded in a Data Set value key.
        /// </summary>
        /// <param name="valueKey">The Data Set value key to parse.</param>
        /// <returns>The entry key, or null when the value key is not a Data Set key.</returns>
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

        /// <summary>
        /// Determines whether the given value key identifies a Data Set entry.
        /// </summary>
        /// <param name="valueKey">The value key to test.</param>
        /// <returns>True when the key uses the Data Set prefix; otherwise false.</returns>
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

        /// <summary>
        /// Builds the value key for a Table Observation entry, optionally addressing an individual cell.
        /// </summary>
        /// <param name="key">The Table entry (row) key.</param>
        /// <param name="cellKey">The optional cell (column) key within the entry.</param>
        /// <returns>The composed Table value key.</returns>
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

        /// <summary>
        /// Determines whether the given value key identifies a Table entry.
        /// </summary>
        /// <param name="valueKey">The value key to test.</param>
        /// <returns>True when the key uses the Table prefix; otherwise false.</returns>
        public static bool IsTableKey(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey))
            {
                return valueKey.StartsWith(TablePrefix);
            }

            return false;
        }

        /// <summary>
        /// Extracts the entry (row) key encoded in a Table value key.
        /// </summary>
        /// <param name="valueKey">The Table value key to parse.</param>
        /// <returns>The entry key, or null when the value key is not a Table key.</returns>
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

        /// <summary>
        /// Extracts the cell (column) key encoded in a Table value key.
        /// </summary>
        /// <param name="valueKey">The Table value key to parse.</param>
        /// <returns>The cell key, or null when the value key does not address a cell.</returns>
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


        /// <summary>
        /// Returns the PascalCase form of a value key, caching the conversion for reuse.
        /// </summary>
        /// <param name="key">The value key to convert.</param>
        /// <returns>The PascalCase key, or null when <paramref name="key"/> is null or empty.</returns>
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

        /// <summary>
        /// Returns the camelCase form of a value key, caching the conversion for reuse.
        /// </summary>
        /// <param name="key">The value key to convert.</param>
        /// <returns>The camelCase key, or null when <paramref name="key"/> is null or empty.</returns>
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