// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.IO;
using System.Xml;

namespace MTConnect
{
    internal static class XmlFunctions
    {
        public static string IndentXml(string xml)
        {
            try
            {
                using (var writer = new StringWriter())
                {
                    var document = new XmlDocument();
                    document.LoadXml(xml);
                    document.Save(writer);
                    return writer.ToString();
                }
            }
            catch { }

            return null;
        }
    }
}
