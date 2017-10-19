// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace MTConnect.MTConnectAssets
{
    [XmlRoot("MTConnectAssets")]
    public class Document
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof(Document));

        public Document() { }

        public static Document Create(string xml)
        {
            try
            {
                xml = xml.Trim();

                var version = MTConnect.Version.Get(xml);

                using (var textReader = new StringReader(Namespaces.Clear(xml)))
                using (var xmlReader = XmlReader.Create(textReader))
                {
                    var doc = (Document)serializer.Deserialize(xmlReader);
                    if (doc != null)
                    {
                        doc._version = version;
                        doc.Xml = xml;
                        return doc;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        [XmlElement("Header")]
        public Headers.MTConnectAssetsHeader Header { get; set; }

        [XmlElement("Assets")]
        public AssetCollection Assets { get; set; }

        protected double _version;
        [XmlIgnore]
        public double Version { get { return _version; } }

        [XmlIgnore]
        public object UserObject { get; set; }

        [XmlIgnore]
        public string Url { get; set; }

        [XmlIgnore]
        public string Xml { get; set; }
    }
}
