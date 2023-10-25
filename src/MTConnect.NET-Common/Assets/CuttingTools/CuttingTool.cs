// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public partial class CuttingTool : Asset
    {
        public const string TypeId = "CuttingTool";


        /// <summary>
        /// The unique identifier for this assembly. 
        /// This is defined as an XML string type and is implementation dependent.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// The identifier for a class of cutting tools. 
        /// This is defined as an XML string type and is implementation dependent.
        /// </summary>
        public string ToolId { get; set; }

        /// <summary>
        /// The manufacturers of the cutting tool. 
        /// An optional attribute referring to the manufacturers of this tool, for this element, this will reference the Tool Item and Adaptive Items specifically. 
        /// The Cutting Items manufacturers’ will be an attribute of the CuttingItem elements. 
        /// The representation will be a comma (,) delimited list of manufacturer names. This can be any series of numbers and letters as defined by the XML type string.
        /// </summary>
        [XmlAttribute("manufacturers")]
        [JsonPropertyName("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// Data regarding the use of this tool.
        /// </summary>
        [XmlElement("CuttingToolLifeCycle")]
        [JsonPropertyName("cuttingToolLifeCycle")]
        public CuttingToolLifeCycle CuttingToolLifeCycle { get; set; }

        /// <summary>
        /// The content of this XML element is the assetId of the CuttingToolArchetype document.
        /// </summary>
        [XmlElement("CuttingToolArchetypeReference")]
        [JsonPropertyName("cuttingToolArchetypeReference")]
        public string CuttingToolArchetypeReference { get; set; }


        public CuttingToolAsset()
        {
            Type = TypeId;
            CuttingToolLifeCycle = new CuttingToolLifeCycle();
        }


        protected override IAsset OnProcess(Version mtconnectVersion)
        {
            if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version12)
            {
                var asset = new CuttingToolAsset();
                asset.AssetId = AssetId;
                asset.InstanceId = InstanceId;
                asset.Type = Type;
                asset.Timestamp = Timestamp;
                if (mtconnectVersion > MTConnectVersions.Version13) asset.DeviceUuid = DeviceUuid;
                asset.Removed = Removed;
                asset.Description = Description;
                asset.Hash = Hash;

                if (!string.IsNullOrEmpty(SerialNumber)) asset.SerialNumber = SerialNumber;
                else asset.SerialNumber = AssetId;

                asset.ToolId = ToolId;
                asset.Manufacturers = Manufacturers;
                asset.CuttingToolLifeCycle = CuttingToolLifeCycle.Process();
                asset.CuttingToolArchetypeReference = CuttingToolArchetypeReference;

                return asset;
            }

            return null;         
        }

        public override AssetValidationResult IsValid(Version mtconnectVersion)
        {
            var message = "";
            var result = true;

            if (string.IsNullOrEmpty(SerialNumber))
            {
                message = "SerialNumber property is Required";
                result = false;
            }
            else if (string.IsNullOrEmpty(ToolId))
            {
                message = "ToolId property is Required";
                result = false;
            }

            return new AssetValidationResult(result, message);
        }


		public override string GenerateHash()
		{
			return GenerateHash(this);
		}

		public static string GenerateHash(CuttingToolAsset asset)
		{
			if (asset != null)
			{
				var ids = new List<string>();

				ids.Add(ObjectExtensions.GetHashPropertyString(asset).ToSHA1Hash());

                // Need to include CuttingItems

				return StringFunctions.ToSHA1Hash(ids.ToArray());
			}

			return null;
		}
	}
}