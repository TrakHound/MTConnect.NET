using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MTConnect
{
    internal static partial class Tools
    {
        public static class Address
        {
            public static string GetComponents(XmlNode node)
            {
                string result = "";

                do
                {
                    string name = node.Name;

                    string address = name;

                    string id = XML.GetAttribute(node, "id");
                    if (!String.IsNullOrEmpty(id))
                    {
                        address = name + "[@id='" + id + "']";
                    }

                    result = address + "/" + result;
                    node = node.ParentNode;

                    if (node == null) break;

                } while (node.Name != "Device");

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

            public static string GetStreams(XmlNode node)
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
}
