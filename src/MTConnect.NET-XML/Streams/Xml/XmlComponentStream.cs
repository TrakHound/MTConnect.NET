// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public class XmlComponentStream : ComponentStream, IXmlSerializable
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
                    var objs = new List<SampleObservation>();
                    foreach (var dataItem in _samplesContainer.Samples.ToList()) objs.Add(dataItem);
                    Samples = objs;
                }
                else
                {
                    Samples = Enumerable.Empty<SampleObservation>();
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
                    var objs = new List<EventObservation>();
                    foreach (var dataItem in _eventsContainer.Events.ToList()) objs.Add(dataItem);
                    Events = objs;
                }
                else
                {
                    Events = Enumerable.Empty<EventObservation>();
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
                    var objs = new List<ConditionObservation>();
                    foreach (var dataItem in _conditionsContainer.Conditions.ToList()) objs.Add(dataItem);
                    Conditions = objs;
                }
                else
                {
                    Conditions = Enumerable.Empty<ConditionObservation>();
                }
            }
        }


        public XmlComponentStream() { }

        public XmlComponentStream(IComponentStream componentStream)
        {
            if (componentStream != null)
            {
                Component = componentStream.Component;
                ComponentType = componentStream.ComponentType;
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


        public IComponentStream ToComponentStream()
        {
            var componentStream = new ComponentStream();

            componentStream.Component = Component;
            componentStream.ComponentType = ComponentType;
            componentStream.ComponentId = ComponentId;
            componentStream.Name = Name;
            componentStream.NativeName = NativeName;
            componentStream.Uuid = Uuid;

            // Add Samples
            if (SamplesContainer != null && !SamplesContainer.Samples.IsNullOrEmpty())
            {
                componentStream.Samples = SamplesContainer.Samples;
            }

            // Add Events
            if (EventsContainer != null && !EventsContainer.Events.IsNullOrEmpty())
            {
                componentStream.Events = EventsContainer.Events;
            }

            // Add Conditions
            if (ConditionsContainer != null && !ConditionsContainer.Conditions.IsNullOrEmpty())
            {
                componentStream.Conditions = ConditionsContainer.Conditions;
            }

            return componentStream;
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (Component != null)
            {
                writer.WriteComment(Component.TypeDescription);
            }

            writer.WriteStartElement("ComponentStream");
            writer.WriteAttributeString("component", ComponentType);
            writer.WriteAttributeString("componentId", ComponentId);
            if (!string.IsNullOrEmpty(Name)) writer.WriteAttributeString("name", Name);
            if (!string.IsNullOrEmpty(NativeName)) writer.WriteAttributeString("nativeName", NativeName);
            if (!string.IsNullOrEmpty(Uuid)) writer.WriteAttributeString("uuid", Uuid);

            if (SamplesContainer != null)
            {
                writer.WriteStartElement("Samples");
                SamplesContainer.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (EventsContainer != null)
            {
                writer.WriteStartElement("Events");
                EventsContainer.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (ConditionsContainer != null)
            {
                writer.WriteStartElement("Condition");
                ConditionsContainer.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }


        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Child Nodes
                var inner = reader.ReadSubtree();

                // Read Attributes
                ComponentType = reader.GetAttribute("component");
                ComponentId = reader.GetAttribute("componentId");
                Name = reader.GetAttribute("name");
                NativeName = reader.GetAttribute("nativeName");
                Uuid = reader.GetAttribute("uuid");

                while (inner.Read())
                {
                    if (inner.NodeType == XmlNodeType.Element)
                    {
                        switch (inner.Name)
                        {
                            case "Samples":

                                // Read Samples Container
                                var samplesContainer = new XmlSamplesContainer();
                                samplesContainer.ReadXml(inner);
                                SamplesContainer = samplesContainer;
                                break;

                            case "Events":

                                // Read Events Container
                                var eventsContainer = new XmlEventsContainer();
                                eventsContainer.ReadXml(inner);
                                EventsContainer = eventsContainer;
                                break;

                            case "Condition":

                                // Read Conditions Container
                                var conditionsContainer = new XmlConditionsContainer();
                                conditionsContainer.ReadXml(inner);
                                ConditionsContainer = conditionsContainer;
                                break;
                        }
                    }
                }
            }
            catch { }
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion
    }
}
