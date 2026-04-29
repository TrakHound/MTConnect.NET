// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <Transformation> of type TransformationType. Translation/TranslationDataSet
//   form one xs:choice and Rotation/RotationDataSet form a separate xs:choice within
//   the same sequence (each independently optional).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Transformation (UML ID `_19_0_3_45f01b9_1579103900791_417826_16362`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Transformation")]
    public class XmlTransformation
    {
        [XmlElement("Translation")]
        public XmlTranslation Translation { get; set; }

        [XmlElement("TranslationDataSet")]
        public XmlTranslationDataSet TranslationDataSet { get; set; }

        [XmlElement("Rotation")]
        public XmlRotation Rotation { get; set; }

        [XmlElement("RotationDataSet")]
        public XmlRotationDataSet RotationDataSet { get; set; }


        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            if (TranslationDataSet != null) transformation.Translation = TranslationDataSet.ToTranslationDataSet();
            else if (Translation != null) transformation.Translation = Translation.ToTranslation();
            if (RotationDataSet != null) transformation.Rotation = RotationDataSet.ToRotationDataSet();
            else if (Rotation != null) transformation.Rotation = Rotation.ToRotation();
            return transformation;
        }

        public static void WriteXml(XmlWriter writer, ITransformation transformation)
        {
            if (transformation != null)
            {
                writer.WriteStartElement("Transformation");

                // Write Translation (or TranslationDataSet)
                if (transformation.Translation is ITranslationDataSet translationDataSet)
                {
                    XmlTranslationDataSet.WriteXml(writer, translationDataSet);
                }
                else if (transformation.Translation is ITranslation translation)
                {
                    XmlTranslation.WriteXml(writer, translation);
                }

                // Write Rotation (or RotationDataSet)
                if (transformation.Rotation is IRotationDataSet rotationDataSet)
                {
                    XmlRotationDataSet.WriteXml(writer, rotationDataSet);
                }
                else if (transformation.Rotation is IRotation rotation)
                {
                    XmlRotation.WriteXml(writer, rotation);
                }

                writer.WriteEndElement();
            }
        }
    }
}
