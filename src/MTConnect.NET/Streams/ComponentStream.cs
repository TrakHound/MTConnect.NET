// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public class ComponentStream
    {
        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        [XmlAttribute("component")]
        [JsonPropertyName("component")]
        public string Component { get; set; }

        /// <summary>
        /// The identifier of the Structural Element as defined by the id attribute of the corresponding Structural Element in the MTConnectDevices XML document.
        /// </summary>
        [XmlAttribute("componentId")]
        [JsonPropertyName("componentId")]
        public string ComponentId { get; set; }

        /// <summary>
        /// The name of the ComponentStream element.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// NativeName identifies the common name normally associated with the ComponentStream element.
        /// </summary>
        [XmlAttribute("nativeName")]
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// Uuid of the ComponentStream element.
        /// </summary>
        [XmlAttribute("uuid")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public List<IDataItem> DataItems
        {
            get
            {
                var l = new List<IDataItem>();

                if (Samples != null) l.AddRange(Samples);
                if (Events != null) l.AddRange(Events);
                if (Conditions != null) l.AddRange(Conditions);

                return l;
            }
        }

        [XmlIgnore]
        [JsonPropertyName("samples")]
        public IEnumerable<Sample> Samples { get; set; }

        [XmlIgnore]
        [JsonPropertyName("events")]
        public IEnumerable<Event> Events { get; set; }

        [XmlIgnore]
        [JsonPropertyName("condition")]
        public IEnumerable<Condition> Conditions { get; set; }


        public void AddSample(Sample sampleDataItem)
        {
            if (sampleDataItem != null)
            {
                var objs = Samples?.ToList();
                if (objs == null) objs = new List<Sample>();
                objs.Add(sampleDataItem);
                Samples = objs;
            }
        }

        public void AddEvent(Event eventDataItem)
        {
            if (eventDataItem != null)
            {
                var objs = Events?.ToList();
                if (objs == null) objs = new List<Event>();
                objs.Add(eventDataItem);
                Events = objs;
            }
        }

        public void AddCondition(Condition conditionDataItem)
        {
            if (conditionDataItem != null)
            {
                var objs = Conditions?.ToList();
                if (objs == null) objs = new List<Condition>();
                objs.Add(conditionDataItem);
                Conditions = objs;
            }
        }
    }  
}
