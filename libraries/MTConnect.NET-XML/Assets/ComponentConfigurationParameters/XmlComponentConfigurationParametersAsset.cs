// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Xml;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// XML serialization surrogate for a <c>ComponentConfigurationParameters</c>
    /// asset, which captures the configurable parameters of a component grouped
    /// into named parameter sets.
    /// </summary>
    [XmlRoot("ComponentConfigurationParameters")]
    public class XmlComponentConfigurationParametersAsset : XmlAsset
    {
        /// <summary>
        /// The parameter sets carried by the asset, serialized as
        /// <c>ParameterSet</c> elements within <c>ParameterSets</c>.
        /// </summary>
        [XmlArray("ParameterSets")]
        [XmlArrayItem("ParameterSet")]
        public List<XmlParameterSet> ParameterSets { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ComponentConfigurationParametersAsset"/>, copying the
        /// shared asset fields and converting each parameter set.
        /// </summary>
        public override IAsset ToAsset()
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
                foreach (var parameterSet in ParameterSets)
                {
                    parameterSets.Add(parameterSet.ToParameterSet());
                }
                asset.ParameterSets = parameterSets;
            }

            return asset;
        }

        /// <summary>
        /// Writes the <c>ComponentConfigurationParameters</c> element, emitting
        /// the shared asset attributes followed by the parameter sets when
        /// present.
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
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
