// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MTConnect.Types
{
    public class EventType
    {
        public string Type { get; set; }
        public string[] SubTypes { get; set; }
        public string[] Values { get; set; }

        public static EventType Get(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                var result = new EventType();

                result.Type = node.Name;

                var values = new List<string>();

                foreach (XmlNode valueNode in node.ChildNodes)
                {
                    values.Add(valueNode.Name);
                }

                result.Values = values.ToArray();

                return result;
            }

            return null;
        }
    }

    public static class Events
    {
        public static EventType[] Get()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                var stream = assembly.GetManifestResourceStream("MTConnect.Types.EventTypes.xml");
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var xml = new XmlDocument();
                        xml.Load(reader);

                        if (xml.DocumentElement != null)
                        {
                            var result = new List<EventType>();

                            XmlNode root = xml.DocumentElement;

                            foreach (XmlNode node in root.ChildNodes)
                            {
                                var type = EventType.Get(node);
                                if (type != null) result.Add(type);
                            }

                            return result.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return null;
        }

    }
}
