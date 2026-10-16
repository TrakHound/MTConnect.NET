// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// XML serialization surrogate used when an asset element is encountered
    /// whose <c>assetType</c> is not recognized by the strongly-typed model.
    /// The raw XML is retained so the asset can still be round-tripped or
    /// resolved later once its concrete type is known.
    /// </summary>
    public class XmlUnknownAsset : XmlAsset
    {
        /// <summary>
        /// The raw XML fragment of the unrecognized asset element, preserved
        /// verbatim so no information is lost during deserialization.
        /// </summary>
        [XmlIgnore]
        public string Xml { get; set; }


        /// <summary>
        /// Deserializes the supplied XML bytes using the surrogate registered for the
        /// <paramref name="type"/> name; returns <c>null</c> when none is registered or the
        /// input cannot be parsed. Equivalent to <see cref="XmlAsset.FromXml(string, byte[])"/>.
        /// </summary>
        public static new IAsset FromXml(string type, byte[] xmlBytes)
        {
            return XmlAsset.FromXml(type, xmlBytes);
        }
    }
}
