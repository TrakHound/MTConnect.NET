// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Xml;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    [XmlRoot("ComponentConfigurationParameters")]
    public class XmlComponentConfigurationParametersAsset : XmlAsset
    {
        [XmlArray("ParameterSets")]
        [XmlArrayItem("ParameterSet")]
        public List<XmlParameterSet> ParameterSets { get; set; }


        public override IComponentConfigurationParametersAsset ToAsset()
        {
            var asset = new ComponentConfigurationParametersAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            //if (Description != null) asset.Description = Description.ToDescription();

            if (!ParameterSets.IsNullOrEmpty())
            {
                var parameterSets = new List<IParameterSet>();
                foreach (var  parameterSet in ParameterSets)
                {
                    parameterSets.Add(parameterSet.ToParameterSet());
                }
                asset.ParameterSets = parameterSets;
            }

            return asset;
        }

        public static void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var componentConfigurationParametersAsset = (IComponentConfigurationParametersAsset)asset;

                writer.WriteStartElement("ComponentConfigurationParameters");

                WriteCommonXml(writer, asset);

                // ParameterSets
                if (!componentConfigurationParametersAsset.ParameterSets.IsNullOrEmpty())
                {
                    XmlParameterSet.WriteXml(writer, componentConfigurationParametersAsset.ParameterSets);
                }

                writer.WriteEndElement();
            }
        }
    }
}
