// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectStreams
{
    /// <summary>
    /// An XML container type element that may be provided in a MTConnectStreams XML document returned from a MTConnect Agent in response to a Current or Sample request that organizes data for a specific Structural Element of a device.
    /// </summary>
    public class ComponentStream
    {
        public ComponentStream()
        {
            Conditions = new DataItemCollection<Condition>();
            Events = new DataItemCollection<Event>();
            Samples = new DataItemCollection<Sample>();
        }

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

        public List<DataItem> DataItems
        {
            get
            {
                var l = new List<DataItem>();

                if (Conditions != null) l.AddRange(Conditions.DataItems);
                if (Events != null) l.AddRange(Events.DataItems);
                if (Samples != null) l.AddRange(Samples.DataItems);

                return l;
            }
        }

        [XmlElement("Condition")]
        public DataItemCollection<Condition> Conditions { get; set; }

        [XmlElement("Events")]
        public DataItemCollection<Event> Events { get; set; }

        [XmlElement("Samples")]
        public DataItemCollection<Sample> Samples { get; set; }

    }
    
}
