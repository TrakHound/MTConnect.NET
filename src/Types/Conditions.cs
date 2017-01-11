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
    public class ConditionType
    {
        public string Type { get; set; }
        public string[] SubTypes { get; set; }

        public static ConditionType Get(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                var result = new ConditionType();
                result.Type = node.Name;
                return result;
            }

            return null;
        }
    }

    public static class Conditions
    {
        public static ConditionType[] Get()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                var stream = assembly.GetManifestResourceStream("MTConnect.Types.ConditionTypes.xml");
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var xml = new XmlDocument();
                        xml.Load(reader);

                        if (xml.DocumentElement != null)
                        {
                            var result = new List<ConditionType>();

                            XmlNode root = xml.DocumentElement;

                            foreach (XmlNode node in root.ChildNodes)
                            {
                                var type = ConditionType.Get(node);
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
