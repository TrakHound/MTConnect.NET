// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlReference
    {
        [XmlAttribute("idRef")]
        public string IdRef { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }


        public virtual IReference ToReference()
        {
            var reference = new Reference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }

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
