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
    /// <summary>
    /// XML serialization surrogate for a <c>Transformation</c>, the
    /// translation and rotation that position a component relative to its
    /// parent. Mirrors the on-the-wire element and converts to and from the
    /// strongly-typed <see cref="Transformation"/> model.
    /// </summary>
    [XmlRoot("Transformation")]
    public class XmlTransformation
    {
        /// <summary>
        /// The translation expressed as a coordinate triple; mutually
        /// exclusive with <see cref="TranslationDataSet"/>.
        /// </summary>
        [XmlElement("Translation")]
        public XmlTranslation Translation { get; set; }

        /// <summary>
        /// The translation expressed as a data set; mutually exclusive with
        /// <see cref="Translation"/>.
        /// </summary>
        [XmlElement("TranslationDataSet")]
        public XmlTranslationDataSet TranslationDataSet { get; set; }

        /// <summary>
        /// The rotation expressed as a coordinate triple; mutually exclusive
        /// with <see cref="RotationDataSet"/>.
        /// </summary>
        [XmlElement("Rotation")]
        public XmlRotation Rotation { get; set; }

        /// <summary>
        /// The rotation expressed as a data set; mutually exclusive with
        /// <see cref="Rotation"/>.
        /// </summary>
        [XmlElement("RotationDataSet")]
        public XmlRotationDataSet RotationDataSet { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Transformation"/>, resolving the translation and rotation
        /// choices in favour of the data-set form when present.
        /// </summary>
        public ITransformation ToTransformation()
        {
            var transformation = new Transformation();
            if (TranslationDataSet != null) transformation.Translation = TranslationDataSet.ToTranslationDataSet();
            else if (Translation != null) transformation.Translation = Translation.ToTranslation();
            if (RotationDataSet != null) transformation.Rotation = RotationDataSet.ToRotationDataSet();
            else if (Rotation != null) transformation.Rotation = Rotation.ToRotation();
            return transformation;
        }

        /// <summary>
        /// Writes the given <see cref="ITransformation"/> to
        /// <paramref name="writer"/> as a <c>Transformation</c> element,
        /// emitting the data-set form of translation and rotation when the
        /// model carries it.
        /// </summary>
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
