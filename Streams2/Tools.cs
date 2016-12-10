// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

using System.Xml;

namespace TH_MTConnect.Streams
{
    public static class Tools
    {

        //public static DeviceStream GetDeviceStreamFromXML(XmlNode DeviceNode)
        //{
        //    DeviceStream result = null;

        //    if (DeviceNode != null)
        //    {
        //        DeviceStream deviceStream = new DeviceStream(DeviceNode);

        //        foreach (XmlNode ChildNode in DeviceNode.ChildNodes)
        //        {
        //            if (ChildNode.NodeType == XmlNodeType.Element)
        //            {
        //                switch (ChildNode.Name.ToLower())
        //                {
        //                    case "componentstream":

        //                        deviceStream.ComponentStreams.Add(ProcessComponentStream(ChildNode));

        //                        break;

        //                    case "events":

        //                        result.DataItems.Events.AddRange(ProcessEvents(ChildNode));

        //                        break;

        //                    case "samples":

        //                        result.DataItems.Samples.AddRange(ProcessSamples(ChildNode));

        //                        break;
        //                }
        //            }
        //        }

        //        result = deviceStream;
        //    }

        //    return result;
        //}

        //public static ComponentStream ProcessComponentStream(XmlNode ComponentStreamNode)
        //{
        //    ComponentStream result = new ComponentStream(ComponentStreamNode);

        //    foreach (XmlNode DataItemNode in ComponentStreamNode.ChildNodes)
        //    {
        //        if (DataItemNode.NodeType == XmlNodeType.Element)
        //        {
        //            switch (DataItemNode.Name.ToLower())
        //            {
        //                case "condition":

        //                    result.DataItems.Conditions.AddRange(ProcessConditions(DataItemNode));

        //                    break;

        //                case "events":

        //                    result.DataItems.Events.AddRange(ProcessEvents(DataItemNode));

        //                    break;

        //                case "samples":

        //                    result.DataItems.Samples.AddRange(ProcessSamples(DataItemNode));

        //                    break;
        //            }
        //        }
        //    }

        //    return result;
        //}

        //public static List<Condition> ProcessConditions(XmlNode ConditionNode)
        //{
        //    List<Condition> result = new List<Condition>();

        //    foreach (XmlNode ChildNode in ConditionNode.ChildNodes)
        //    {
        //        Condition condition = new Condition(ChildNode);
        //        result.Add(condition);
        //    }

        //    return result;
        //}

        //public static List<Event> ProcessEvents(XmlNode EventsNode)
        //{
        //    List<Event> result = new List<Event>();

        //    foreach (XmlNode EventNode in EventsNode.ChildNodes)
        //    {
        //        Event event_DI = new Event(EventNode);
        //        result.Add(event_DI);
        //    }

        //    return result;
        //}

        //public static List<Sample> ProcessSamples(XmlNode SamplesNode)
        //{
        //    List<Sample> result = new List<Sample>();

        //    foreach (XmlNode SampleNode in SamplesNode.ChildNodes)
        //    {
        //        Sample sample_DI = new Sample(SampleNode);
        //        result.Add(sample_DI);
        //    }

        //    return result;
        //}

        //public static DataItemCollection GetDataItemsFromDeviceStream(DeviceStream deviceStream)
        //{
        //    DataItemCollection result = new DataItemCollection();

        //    foreach (ComponentStream componentStream in deviceStream.ComponentStreams)
        //    {
        //        foreach (Condition item in componentStream.DataItems.Conditions) result.Conditions.Add(item);
        //        foreach (Event item in componentStream.DataItems.Events) result.Events.Add(item);
        //        foreach (Sample item in componentStream.DataItems.Samples) result.Samples.Add(item);
        //    }

        //    foreach (Condition item in deviceStream.DataItems.Conditions) result.Conditions.Add(item);
        //    foreach (Event item in deviceStream.DataItems.Events) result.Events.Add(item);
        //    foreach (Sample item in deviceStream.DataItems.Samples) result.Samples.Add(item);

        //    return result;
        //}


        public static string GetFullAddress(XmlNode node)
        {
            string result = "";

            do
            {
                string name = node.Name;

                string address = name;

                string id = XML.GetAttribute(node, "componentId");
                if (String.IsNullOrEmpty(id)) id = XML.GetAttribute(node, "dataItemId");
                if (!String.IsNullOrEmpty(id))
                {
                    address = name + "[@id='" + id + "']";
                }

                result = address + "/" + result;
                node = node.ParentNode;

                if (node == null) break;

            } while (node.Name != "DeviceStream");

            if (result.Length > 0)
            {
                if (result[0] != Convert.ToChar("/")) result = "/" + result;
                if (result.Length > 1)
                {
                    if (result[result.Length - 1] == Convert.ToChar("/")) result = result.Remove(result.Length - 1);
                }
            }

            return result;
        }
    }
}
