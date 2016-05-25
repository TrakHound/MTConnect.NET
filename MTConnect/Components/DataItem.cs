// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Xml;

namespace MTConnect.Components
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements.
    /// There can be mulitple types of DataItem XML Elements in the document.
    /// </summary>
    public class DataItem
    {
        public DataItem() { }

        public DataItem(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            FullAddress = Tools.Address.GetComponents(node);

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    switch (child.Name.ToLower())
                    {
                        case "source":

                            Source = new Source(child);
                            break;

                        case "constraints":

                            Constraints = new Constraints(child);

                            break;
                    }
                }
            }
        }

        #region "Required"

        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// Each category of information will provide similar characteristics in its representation.
        /// The available options are SAMPLE, EVENT, or CONDITION.
        /// </summary>
        public DataItemCategory Category { get; set; }

        /// <summary>
        /// The unique identifier for this DataItem.
        /// The id attribute MUST be unique across the entire document including the ids for components.
        /// An XML ID-type.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        public string Type { get; set; }

        #endregion

        #region "Optional"

        /// <summary>
        /// The coordinate system being used.
        /// The available values for coordinateSystem are WORK and MACHINE.
        /// </summary>
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.
        /// It is not required and will be implementation dependent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The multiplier for the native units.
        /// The received data MAY be divided by this value before conversion.
        /// If provided, the value MUST be numeric.
        /// </summary>
        public string NativeScale { get; set; }

        /// <summary>
        /// The native units used by the Component.
        /// These units will be converted before they are delivered to the application.
        /// </summary>
        public string NativeUnits { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// Data calculated specific to a DataItem.
        /// Examples of statistic are AVERAGE, MINIMUM, MAXIMUM, ROOT_MEAN_SQUARE, RANGE, MEDIAN, MODE and STANDARD_DEVIATION.
        /// </summary>
        public string Statistic { get; set; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// The reate at which successive samples of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a floating point number.
        /// For example, a rate 1 per 10 seconds would be 0.1.
        /// </summary>
        public string SampleRate { get; set; }

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        public string Representation { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value.
        /// This is used by applications to dtermine accuracy of values.
        /// This SHOULD be specified for all numeric values.
        /// </summary>
        public string SignificantDigits { get; set; }

        #endregion

        #region "Sub-Elements"

        /// <summary>
        /// Source is an XML element that indentifies the Component, Subcomponent, or DataItem representing the part of the device from which a measured value originates.
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        public Constraints Constraints { get; set; }

        #endregion


        /// <summary>
        /// Full XML XPath address of the DataItem.
        /// (Added for TrakHound)
        /// </summary>
        public string FullAddress { get; set; }

    }


    public enum DataItemCategory
    {
        CONDITION,
        EVENT,
        SAMPLE
    }
}
