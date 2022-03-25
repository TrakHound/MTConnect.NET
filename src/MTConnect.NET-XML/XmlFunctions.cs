// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Writers;
using System.IO;
using System.Xml;

namespace MTConnect
{
    internal static class XmlFunctions
    {
        public static string FormatXml(string xml, bool indent = true, bool outputComments = false)
        {
            try
            {
                // Set XML Reader Settings
                var readerSettings = new XmlReaderSettings();
                readerSettings.ValidationType = ValidationType.None;
                readerSettings.IgnoreComments = !outputComments;

                // Set XML Reader Settings
                using (var stringReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(stringReader, readerSettings))
                {
                    var document = new XmlDocument();
                    document.Load(xmlReader);

                    // Set XML Writer Settings
                    var writerSettings = new XmlWriterSettings();
                    writerSettings.NewLineChars = "\r\n";
                    writerSettings.Indent = indent;
                    writerSettings.Encoding = System.Text.Encoding.UTF8;

                    using (var stringWriter = new Utf8Writer())
                    using (var xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
                    {
                        document.Save(xmlWriter);
                        return stringWriter.ToString();
                    }
                }
            }
            catch { }

            return xml;
        }
    }
}
