using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
        private static string DEFINITION_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EventTypes.xml");

        public static EventType[] Get()
        {
            if (File.Exists(DEFINITION_FILE_PATH))
            {
                try
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(DEFINITION_FILE_PATH);

                    if (xml.DocumentElement != null)
                    {
                        var result = new List<EventType>();

                        XmlNode root = xml.DocumentElement;

                        foreach (XmlNode node in root.ChildNodes)
                        {
                            var eventType = EventType.Get(node);
                            if (eventType != null) result.Add(eventType);
                        }

                        return result.ToArray();
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }

            return null;
        }

    }
}
