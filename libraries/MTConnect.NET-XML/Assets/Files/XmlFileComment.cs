// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    /// <summary>
    /// XML serialization surrogate for a single <c>FileComment</c>, a
    /// timestamped free-text note attached to a File asset.
    /// </summary>
    public class XmlFileComment
    {
        /// <summary>
        /// When the comment was made, carried by the <c>timestamp</c>
        /// attribute in ISO 8601 round-trip form.
        /// </summary>
        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The comment text, carried as the element's text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="FileComment"/>.
        /// </summary>
        public IFileComment ToFileComment()
        {
            var fileComment = new FileComment();
            fileComment.Timestamp = Timestamp;
            fileComment.Value = Value;
            return fileComment;
        }

        /// <summary>
        /// Writes the <c>FileComments</c> container with one <c>FileComment</c>
        /// element per comment, formatting each timestamp in ISO 8601
        /// round-trip form; nothing is written when the collection is empty.
        /// </summary>
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