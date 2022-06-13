// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using System;
using System.Text.RegularExpressions;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrAsset
    {
        public const string AssetDesignator = "@ASSET@";

        private const string AssetIdPattern = "@ASSET@\\|(.*)\\|.*\\|--multiline--";
        private const string AssetTypePattern = "@ASSET@\\|.*\\|(.*)\\|--multiline--";
        private const string AssetMutlilineBeginPattern = "@ASSET@.*--multiline--(.*)";
        private const string AssetMutlilineEndPattern = "--multiline--(.*)";

        private static readonly Regex _assetIdRegex = new Regex(AssetIdPattern);
        private static readonly Regex _assetTypeRegex = new Regex(AssetTypePattern);
        private static readonly Regex _multilineBeginRegex = new Regex(AssetMutlilineBeginPattern);
        private static readonly Regex _multilineEndRegex = new Regex(AssetMutlilineEndPattern);


        public string AssetId { get; set; }

        public string AssetType { get; set; }

        public IAsset Asset { get; set; }

        public string Xml { get; set; }

        public long Timestamp { get; set; }

        public bool IsSent { get; set; }

        public string ChangeId
        {
            get
            {
                var x = new ShdrAsset();
                x.AssetId = AssetId;
                x.AssetType = AssetType;
                x.Timestamp = 0; // Normalize Timestamp in order to compare
                x.Xml = x.Xml;
                return x.ToString();
            }
        }


        public ShdrAsset() { }

        public ShdrAsset(string assetId, string assetType, string xml, long timestamp = 0)
        {
            AssetId = assetId;
            AssetType = assetType;
            Asset = XmlAsset.FromXml(assetType, xml);
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

                    var header = $"{AssetDesignator}|{AssetId}|{AssetType}|--multiline--{multilineId}";
                    if (Timestamp > 0) header = $"{Timestamp.ToString("o")}|{header}";

                    var xml = XmlFunctions.FormatXml(Xml, true, false, true);

                    var result = header;
                    result += "\n";
                    result += xml;
                    result += $"\n--multiline--{multilineId}\n";

                    return result;
                }
                else
                {
                    if (Timestamp > 0) return $"{Timestamp.ToString("o")}|{AssetDesignator}|{AssetId}|{AssetType}|{Xml}";
                    else return $"{AssetDesignator}|{AssetId}|{AssetType}|{Xml}";
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
                    return $"{asset.Timestamp.ToString("o")}|{AssetDesignator}|{asset.AssetId}|{asset.AssetType}|{asset.Xml}";
                }
                else
                {
                    return $"{AssetDesignator}|{asset.AssetId}|{asset.AssetType}|{asset.Xml}";
                }
            }

            return "";
        }

        public static bool IsAssetLine(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Single) : <timestamp>|@ASSET@|<assetId>|<assetType>|<xml>
                // Expected format (Single) : 2012-02-21T23:59:33.460470Z|@ASSET@|KSSP300R.1|CuttingTool|<CuttingTool>...

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);
                    x = ShdrLine.GetNextValue(y);
                    return x == AssetDesignator;
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return false;
        }

        public static bool IsAssetMultilineBegin(string input)
        {
            if (!string.IsNullOrEmpty(input))
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
                    var asset = new ShdrAsset();
                    asset.Timestamp = timestamp;

                    // Skip @ASSET@. We already know if it is an Asset or not by this point.
                    var y = ShdrLine.GetNextSegment(input);
                    if (y != null)
                    {
                        // Set Asset ID
                        var x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);
                        asset.AssetId = x;

                        if (y != null)
                        {
                            // Set Asset Type
                            x = ShdrLine.GetNextValue(y);
                            y = ShdrLine.GetNextSegment(y);
                            asset.AssetType = x;

                            if (y != null)
                            {
                                // Set Asset XML
                                x = ShdrLine.GetNextValue(y);
                                if (x != null)
                                {
                                    asset.Xml = x;
                                    return asset;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
