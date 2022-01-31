// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public class XmlComponentStream : ComponentStream
    {

        private XmlSamplesContainer _samplesContainer;
        [XmlElement("Samples")]
        public XmlSamplesContainer SamplesContainer
        {
            get
            {
                return _samplesContainer;
            }
            set
            {
                _samplesContainer = value;
                if (_samplesContainer != null && !_samplesContainer.Samples.IsNullOrEmpty())
                {
                    foreach (var dataItem in _samplesContainer.Samples.ToList()) AddSample(dataItem);
                }
            }
        }


        private XmlEventsContainer _eventsContainer;
        [XmlElement("Events")]
        public XmlEventsContainer EventsContainer
        {
            get
            {
                return _eventsContainer;
            }
            set
            {
                _eventsContainer = value;
                if (_eventsContainer != null && !_eventsContainer.Events.IsNullOrEmpty())
                {
                    foreach (var dataItem in _eventsContainer.Events.ToList()) AddEvent(dataItem);
                }
            }
        }


        private XmlConditionsContainer _conditionsContainer;
        [XmlElement("Condition")]
        public XmlConditionsContainer ConditionsContainer
        {
            get
            {
                return _conditionsContainer;
            }
            set
            {
                _conditionsContainer = value;
                if (_conditionsContainer != null && !_conditionsContainer.Conditions.IsNullOrEmpty())
                {
                    foreach (var dataItem in _conditionsContainer.Conditions.ToList()) AddCondition(dataItem);
                }
            }
        }


        public XmlComponentStream() { }

        public XmlComponentStream(ComponentStream componentStream)
        {
            if (componentStream != null)
            {
                Component = componentStream.Component;
                ComponentId = componentStream.ComponentId;
                Name = componentStream.Name;
                NativeName = componentStream.NativeName;
                Uuid = componentStream.Uuid;

                // Add Samples
                if (!componentStream.Samples.IsNullOrEmpty())
                {
                    SamplesContainer = new XmlSamplesContainer
                    {
                        Samples = componentStream.Samples.ToList()
                    };
                }

                // Add Events
                if (!componentStream.Events.IsNullOrEmpty())
                {
                    EventsContainer = new XmlEventsContainer
                    {
                        Events = componentStream.Events.ToList()
                    };
                }

                // Add Conditions
                if (!componentStream.Conditions.IsNullOrEmpty())
                {
                    ConditionsContainer = new XmlConditionsContainer
                    {
                        Conditions = componentStream.Conditions.ToList()
                    };
                }
            }
        }


        public ComponentStream ToComponentStream()
        {
            var componentStream = new ComponentStream();

            componentStream.Component = Component;
            componentStream.ComponentId = ComponentId;
            componentStream.Name = Name;
            componentStream.NativeName = NativeName;
            componentStream.Uuid = Uuid;

            // Add Samples
            if (SamplesContainer != null && !SamplesContainer.Samples.IsNullOrEmpty())
            {
                componentStream.Samples = SamplesContainer.Samples.ToList();
            }

            // Add Events
            if (EventsContainer != null && !EventsContainer.Events.IsNullOrEmpty())
            {
                componentStream.Events = EventsContainer.Events.ToList();
            }

            // Add Conditions
            if (ConditionsContainer != null && !ConditionsContainer.Conditions.IsNullOrEmpty())
            {
                componentStream.Conditions = ConditionsContainer.Conditions.ToList();
            }

            return componentStream;
        }
    }  
}
