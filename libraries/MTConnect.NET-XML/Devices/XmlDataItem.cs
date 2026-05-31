// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>DataItem</c>. Mirrors the
    /// <c>DataItem</c> element of an MTConnectDevices document so the XML
    /// serializer can read and write the on-the-wire attribute and child-element
    /// shape, then converts to and from the strongly-typed <see cref="DataItem"/>
    /// model.
    /// </summary>
    public class XmlDataItem
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlDataItem));


        /// <summary>
        /// The <c>category</c> attribute classifying the data item as SAMPLE,
        /// EVENT, or CONDITION.
        /// </summary>
        [XmlAttribute("category")]
        public string DataItemCategory { get; set; }

        /// <summary>
        /// The unique <c>id</c> of the data item within the device.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The MTConnect <c>type</c> identifying the kind of data reported.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The coordinate system the reported values are expressed in (for
        /// example MACHINE or WORK).
        /// </summary>
        [XmlAttribute("coordinateSystem")]
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of a CoordinateSystem component the
        /// values are expressed relative to.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The optional human-readable <c>name</c> of the data item.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the Composition the data item belongs to.
        /// </summary>
        [XmlAttribute("compositionId")]
        public string CompositionId { get; set; }

        /// <summary>
        /// The power-of-ten scaling applied to native values before conversion
        /// to the reported units.
        /// </summary>
        [XmlAttribute("nativeScale")]
        public int NativeScale { get; set; }

        /// <summary>
        /// The units the data source natively reports values in, prior to
        /// conversion to the MTConnect <see cref="Units"/>.
        /// </summary>
        [XmlAttribute("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// The optional <c>subType</c> further qualifying the data item's
        /// <see cref="Type"/>.
        /// </summary>
        [XmlAttribute("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The statistical operation (for example AVERAGE or MAXIMUM) applied
        /// to the reported values.
        /// </summary>
        [XmlAttribute("statistic")]
        public string Statistic { get; set; }

        /// <summary>
        /// The engineering units the reported values are expressed in.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// The rate, in samples per second, at which the data source samples
        /// values for a TIME_SERIES representation.
        /// </summary>
        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        /// <summary>
        /// Indicates whether the reported values are discrete events rather
        /// than a continuously varying signal.
        /// </summary>
        [XmlAttribute("discrete")]
        public string Discrete { get; set; }

        /// <summary>
        /// The data item's <c>representation</c> (for example VALUE, DATA_SET,
        /// TABLE, or TIME_SERIES).
        /// </summary>
        [XmlAttribute("representation")]
        public string Representation { get; set; }

        /// <summary>
        /// The number of significant digits retained when reporting numeric
        /// values.
        /// </summary>
        [XmlAttribute("significantDigits")]
        public int SignificantDigits { get; set; }

        /// <summary>
        /// The <c>Source</c> element identifying the component, sensor, or data
        /// item the values originate from.
        /// </summary>
        [XmlElement("Source")]
        public XmlSource Source { get; set; }

        /// <summary>
        /// The <c>Constraints</c> element bounding the values the data item may
        /// report.
        /// </summary>
        [XmlElement("Constraints")]
        public XmlConstraints Constraints { get; set; }

        /// <summary>
        /// The <c>Filters</c> applied to the data item's values, such as a
        /// minimum-delta or period filter.
        /// </summary>
        [XmlArray("Filters")]
        [XmlArrayItem("Filter")]
        public List<XmlFilter> Filters { get; set; }

        /// <summary>
        /// The starting value reported for the data item before any
        /// observation is recorded.
        /// </summary>
        [XmlElement("InitialValue")]
        public string InitialValue { get; set; }

        /// <summary>
        /// The condition under which a measured or accumulated value resets.
        /// </summary>
        [XmlElement("ResetTrigger")]
        public string ResetTrigger { get; set; }

        /// <summary>
        /// The <c>Definition</c> element providing additional descriptive
        /// metadata, including entry and cell definitions for DATA_SET and
        /// TABLE data items.
        /// </summary>
        [XmlElement("Definition")]
        public XmlDataItemDefinition Definition { get; set; }

        /// <summary>
        /// The relationships associating this data item with other data items
        /// or specifications.
        /// </summary>
        [XmlArray("Relationships")]
        [XmlArrayItem("DataItemRelationship", typeof(XmlDataItemRelationship))]
        [XmlArrayItem("SpecificationRelationship", typeof(XmlSpecificationRelationship))]
        public List<XmlAbstractDataItemRelationship> Relationships { get; set; }


        /// <summary>
        /// Converts this XML surrogate into the strongly-typed
        /// <see cref="DataItem"/> model, resolving enum-valued attributes and
        /// projecting the nested Source, Constraints, Definition, Filters, and
        /// Relationships.
        /// </summary>
        public DataItem ToDataItem()
        {
            var dataItem = DataItem.Create(Type);
            if (dataItem == null) dataItem = new DataItem();

            if (!string.IsNullOrEmpty(DataItemCategory)) dataItem.Category = DataItemCategory.ConvertEnum<DataItemCategory>();
            dataItem.Id = Id;
            dataItem.Name = Name;
            dataItem.Type = Type;
            dataItem.SubType = SubType;
            dataItem.NativeUnits = NativeUnits;
            dataItem.NativeScale = NativeScale;
            dataItem.SampleRate = SampleRate;
            dataItem.CompositionId = CompositionId;
            if (!string.IsNullOrEmpty(Representation)) dataItem.Representation = Representation.ConvertEnum<DataItemRepresentation>();
            if (!string.IsNullOrEmpty(ResetTrigger)) dataItem.ResetTrigger = ResetTrigger.ConvertEnum<DataItemResetTrigger>();
            if (!string.IsNullOrEmpty(CoordinateSystem)) dataItem.CoordinateSystem = CoordinateSystem.ConvertEnum<DataItemCoordinateSystem>();
            dataItem.CoordinateSystemIdRef = CoordinateSystemIdRef;
            dataItem.Units = Units;
            if (!string.IsNullOrEmpty(Statistic)) dataItem.Statistic = Statistic.ConvertEnum<DataItemStatistic>();
            dataItem.SignificantDigits = SignificantDigits;
            dataItem.InitialValue = InitialValue;
            dataItem.Discrete = Discrete.ToBoolean();

            // Source
            if (Source != null) dataItem.Source = Source.ToSource();

            // Constraints
            if (Constraints != null) dataItem.Constraints = Constraints.ToConstraints();

            // Definition
            if (Definition != null) dataItem.Definition = Definition.ToDefinition();

            // Filters
            if (!Filters.IsNullOrEmpty())
            {
                var filters = new List<IFilter>();
                foreach (var filter in Filters)
                {
                    filters.Add(filter.ToFilter());
                }
                dataItem.Filters = filters;
            }

            // Relationships
            if (!Relationships.IsNullOrEmpty())
            {
                var relationships = new List<IAbstractDataItemRelationship>();
                foreach (var relationship in Relationships)
                {
                    relationships.Add(relationship.ToRelationship());
                }
                dataItem.Relationships = relationships;
            }

            return dataItem;
        }


        /// <summary>
        /// Deserializes a single <c>DataItem</c> XML document from
        /// <paramref name="xmlBytes"/> and returns it as an
        /// <see cref="IDataItem"/>, or <c>null</c> when the bytes are empty or
        /// not well-formed.
        /// </summary>
        public static IDataItem FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    using (var textReader = new MemoryStream(xmlBytes))
                    {
                        using (var xmlReader = XmlReader.Create(textReader))
                        {
                            var xmlObj = (XmlDataItem)_serializer.Deserialize(xmlReader);
                            if (xmlObj != null)
                            {
                                return xmlObj.ToDataItem();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }


        /// <summary>
        /// Writes <paramref name="dataItem"/> as a <c>DataItem</c> element to
        /// <paramref name="writer"/>, emitting only the attributes that differ
        /// from their MTConnect defaults. When <paramref name="outputComments"/>
        /// is <c>true</c>, the type and subType descriptions are written as
        /// preceding XML comments.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IDataItem dataItem, bool outputComments = false)
        {
            if (dataItem != null)
            {
                // Add Comments
                if (outputComments && dataItem != null)
                {
                    // Write DataItem Type Description as Comment
                    if (!string.IsNullOrEmpty(dataItem.TypeDescription))
                    {
                        writer.WriteComment($"Type = {dataItem.Type} : {dataItem.TypeDescription}");
                    }

                    // Write DataItem SubType Description as Comment
                    if (!string.IsNullOrEmpty(dataItem.SubType) && !string.IsNullOrEmpty(dataItem.SubTypeDescription))
                    {
                        writer.WriteComment($"SubType = {dataItem.SubType} : {dataItem.SubTypeDescription}");
                    }
                }

                writer.WriteStartElement("DataItem");

                // Write DataItem Properties
                writer.WriteAttributeString("category", dataItem.Category.ToString());
                writer.WriteAttributeString("id", dataItem.Id);
                if (!string.IsNullOrEmpty(dataItem.Name)) writer.WriteAttributeString("name", dataItem.Name);
                writer.WriteAttributeString("type", dataItem.Type);
                if (!string.IsNullOrEmpty(dataItem.SubType)) writer.WriteAttributeString("subType", dataItem.SubType);
                if (dataItem.CoordinateSystem != DataItemCoordinateSystem.MACHINE) writer.WriteAttributeString("coordinateSystem", dataItem.CoordinateSystem.ToString());
                if (!string.IsNullOrEmpty(dataItem.CoordinateSystemIdRef)) writer.WriteAttributeString("coordinateSystemIdRef", dataItem.CoordinateSystemIdRef);
                if (dataItem.NativeScale > 0) writer.WriteAttributeString("nativeScale", dataItem.NativeScale.ToString());
                if (!string.IsNullOrEmpty(dataItem.NativeUnits)) writer.WriteAttributeString("nativeUnits", dataItem.NativeUnits);
                if (!string.IsNullOrEmpty(dataItem.Units)) writer.WriteAttributeString("units", dataItem.Units);
                if (dataItem.Statistic != null) writer.WriteAttributeString("statistic", dataItem.Statistic.ToString());
                if (dataItem.SampleRate > 0) writer.WriteAttributeString("sampleRate", dataItem.SampleRate.ToString());
                if (dataItem.Discrete) writer.WriteAttributeString("discrete", dataItem.Discrete.ToString());
                if (dataItem.Representation != DataItemRepresentation.VALUE) writer.WriteAttributeString("representation", dataItem.Representation.ToString());
                if (dataItem.SignificantDigits > 0) writer.WriteAttributeString("significantDigits", dataItem.SignificantDigits.ToString());
                if (!string.IsNullOrEmpty(dataItem.CompositionId)) writer.WriteAttributeString("compositionId", dataItem.CompositionId);
                if (!string.IsNullOrEmpty(dataItem.InitialValue)) writer.WriteAttributeString("initialValue", dataItem.InitialValue);
                if (dataItem.ResetTrigger != null) writer.WriteAttributeString("resetTrigger", dataItem.ResetTrigger.ToString());


                // Write Source
                XmlSource.WriteXml(writer, dataItem.Source);

                // Write Constraints
                XmlConstraints.WriteXml(writer, dataItem.Constraints);

                // Write Filters
                if (!dataItem.Filters.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Filters");
                    foreach (var filter in dataItem.Filters)
                    {
                        XmlFilter.WriteXml(writer, filter);
                    }
                    writer.WriteEndElement();
                }

                // Write Definition
                XmlDataItemDefinition.WriteXml(writer, dataItem.Definition);

                // Write Relationships
                if (!dataItem.Relationships.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Relationships");
                    foreach (var relationship in dataItem.Relationships)
                    {
                        if (typeof(IDataItemRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            XmlDataItemRelationship.WriteXml(writer, (IDataItemRelationship)relationship);
                        }
                        else if (typeof(ISpecificationRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            XmlSpecificationRelationship.WriteXml(writer, (ISpecificationRelationship)relationship);
                        }
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}