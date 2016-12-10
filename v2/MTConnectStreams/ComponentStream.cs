// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.MTConnectStreams
{
    /// <summary>
    /// An XML container type element that may be provided in a MTConnectStreams XML document returned from a MTConnect Agent in response to a Current or Sample request that organizes data for a specific Structural Element of a device.
    /// </summary>
    public class ComponentStream
    {

        #region "Required"

        /// <summary>
        /// Id attribute of a device's Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        [XmlAttribute("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The type of Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        [XmlAttribute("component")]
        public string Component { get; set; }

        #endregion

        #region "Optional"

        /// <summary>
        /// Name attribute of the Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// NativeName attribute of the Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// Uuid attribute (unique identifier) of the Structural Element (Device, Component type or Subcomponent type) for which data is provided.
        /// </summary>
        [XmlAttribute("uuid")]
        public string Uuid { get; set; }

        #endregion

        //[XmlArray("Samples")]
        //[XmlElement("Events")]
        //[XmlElement("Conditions")]
        //public List<DataItem> DataItems { get; set; }

        [XmlArray("Conditions")]
        public List<object> Conditions { get; set; }

        [XmlArray("Events")]
        public List<object> Events { get; set; }

        [XmlArray("Samples")]
        public List<object> Samples { get; set; }

    }
}
