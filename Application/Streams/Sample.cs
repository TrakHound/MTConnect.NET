// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Xml;

namespace MTConnect.Application.Streams
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public class Sample : DataItem
    {
        public Sample() { }

        public Sample(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            FullAddress = Tools.Address.GetStreams(node);
            Category = DataItemCategory.SAMPLE;
            Type = node.Name;
            CDATA = node.InnerText;
        }

        /// <summary>
        /// The time elapsed since the statistic calculation was last reset.
        /// The duration attribute MUST be provided when the value of the data returned for the data item is a statistic.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// The number of readings of the value of a data item provided in the data returned when the representation attribute for teh data item is TIME_SERIES.
        /// SampleCount is not provided for data items unless the representation attribute is TIME_SERIES and it MUST be specified when the attribute is TIME_SERIES.
        /// </summary>
        public string SampleCount { get; set; }

        /// <summary>
        /// The rate at which successive samples of the value of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a decimal type floating point number.
        /// For example, a rate of 1 per 10 seconds would be 0.1.
        /// The SampleRate attribute MUST be provided when the representation attribute for the data item is TIME_SERIES.
        /// For data items where the representation attribute for the data item IS NOT TIME_SERIES, it may be assumed that the SampleRate is constant and SampleRate does not need to be reported in the MTConnectStreams document.
        /// </summary>
        public string SampleRate { get; set; }

        /// <summary>
        /// The type of statistical calculation specified in the Device Information Model that this Sample element represents.
        /// </summary>
        public string Statistic { get; set; }
               
    }
}
