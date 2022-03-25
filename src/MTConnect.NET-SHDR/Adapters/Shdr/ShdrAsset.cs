// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using System;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrAsset : Asset
    {
        public const string AssetDesignator = "@ASSET@";


        public bool IsSent { get; set; }

        public string ChangeId
        {
            get
            {
                var x = new ShdrAsset();
                x.AssetId = AssetId;
                x.Type = Type;
                x.Timestamp = DateTime.MinValue; // Normalize Timestamp in order to compare
                x.Xml = x.Xml;
                return x.ToString();
            }
        }


        public ShdrAsset() { }

        public ShdrAsset(IAsset asset)
        {
            if (asset != null)
            {
                var type = asset.GetType();
                Console.WriteLine(type.ToString());


                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp;
                Xml = XmlAsset.ToXml(asset);
            }
        }


        public override string ToString()
        {
            if (!string.IsNullOrEmpty(AssetId) && !string.IsNullOrEmpty(Xml))
            {
                if (Timestamp.ToUnixTime() > 0)
                {
                    return $"{Timestamp.ToString("o")}|{AssetDesignator}|{AssetId}|{Type}|{Xml}";
                }
                else
                {
                    return $"{AssetDesignator}|{AssetId}|{Type}|{Xml}";
                }
            }

            return null;
        }

        private static string ToString(ShdrAsset asset, bool ignoreTimestamp = false)
        {
            if (asset != null && !string.IsNullOrEmpty(asset.AssetId) && !string.IsNullOrEmpty(asset.Xml))
            {
                if (asset.Timestamp.ToUnixTime() > 0 && !ignoreTimestamp)
                {
                    return $"{asset.Timestamp.ToString("o")}|{AssetDesignator}|{asset.AssetId}|{asset.Type}|{asset.Xml}";
                }
                else
                {
                    return $"{AssetDesignator}|{asset.AssetId}|{asset.Type}|{asset.Xml}";
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
                    asset.Timestamp = timestamp.ToDateTime();

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
                            asset.Type = x;

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
