// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements.
    /// There can be mulitple types of DataItem XML Elements in the document.
    /// </summary>
    [XmlRoot("DataItem")]
    public class XmlDataItem
    {
        /// <summary>
        /// The XPath address of the DataItem
        /// </summary>
        [XmlIgnore]
        public string XPath { get; set; }

        /// <summary>
        /// The path of the DataItem by Type
        /// </summary>
        [XmlIgnore]
        public string TypePath { get; set; }


        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// Each category of information will provide similar characteristics in its representation.
        /// The available options are SAMPLE, EVENT, or CONDITION.
        /// </summary>
        [XmlAttribute("category")]
        public DataItemCategory DataItemCategory { get; set; }

        /// <summary>
        /// The unique identifier for this DataItem.
        /// The id attribute MUST be unique across the entire document including the ids for components.
        /// An XML ID-type.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        [XmlAttribute("type")]
         public string Type { get; set; }

        /// <summary>
        /// The coordinate system being used.
        /// The available values for coordinateSystem are WORK and MACHINE.
        /// </summary>
        [XmlAttribute("coordinateSystem")]
        public DataItemCoordinateSystem CoordinateSystem { get; set; }

        [XmlIgnore]
        public bool CoordinateSystemSpecified => CoordinateSystem != DataItemCoordinateSystem.MACHINE;

        /// <summary>
        /// The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.
        /// It is not required and will be implementation dependent.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The multiplier for the native units.
        /// The received data MAY be divided by this value before conversion.
        /// If provided, the value MUST be numeric.
        /// </summary>
        [XmlAttribute("nativeScale")]
        public double NativeScale { get; set; }

        [XmlIgnore]
        public bool NativeScaleSpecified => NativeScale > 0;

        /// <summary>
        /// The native units used by the Component.
        /// These units will be converted before they are delivered to the application.
        /// </summary>
        [XmlAttribute("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        [XmlAttribute("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Data calculated specific to a DataItem.
        /// Examples of statistic are AVERAGE, MINIMUM, MAXIMUM, ROOT_MEAN_SQUARE, RANGE, MEDIAN, MODE and STANDARD_DEVIATION.
        /// </summary>
        [XmlAttribute("statistic")]
        public DataItemStatistic Statistic { get; set; }

        [XmlIgnore]
        public bool StatisticSpecified => Statistic != DataItemStatistic.NONE;

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// The reate at which successive samples of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a floating point number.
        /// For example, a rate 1 per 10 seconds would be 0.1.
        /// </summary>
        [XmlAttribute("sampleRate")]
        public double SampleRate { get; set; }

        [XmlIgnore]
        public bool SampleRateSpecified => SampleRate > 0;

        /// <summary>
        /// An indication signifying whether each value reported for the Data Entity is significant and whether duplicate values are to be suppressed.
        /// </summary>
        [XmlAttribute("discrete")]
        public bool Discrete { get; set; }

        [XmlIgnore]
        public bool DiscreteSpecified => Discrete;

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        [XmlAttribute("representation")]
        public DataItemRepresentation Representation { get; set; }

        [XmlIgnore]
        public bool RepresentationSpecified => Representation != DataItemRepresentation.VALUE;

        /// <summary>
        /// The number of significant digits in the reported value.
        /// This is used by applications to dtermine accuracy of values.
        /// This SHOULD be specified for all numeric values.
        /// </summary>
        [XmlAttribute("significantDigits")]
        public int SignificantDigits { get; set; }

        [XmlIgnore]
        public bool SignificantDigitsSpecified => SignificantDigits > 0;

        /// <summary>
        /// Source is an XML element that indentifies the Component, Subcomponent, or DataItem representing the part of the device from which a measured value originates.
        /// </summary>
        [XmlElement("Source")]
        public Source Source { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        [XmlElement("Constraints")]
        public Constraints Constraints { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        [XmlArray("Filters")]
        [XmlArrayItem("Filter")]
        public List<Filter> Filters { get; set; }

        [XmlIgnore]
        public bool FiltersSpecified => !Filters.IsNullOrEmpty();

        /// <summary>
        /// InitialValue is an optional XML element that defines the starting value for a data item as well as the value to be set for the data item after a reset event.
        /// </summary>
        [XmlElement("InitialValue")]
        public string InitialValue { get; set; }

        /// <summary>
        /// ResetTrigger is an XML element that describes the reset action that causes a reset to occur.
        /// </summary>
        [XmlElement("ResetTrigger")]
        public DataItemResetTrigger ResetTrigger { get; set; }

        [XmlIgnore]
        public bool ResetTriggerSpecified => ResetTrigger != DataItemResetTrigger.NONE;

        /// <summary>
        /// The Definition provides additional descriptive information for any DataItem representations.
        /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
        /// </summary>
        [XmlElement("Definition")]
        public DataItemDefinition Definition { get; set; }

        /// <summary>
        /// Relationships organizes DataItemRelationship and SpecificationRelationship.
        /// </summary>
        [XmlArray("Relationships")]
        [XmlArrayItem("DataItemRelationship", typeof(DataItemRelationship))]
        [XmlArrayItem("SpecificationRelationship", typeof(SpecificationRelationship))]
        public List<Relationship> Relationships { get; set; }

        [XmlIgnore]
        public bool RelationshipsSpecified => !Relationships.IsNullOrEmpty();


        public XmlDataItem() { }

        public XmlDataItem(DataItem dataItem)
        {
            if (dataItem != null)
            {
                DataItemCategory = dataItem.Category;
                Id = dataItem.Id;
                Name = dataItem.Name;
                Type = dataItem.Type;
                SubType = dataItem.SubType;
                NativeUnits = dataItem.NativeUnits;
                NativeScale = dataItem.NativeScale;
                SampleRate = dataItem.SampleRate;
                Source = dataItem.Source;
                Relationships = dataItem.Relationships;
                Representation = dataItem.Representation;
                ResetTrigger = dataItem.ResetTrigger;
                CoordinateSystem = dataItem.CoordinateSystem;
                Constraints = dataItem.Constraints;
                Definition = dataItem.Definition;
                Units = dataItem.Units;
                Statistic = dataItem.Statistic;
                SignificantDigits = dataItem.SignificantDigits;
                Filters = dataItem.Filters;
                InitialValue = dataItem.InitialValue;
                Discrete = dataItem.Discrete;
            }
        }

        public DataItem ToDataItem()
        {
            var dataItem = DataItem.Create(Type);
            if (dataItem == null) dataItem = new DataItem();

            dataItem.Category = DataItemCategory;
            dataItem.Id = Id;
            dataItem.Name = Name;
            dataItem.Type = Type;
            dataItem.SubType = SubType;
            dataItem.NativeUnits = NativeUnits;
            dataItem.NativeScale = NativeScale;
            dataItem.SampleRate = SampleRate;
            dataItem.Source = Source;
            dataItem.Relationships = Relationships;
            dataItem.Representation = Representation;
            dataItem.ResetTrigger = ResetTrigger;
            dataItem.CoordinateSystem = CoordinateSystem;
            dataItem.Constraints = Constraints;
            dataItem.Definition = Definition;
            dataItem.Units = Units;
            dataItem.Statistic = Statistic;
            dataItem.SignificantDigits = SignificantDigits;
            dataItem.Filters = Filters;
            dataItem.InitialValue = InitialValue;
            dataItem.Discrete = Discrete;

            return dataItem;
        }
    }
}
