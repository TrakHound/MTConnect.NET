// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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
        public const string AssetDesignator = "@ASSET@";
        public const string AssetRemoveDesignator = "@REMOVE_ASSET@";
        public const string AssetRemoveAllDesignator = "@REMOVE_ALL_ASSETS@";
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
                        asset.Xml = XmlAsset.ToXml(asset.Asset);
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


        public ShdrAsset() { }

        public ShdrAsset(string assetId, string assetType, string xml, long timestamp = 0)
        {
            AssetId = assetId;
            AssetType = assetType;

            var xmlBytes = Encoding.UTF8.GetBytes(xml);
            Asset = XmlAsset.FromXml(assetType, xmlBytes);
            Xml = xml;
            Timestamp = timestamp;
        }

        public ShdrAsset(IAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                AssetType = asset.Type;
                Asset = asset;
                Xml = XmlAsset.ToXml(asset);
                Timestamp = asset.Timestamp.ToUnixTime();
            }
        }


        public override string ToString() => ToString(true);

        public string ToString(bool multiline = false)
        {
            if (!string.IsNullOrEmpty(AssetId) && !string.IsNullOrEmpty(Xml))
            {
                if (multiline)
                {
                    var multilineId = StringFunctions.RandomString(10);

                    var header = $"|{AssetDesignator}|{AssetId}|{AssetType}|--multiline--{multilineId}";
                    if (Timestamp > 0) header = $"{Timestamp.ToDateTime().ToString("o")}{header}";

                    var xml = XmlFunctions.FormatXml(Xml, true, false, true);

                    var result = header;
                    result += "\n";
                    result += xml;
                    result += $"\n--multiline--{multilineId}\n";

                    return result;
                }
                else
                {
                    if (Timestamp > 0) return $"{Timestamp.ToDateTime().ToString("o")}|{AssetDesignator}|{AssetId}|{AssetType}|{Xml}";
                    else return $"|{AssetDesignator}|{AssetId}|{AssetType}|{Xml}";
                }
            }

            return null;
        }

        private static string ToString(ShdrAsset asset, bool ignoreTimestamp = false)
        {
            if (asset != null && !string.IsNullOrEmpty(asset.AssetId) && !string.IsNullOrEmpty(asset.Xml))
            {
                if (asset.Timestamp > 0 && !ignoreTimestamp)
                {
                    return $"{asset.Timestamp.ToDateTime().ToString("o")}|{AssetDesignator}|{asset.AssetId}|{asset.AssetType}|{asset.Xml}";
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
    }
}