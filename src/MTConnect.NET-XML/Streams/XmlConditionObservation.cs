// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using System.Text.RegularExpressions;
using System.Xml;

namespace MTConnect.Streams
{
    public static class XmlConditionObservation
    {
        private const string NodePrefixPattern = "(.*):(.*)";
        private static readonly XmlDocument _document = new XmlDocument();


        public static void WriteXml(XmlWriter writer, IObservation observation)
        {
            if (observation != null)
            {
                var condition = (ConditionObservation)observation;

                try
                {
                    // Get Element name
                    var name = GetElementName(condition);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                        if (node != null)
                        {
                            // Add Common Attributes to Node
                            XmlObservation.AddAttributes(condition, node);
                            node.Attributes.RemoveNamedItem("message");

                            // Set InnerText to the CDATA
                            node.InnerText = condition.Message?.Trim();

                            // Add Comment
                            if (condition.DataItem != null)
                            {
                                // Write DataItem Type Description as Comment
                                writer.WriteComment($"Type = {condition.DataItem.Type} : {condition.DataItem.TypeDescription}");
                                writer.WriteWhitespace("\r\n");

                                // Write DataItem SubType Description as Comment
                                if (!string.IsNullOrEmpty(condition.DataItem.SubType))
                                {
                                    writer.WriteComment($"SubType = {condition.DataItem.SubType} : {condition.DataItem.SubTypeDescription}");
                                    writer.WriteWhitespace("\r\n");
                                }
                            }

                            // Write Value Description as Comment
                            var valueDescriptionText = Observation.GetDescriptionText(observation.Category, condition.Type, condition.SubType, condition.Message);
                            if (!string.IsNullOrEmpty(valueDescriptionText))
                            {
                                writer.WriteComment($"Message = {condition.Message} : {valueDescriptionText}");
                                writer.WriteWhitespace("\r\n");
                            }

                            // Write Node to XmlWriter
                            writer.WriteNode(new XmlNodeReader(node), false);
                        }
                    }
                }
                catch { }
            }
        }

        private static string GetElementName(IConditionObservation observation)
        {
            if (observation != null && !string.IsNullOrEmpty(observation.Type))
            {
                string name;

                var match = Regex.Match(observation.Type, NodePrefixPattern);
                if (match.Success)
                {
                    name = Devices.DataItem.GetPascalCaseType(match.Groups[2].Value);
                }
                else
                {
                    name = Devices.DataItem.GetPascalCaseType(observation.Type);
                }

                return name;
            }

            return null;
        }
    }
}
