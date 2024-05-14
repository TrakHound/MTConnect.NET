// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    public class XmlFileComment
    {
        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlText]
        public string Value { get; set; }


        public IFileComment ToFileComment()
        {
            var fileComment = new FileComment();
            fileComment.Timestamp = Timestamp;
            fileComment.Value = Value;
            return fileComment;
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IFileComment> fileComments)
        {
            if (!fileComments.IsNullOrEmpty())
            {
                writer.WriteStartElement("FileComments");

                foreach (var fileComment in fileComments)
                {
                    writer.WriteStartElement("FileComment");
                    writer.WriteAttributeString("timestamp", fileComment.Timestamp.ToString("o"));
                    writer.WriteString(fileComment.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}