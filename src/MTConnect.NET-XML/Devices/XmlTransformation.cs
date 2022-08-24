// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Transformation")]
    public class XmlTransformation
    {
        [XmlElement("Translation")]
        public string Translation { get; set; }

        [XmlElement("Rotation")]
        public string Rotation { get; set; }


        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            transformation.Translation = Translation;
            transformation.Rotation = Rotation;
            return transformation;
        }

        public static void WriteXml(XmlWriter writer, ITransformation transformation)
        {
            if (transformation != null)
            {
                writer.WriteStartElement("Transformation");

                // Write Translation
                if (transformation.Translation != null)
                {
                    writer.WriteStartElement("Translation");
                    writer.WriteString(transformation.Translation);
                    writer.WriteEndElement();
                }

                // Write Rotation
                if (transformation.Rotation != null)
                {
                    writer.WriteStartElement("Rotation");
                    writer.WriteString(transformation.Rotation);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
