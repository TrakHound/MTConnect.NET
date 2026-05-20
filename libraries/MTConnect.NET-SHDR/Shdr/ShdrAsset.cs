// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Xml;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// An Asset representing an MTConnect Asset to be sent using the SHDR protocol
    /// </summary>
    public class ShdrAsset
    {
        /// <summary>The SHDR designator that introduces a multi-line asset publish (<c>@ASSET@|assetId|assetType|--multiline--...</c>).</summary>
        public const string AssetDesignator = "@ASSET@";

        /// <summary>The SHDR designator that signals removal of a single asset by id (<c>@REMOVE_ASSET@|assetId</c>).</summary>
        public const string AssetRemoveDesignator = "@REMOVE_ASSET@";

        /// <summary>The SHDR designator that signals removal of every asset of a given type (<c>@REMOVE_ALL_ASSETS@|assetType</c>).</summary>
        public const string AssetRemoveAllDesignator = "@REMOVE_ALL_ASSETS@";

        /// <summary>The SHDR designator that updates an existing asset (<c>@UPDATE_ASSET@|assetId|...</c>).</summary>
        public const string AssetUpdateDesignator = "@UPDATE_ASSET@";

        private static readonly string AssetIdPattern = $"{AssetDesignator}\\|(.*)\\|.*\\|--multiline--";
        private static readonly string AssetTypePattern = $"{AssetDesignator}\\|.*\\|(.*)\\|--multiline--";
        private static readonly string AssetMutlilineBeginPattern = $"{AssetDesignator}.*--multiline--(.*)";
        private static readonly string AssetMutlilineEndPattern = "--multiline--(.*)";
        private static readonly string AssetRemovePattern = $"{AssetRemoveDesignator}\\|(.*)";
        private static readonly string AssetRemoveAllPattern = $"{AssetRemoveAllDesignator}\\|(.*)";
        private static readonly string AssetUpdatePattern = $"{AssetUpdateDesignator}\\|(.*)";

        private static readonly Encoding _utf8 = new UTF8Encoding();

        private static readonly Regex _assetIdRegex = new Regex(AssetIdPattern);
        private static readonly Regex _assetTypeRegex = new Regex(AssetTypePattern);
        private static readonly Regex _multilineBeginRegex = new Regex(AssetMutlilineBeginPattern);
        private static readonly Regex _multilineEndRegex = new Regex(AssetMutlilineEndPattern);
        private static readonly Regex _assetRemoveRegex = new Regex(AssetRemovePattern);
        private static readonly Regex _assetRemoveAllRegex = new Regex(AssetRemoveAllPattern);
        private static readonly Regex _assetUpdateRegex = new Regex(AssetUpdatePattern);

        /// <summary>
        /// Flag to set whether the Asset has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }

        /// <summary>
        /// The unique idenifier of the Asset
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The Type associated with the Asset
        /// </summary>
        public string AssetType { get; set; }

        /// <summary>
        /// The Asset object that represents the MTConnect Asset to output in the SHDR protocol
        /// </summary>
        public IAsset Asset { get; }

        /// <summary>
        /// The XML representation of the MTConnect Asset
        /// </summary>
        public string Xml { get; private set; }

        /// <summary>
        /// The Timestamp (in Unix Ticks) that represents when the Asset was created / updated
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The TimeZone that is configured to Output
        /// </summary>
        public TimeZoneInfo TimeZoneInfo { get; set; }


        private byte[] changeId;
        /// <summary>
        /// An MD5 Hash of the Asset that can be used for comparison
        /// </summary>
        public byte[] ChangeId
        {
            get
            {
                if (changeId == null)
                {
                    // Normalize Timestamp in order to compare
                    var asset = new ShdrAsset(AssetId, AssetType, Xml, 0);
                    if (asset.Asset != null)
                    {
                        ((Asset)asset.Asset).Timestamp = DateTime.MinValue;
                        //asset.Asset.Timestamp = 0;

                        var xmlBytes = XmlAsset.ToXml(asset.Asset);
                        if (xmlBytes != null)
                        {
                            //asset.Xml = Encoding.UTF8.GetString(xmlBytes);
                        }
                    }

                    changeId = _utf8.GetBytes(ToString(asset, true));
                }

                return changeId;
            }
        }

        private byte[] changeIdWithTimestamp;
        /// <summary>
        /// An MD5 Hash of the Asset including the Timestamp that can be used for comparison
        /// </summary>
        public byte[] ChangeIdWithTimestamp
        {
            get
            {
                if (changeIdWithTimestamp == null) changeIdWithTimestamp = _utf8.GetBytes(ToString(new ShdrAsset(AssetId, AssetType, Xml, Timestamp)));

                return changeIdWithTimestamp;
            }
        }


        /// <summary>Creates an empty SHDR asset record for builder-style population.</summary>
        public ShdrAsset() { }

        /// <summary>Creates an SHDR asset from its identifier, type, XML representation, and optional Unix-time <paramref name="timestamp"/>; the XML is parsed eagerly to populate <see cref="Asset"/>.</summary>
        public ShdrAsset(string assetId, string assetType, string xml, long timestamp = 0)
        {
            AssetId = assetId;
            AssetType = assetType;

            var xmlBytes = Encoding.UTF8.GetBytes(xml);
            Asset = XmlAsset.FromXml(assetType, xmlBytes);
            Xml = xml;
            Timestamp = timestamp;
        }

        /// <summary>Creates an SHDR asset wrapping the supplied <see cref="IAsset"/>; populates the id, type, and timestamp from the asset.</summary>
        public ShdrAsset(IAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                AssetType = asset.Type;
                Asset = asset;
                Timestamp = asset.Timestamp.ToUnixTime();

                var xmlBytes = XmlAsset.ToXml(asset);
                if (xmlBytes != null)
                {
                    //Xml = Encoding.UTF8.GetString(xmlBytes);
                }
            }
        }


        /// <summary>Serialises the asset using the multi-line SHDR encoding.</summary>
        public override string ToString() => ToString(true);

        /// <summary>Serialises the asset to its SHDR textual form, optionally using the multi-line encoding that wraps the XML body between matching <c>--multiline--id</c> markers.</summary>
        /// <param name="multiline">When true emits the multi-line form; when false produces a single-line representation suitable for compact log diffing.</param>
        public string ToString(bool multiline = false)
        {
            if (!string.IsNullOrEmpty(AssetId) && !string.IsNullOrEmpty(Xml))
            {
                if (multiline)
                {
                    var multilineId = StringFunctions.RandomString(10);

                    var header = $"|{AssetDesignator}|{AssetId}|{AssetType}|--multiline--{multilineId}";
                    if (Timestamp > 0) header = $"{GetTimestampString(Timestamp, timeZoneInfo: TimeZoneInfo)}{header}";

                    var xml = XmlFunctions.FormatXml(Xml, true, false, true);

                    var result = header;
                    result += "\n";
                    result += xml;
                    result += $"\n--multiline--{multilineId}\n";

                    return result;
                }
                else
                {
                    if (Timestamp > 0) return $"{GetTimestampString(Timestamp, timeZoneInfo: TimeZoneInfo)}|{AssetDesignator}|{AssetId}|{AssetType}|{Xml}";
                    else return $"|{AssetDesignator}|{AssetId}|{AssetType}|{Xml}";
                }
            }

            return null;
        }

        private static string ToString(ShdrAsset asset, bool ignoreTimestamp = false, TimeZoneInfo timeZoneInfo = null)
        {
            if (asset != null && !string.IsNullOrEmpty(asset.AssetId) && !string.IsNullOrEmpty(asset.Xml))
            {
                if (asset.Timestamp > 0 && !ignoreTimestamp)
                {
                    return $"{GetTimestampString(asset.Timestamp, timeZoneInfo: timeZoneInfo)}|{AssetDesignator}|{asset.AssetId}|{asset.AssetType}|{asset.Xml}";
                }
                else
                {
                    return $"|{AssetDesignator}|{asset.AssetId}|{asset.AssetType}|{asset.Xml}";
                }
            }

            return "";
        }


        #region "Commands"

        /// <summary>
        /// Create an SHDR string to Remove the specified Asset ID
        /// </summary>
        /// <param name="assetId">The Asset ID of the Asset to remove</param>
        /// <param name="timestamp">The timestamp to output in the SHDR string</param>
        public static string Remove(string assetId, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                return $"{ts.ToDateTime().ToString("o")}|{AssetRemoveDesignator}|{assetId}";
            }

            return null;
        }

        /// <summary>
        /// Create an SHDR string to Remove All Assets of the specified Type
        /// </summary>
        /// <param name="assetType">The Asset Type of the Asset(s) to remove</param>
        /// <param name="timestamp">The timestamp to output in the SHDR string</param>
        public static string RemoveAll(string assetType, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(assetType))
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                return $"{ts.ToDateTime().ToString("o")}|{AssetRemoveAllDesignator}|{assetType}";
            }

            return null;
        }

        #endregion

        #region "Detect"

        /// <summary>Returns true when <paramref name="input"/> begins with the <see cref="AssetDesignator"/> token (with or without a leading ISO timestamp).</summary>
        public static bool IsAssetLine(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Single) : <timestamp>|@ASSET@|<assetId>|<assetType>|<xml>
                // Expected format (Single) : 2012-02-21T23:59:33.460470Z|@ASSET@|KSSP300R.1|CuttingTool|<CuttingTool>...

                // Check if no Timestamp
                if (input[0] == '@' && input.StartsWith(AssetDesignator))
                {
                    return true;
                }

                // Check for Timestamp
                var i = input.IndexOf('|');
                if (i > 0 && i + 1 < input.Length - 1)
                {
                    var x = input.Substring(i + 1);
                    return x[0] == '@' && x.StartsWith(AssetDesignator);
                }
            }

            return false;
        }

        /// <summary>Returns true when <paramref name="input"/> is an asset line that opens a multi-line block (the line ends with <c>--multiline--id</c>).</summary>
        public static bool IsAssetMultilineBegin(string input)
        {
            if (IsAssetLine(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<assetId>|<assetType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    return _multilineBeginRegex.IsMatch(y);
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return false;
        }


        /// <summary>Returns true when <paramref name="input"/> closes the multi-line block whose opener carried the matching <paramref name="multilineId"/>.</summary>
        public static bool IsAssetMultilineEnd(string multilineId, string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : --multiline--0FED07ACED
                var match = _multilineEndRegex.Match(input);
                if (match.Success && match.Groups.Count > 1)
                {
                    var id = match.Groups[1].Value;
                    return id == multilineId;
                }
            }

            return false;
        }


        /// <summary>Returns true when <paramref name="input"/> begins with the <see cref="AssetRemoveDesignator"/> token (with or without a leading timestamp).</summary>
        public static bool IsAssetRemove(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format : @REMOVE_ASSET@|<AssetId>

                // Check if no Timestamp
                if (input[0] == '@' && input.StartsWith(AssetRemoveDesignator))
                {
                    return true;
                }

                // Check for Timestamp
                var i = input.IndexOf('|');
                if (i > 0 && i + 1 < input.Length - 1)
                {
                    var x = input.Substring(i + 1);
                    return x[0] == '@' && x.StartsWith(AssetRemoveDesignator);
                }
            }

            return false;
        }


        /// <summary>Returns true when <paramref name="input"/> begins with the <see cref="AssetRemoveAllDesignator"/> token (with or without a leading timestamp).</summary>
        public static bool IsAssetRemoveAll(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format : @REMOVE_ALL_ASSETS@|<AssetId>

                // Check if no Timestamp
                if (input[0] == '@' && input.StartsWith(AssetRemoveAllDesignator))
                {
                    return true;
                }

                // Check for Timestamp
                var i = input.IndexOf('|');
                if (i > 0 && i + 1 < input.Length - 1)
                {
                    var x = input.Substring(i + 1);
                    return x[0] == '@' && x.StartsWith(AssetRemoveAllDesignator);
                }
            }

            return false;
        }


        /// <summary>Returns true when <paramref name="input"/> begins with the <see cref="AssetUpdateDesignator"/> token (with or without a leading timestamp).</summary>
        public static bool IsAssetUpdate(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format : @UPDATE_ASSET@|<AssetId>|<PropertyName>|<PropertyValue>|...

                // Check if no Timestamp
                if (input[0] == '@' && input.StartsWith(AssetUpdateDesignator))
                {
                    return true;
                }

                // Check for Timestamp
                var i = input.IndexOf('|');
                if (i > 0 && i + 1 < input.Length - 1)
                {
                    var x = input.Substring(i + 1);
                    return x[0] == '@' && x.StartsWith(AssetUpdateDesignator);
                }
            }

            return false;
        }

        #endregion

        #region "Read"

        /// <summary>Parses the leading ISO timestamp segment of <paramref name="input"/> and returns it as Unix time (milliseconds since epoch); returns 0 when no timestamp is present.</summary>
        public static long ReadTimestamp(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out var timestamp))
                {
                    return timestamp.ToUnixTime();
                }
            }

            return 0;
        }

        /// <summary>Extracts the asset id from a multi-line asset header (<c>@ASSET@|assetId|assetType|--multiline--id</c>); returns null when no match is found.</summary>
        public static string ReadAssetId(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<assetId>|<assetType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _assetIdRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        /// <summary>Extracts the asset type from a multi-line asset header; returns null when no match is found.</summary>
        public static string ReadAssetType(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<assetId>|<assetType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _assetTypeRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        /// <summary>Extracts the multi-line block id that follows the <c>--multiline--</c> marker on an asset header; returns null when no match is found.</summary>
        public static string ReadAssetMultilineId(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<assetId>|<assetType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _multilineBeginRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        /// <summary>Extracts the target asset id from a <c>@REMOVE_ASSET@|assetId</c> SHDR line; returns null when no match is found.</summary>
        public static string ReadRemoveAssetId(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _assetRemoveRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        /// <summary>Extracts the target asset type from a <c>@REMOVE_ALL_ASSETS@|type</c> SHDR line; returns null when no match is found.</summary>
        public static string ReadRemoveAllAssetType(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _assetRemoveAllRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }


        /// <summary>Parses an SHDR asset-publish line back into an <see cref="ShdrAsset"/> instance, lifting any leading timestamp; returns null when <paramref name="input"/> does not match the expected layout.</summary>
        public static ShdrAsset FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Single) : <timestamp>|@ASSET@|<assetId>|<assetType>|<xml>
                // Expected format (Single) : 2012-02-21T23:59:33.460470Z|@ASSET@|KSSP300R.1|CuttingTool|<CuttingTool>...

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out var timestamp))
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromLine(y, timestamp.ToUnixTime());
                }
                else
                {
                    return FromLine(input);
                }
            }

            return null;
        }

        private static ShdrAsset FromLine(string input, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    // Skip @ASSET@. We already know if it is an Asset or not by this point.
                    var y = ShdrLine.GetNextSegment(input);
                    if (y != null)
                    {
                        // Set Asset ID
                        var x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);
                        var assetId = x;

                        if (y != null)
                        {
                            // Set Asset Type
                            x = ShdrLine.GetNextValue(y);
                            y = ShdrLine.GetNextSegment(y);
                            var assetType = x;

                            if (y != null)
                            {
                                // Set Asset XML
                                x = ShdrLine.GetNextValue(y);
                                if (x != null)
                                {
                                    return new ShdrAsset(assetId, assetType, x, timestamp);
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        #endregion


        /// <summary>Formats a Unix-time <paramref name="timestamp"/> as the ISO 8601 string used in the SHDR timestamp field, honouring <paramref name="timeZoneInfo"/> when supplied or falling back to UTC. Returns an empty string when <paramref name="timestamp"/> is non-positive.</summary>
        protected static string GetTimestampString(long timestamp, TimeZoneInfo timeZoneInfo = null)
        {
            if (timestamp > 0)
            {
                var dateTime = timestamp.ToDateTime();
                var dateTimeOffset = MTConnectTimeZone.GetTimestamp(dateTime, timeZoneInfo);

                if (dateTimeOffset.Offset != TimeSpan.Zero)
                {
                    return dateTimeOffset.ToString("o");
                }
                else
                {
                    return dateTimeOffset.UtcDateTime.ToString("o");
                }
            }

            return null;
        }
    }
}