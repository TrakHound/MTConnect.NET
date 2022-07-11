// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// Conditions is a XML container type element. 
    /// Conditions organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class XmlConditionsContainer : IXmlSerializable
    {
        private readonly XmlDocument _document;

        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of EVENT.
        /// </summary>
        [XmlIgnore]
        public List<ConditionObservation> Conditions { get; set; }


        public XmlConditionsContainer()
        {
            // Initialize the Conditions List
            Conditions = new List<ConditionObservation>();

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();
        }

        public XmlConditionsContainer(IObservation observation)
        {
            // Initialize the Conditions List
            Conditions = new List<ConditionObservation>();
            if (observation != null)
            {
                var conditionObservation = observation as ConditionObservation;
                if (conditionObservation != null) Conditions.Add(conditionObservation);
            }

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Conditions.IsNullOrEmpty())
            {
                foreach (var dataItem in Conditions)
                {
                    switch (dataItem.Representation)
                    {
                        case DataItemRepresentation.VALUE: WriteValueXml(writer, dataItem); break;
                    }
                }
            }
        }

        private static string GetElementName(IConditionObservation observation)
        {
            if (observation != null)
            {
                return observation.Level.ToString().ToPascalCase();
            }

            return null;
        }

        private void WriteValueXml(XmlWriter writer, ConditionObservation observation)
        {
            try
            {
                // Get Element name
                var name = GetElementName(observation);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        // Add Common Attributes to Node
                        XmlObservation.AddAttributes(observation, node);

                        // Set InnerText to the CDATA
                        node.InnerText = observation.Message?.Trim();

                        // Add Comment
                        if (observation.DataItem != null)
                        {
                            // Write DataItem Type Description as Comment
                            writer.WriteComment($"Type = {observation.DataItem.Type} : {observation.DataItem.TypeDescription}");
                            writer.WriteWhitespace("\r\n");

                            // Write DataItem SubType Description as Comment
                            if (!string.IsNullOrEmpty(observation.DataItem.SubType))
                            {
                                writer.WriteComment($"SubType = {observation.DataItem.SubType} : {observation.DataItem.SubTypeDescription}");
                                writer.WriteWhitespace("\r\n");
                            }
                        }

                        // Write Level Description as Comment
                        writer.WriteComment($"Level = {observation.Level} : {ConditionLevelDescriptions.Get(observation.Level)}");
                        writer.WriteWhitespace("\r\n");

                        if (observation.Qualifier != ConditionQualifier.NOT_SPECIFIED)
                        {
                            // Write Qualifier Description as Comment
                            writer.WriteComment($"Qualifier = {observation.Qualifier} : {ConditionQualifierDescriptions.Get(observation.Qualifier)}");
                            writer.WriteWhitespace("\r\n");
                        }

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }


        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Child Elements
                using (var inner = reader.ReadSubtree())
                {
                    while (inner.Read())
                    {
                        if (inner.NodeType == XmlNodeType.Element)
                        {
                            var node = _document.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (!string.IsNullOrEmpty(child.Name) && child.NodeType == XmlNodeType.Element)
                                {
                                    var elementName = child.Name;

                                    var level = elementName.ToUpper().ConvertEnum<ConditionLevel>();
                                    var type = XmlObservation.ReadAttributeValue(child, "type");
                                    var representation = XmlObservation.GetDataItemRepresentation(elementName);

                                    // Create a new Observation based on Type and Representation
                                    var observation = ConditionObservation.Create(type, representation);
                                    if (observation != null)
                                    {
                                        observation.Level = level;
                                        observation.SetProperty(nameof(Observation.Representation), representation);

                                        // Read the XML Attributes and assign the corresponding Properties
                                        XmlObservation.ReadAttributes(observation, child);

                                        observation.NativeCode = observation.GetProperty<string>(nameof(ConditionObservation.NativeCode));
                                        observation.NativeSeverity = observation.GetProperty<string>(nameof(ConditionObservation.NativeSeverity));
                                        observation.Qualifier = observation.GetProperty<string>(nameof(ConditionObservation.Qualifier)).ConvertEnum<ConditionQualifier>();

                                        switch (observation.Representation)
                                        {
                                            case DataItemRepresentation.VALUE: SetValue(observation, child); break;
                                        }

                                        Conditions.Add(observation);
                                    }
                                }
                            }
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


        private static void SetValue(Observation observation, XmlNode node)
        {
            if (observation != null && node != null)
            {
                observation.AddValue(new ObservationValue(ValueKeys.Result, node.InnerText));
            }
        }

        #endregion
    }
}
