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
        public XmlScale Scale { get; set; }

        [XmlElement("ScaleDataSet")]
        public XmlScaleDataSet ScaleDataSet { get; set; }


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
