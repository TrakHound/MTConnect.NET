// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Base XML serialization surrogate shared by the data-item relationship
    /// surrogates (for example <c>DataItemRelationship</c> and
    /// <c>SpecificationRelationship</c>), carrying the common
    /// <c>name</c>/<c>idRef</c> attributes.
    /// </summary>
    public abstract class XmlAbstractDataItemRelationship
    {
        /// <summary>
        /// The optional relationship name, carried by the <c>name</c> attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The <c>id</c> of the related entity, carried by the <c>idRef</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("idRef")]
        public string IdRef { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed relationship. The base
        /// implementation returns <c>null</c>; concrete subclasses override it.
        /// </summary>
        public virtual IAbstractDataItemRelationship ToRelationship() { return null; }

        /// <summary>
        /// Writes the relationship attributes shared by every subclass,
        /// emitting <c>idRef</c> and <c>name</c> only when populated.
        /// </summary>
        public static void WriteCommonXml(XmlWriter writer, IAbstractDataItemRelationship relationship)
        {
            if (!string.IsNullOrEmpty(relationship.IdRef)) writer.WriteAttributeString("idRef", relationship.IdRef);
            if (!string.IsNullOrEmpty(relationship.Name)) writer.WriteAttributeString("name", relationship.Name);
        }
    }
}