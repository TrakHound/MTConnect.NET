// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Base XML serialization surrogate for an MTConnect reference (the
    /// <c>DataItemRef</c> / <c>ComponentRef</c> family). Carries the common
    /// <c>idRef</c>/<c>name</c> attributes and dispatches element naming on the
    /// concrete reference type.
    /// </summary>
    public class XmlReference
    {
        /// <summary>
        /// The <c>id</c> of the referenced data item or component, carried by
        /// the <c>idRef</c> attribute.
        /// </summary>
        [XmlAttribute("idRef")]
        public string IdRef { get; set; }

        /// <summary>
        /// The optional display name for the reference, carried by the
        /// <c>name</c> attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed reference. The base
        /// implementation returns <c>null</c>; concrete subclasses override it
        /// to produce the appropriate reference type.
        /// </summary>
        public virtual IReference ToReference() { return null; }

        //public virtual IReference ToReference()
        //{
        //    var reference = new Reference();
        //    reference.IdRef = IdRef;
        //    reference.Name = Name;
        //    return reference;
        //}

        /// <summary>
        /// Writes the reference element, selecting the element name
        /// (<c>DataItemRef</c>, <c>ComponentRef</c>, or generic
        /// <c>Reference</c>) from the concrete reference type and emitting the
        /// <c>idRef</c> and optional <c>name</c> attributes.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IReference reference)
        {
            if (reference != null)
            {
                var elementName = "Reference";
                switch (reference.GetType().Name)
                {
                    case "DataItemReference": elementName = "DataItemRef"; break;
                    case "ComponentReference": elementName = "ComponentRef"; break;
                }

                writer.WriteStartElement(elementName);

                // Write Properties
                writer.WriteAttributeString("idRef", reference.IdRef);
                if (!string.IsNullOrEmpty(reference.Name)) writer.WriteAttributeString("name", reference.Name);

                writer.WriteEndElement();
            }
        }
    }
}