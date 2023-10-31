// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Assets.QIF
{
    public class XmlQIFDocument : IXmlSerializable
    {
        public string QIFDocument { get; set; }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            // Use static WriteXml()
        }

        public void ReadXml(XmlReader reader)
        {
            QIFDocument = reader.ReadOuterXml();

            // Advance Reader
            reader.Skip();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion

    }
}
