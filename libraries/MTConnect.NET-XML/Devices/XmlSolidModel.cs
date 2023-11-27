// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("SolidModel")]
    public class XmlSolidModel
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("solidModelIdRef")]
        public string SolidModelIdRef { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("itemRef")]
        public string ItemRef { get; set; }

        [XmlAttribute("mediaType")]
        public MediaType MediaType { get; set; }

        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        [XmlElement("Scale")]
        public string Scale { get; set; }


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
            solidModel.Scale = UnitVector3D.FromString(Scale);
            return solidModel;
        }

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

                // Write Scale
                if (solidModel.Scale != null)
                {
                    writer.WriteStartElement("Scale");
                    writer.WriteString(solidModel.Scale.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}