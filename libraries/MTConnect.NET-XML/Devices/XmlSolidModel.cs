// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <SolidModel> of type SolidModelType. Scale/ScaleDataSet form an xs:choice
//   (only one is permitted); Transformation is a separate optional element.
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class SolidModel (UML ID `_19_0_3_45f01b9_1587596157073_106480_480`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>SolidModel</c>, the
    /// reference to a 3D geometry file describing a component. Mirrors the
    /// on-the-wire element and converts to and from the strongly-typed
    /// <see cref="SolidModel"/> model.
    /// </summary>
    [XmlRoot("SolidModel")]
    public class XmlSolidModel
    {
        /// <summary>
        /// The unique identifier of the solid model within the device.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The <c>id</c> of another solid model this one is defined relative
        /// to.
        /// </summary>
        [XmlAttribute("solidModelIdRef")]
        public string SolidModelIdRef { get; set; }

        /// <summary>
        /// The URL the model file can be retrieved from.
        /// </summary>
        [XmlAttribute("href")]
        public string Href { get; set; }

        /// <summary>
        /// The identifier of the specific item within the referenced model
        /// file.
        /// </summary>
        [XmlAttribute("itemRef")]
        public string ItemRef { get; set; }

        /// <summary>
        /// The format of the model file, such as <c>STEP</c> or <c>STL</c>.
        /// </summary>
        [XmlAttribute("mediaType")]
        public MediaType MediaType { get; set; }

        /// <summary>
        /// The <c>id</c> of the coordinate system the model is defined in.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The translation and rotation applied to position the model.
        /// </summary>
        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        /// <summary>
        /// The scale applied to the model expressed as a coordinate triple;
        /// mutually exclusive with <see cref="ScaleDataSet"/>.
        /// </summary>
        [XmlElement("Scale")]
        public XmlScale Scale { get; set; }

        /// <summary>
        /// The scale applied to the model expressed as a data set; mutually
        /// exclusive with <see cref="Scale"/>.
        /// </summary>
        [XmlElement("ScaleDataSet")]
        public XmlScaleDataSet ScaleDataSet { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="SolidModel"/>, resolving the Scale choice in favour of
        /// the data-set form when present.
        /// </summary>
        public ISolidModel ToSolidModel()
        {
            var solidModel = new SolidModel();
            solidModel.Id = Id;
            solidModel.SolidModelIdRef = Id;
            solidModel.Href = Href;
            solidModel.ItemRef = ItemRef;
            solidModel.MediaType = MediaType;
            solidModel.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (Transformation != null) solidModel.Transformation = Transformation.ToTransformation();
            if (ScaleDataSet != null) solidModel.Scale = ScaleDataSet.ToScaleDataSet();
            else if (Scale != null) solidModel.Scale = Scale.ToScale();
            return solidModel;
        }

        /// <summary>
        /// Writes the given <see cref="ISolidModel"/> to
        /// <paramref name="writer"/> as a <c>SolidModel</c> element, emitting
        /// the data-set form of Scale when the model carries it.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ISolidModel solidModel)
        {
            if (solidModel != null)
            {
                writer.WriteStartElement("SolidModel");

                // Write Properties
                writer.WriteAttributeString("id", solidModel.Id);
                if (!string.IsNullOrEmpty(solidModel.SolidModelIdRef)) writer.WriteAttributeString("solidModelIdRef", solidModel.SolidModelIdRef);
                if (!string.IsNullOrEmpty(solidModel.Href)) writer.WriteAttributeString("href", solidModel.Href);
                if (!string.IsNullOrEmpty(solidModel.ItemRef)) writer.WriteAttributeString("itemRef", solidModel.ItemRef);
                writer.WriteAttributeString("mediaType", solidModel.MediaType.ToString());
                if (!string.IsNullOrEmpty(solidModel.CoordinateSystemIdRef)) writer.WriteAttributeString("coordinateSystemIdRef", solidModel.CoordinateSystemIdRef);
                if (!string.IsNullOrEmpty(solidModel.Units)) writer.WriteAttributeString("units", solidModel.Units);
                if (!string.IsNullOrEmpty(solidModel.NativeUnits)) writer.WriteAttributeString("nativeUnits", solidModel.NativeUnits);

                // Write Transformation
                XmlTransformation.WriteXml(writer, solidModel.Transformation);

                // Write Scale (or ScaleDataSet)
                if (solidModel.Scale is IScaleDataSet scaleDataSet)
                {
                    XmlScaleDataSet.WriteXml(writer, scaleDataSet);
                }
                else if (solidModel.Scale is IScale scale)
                {
                    XmlScale.WriteXml(writer, scale);
                }

                writer.WriteEndElement();
            }
        }
    }
}
