// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Writers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace MTConnect
{
    internal static class XmlFunctions
    {
        public static string FormatXml(string xml, bool indent = true, bool outputComments = false)
        {
            try
            {
                // Set XML Reader Settings
                var readerSettings = new XmlReaderSettings();
                readerSettings.ValidationType = ValidationType.None;
                readerSettings.IgnoreComments = !outputComments;

                // Set XML Reader Settings
                using (var stringReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(stringReader, readerSettings))
                {
                    var document = new XmlDocument();
                    document.Load(xmlReader);

                    // Set XML Writer Settings
                    var writerSettings = new XmlWriterSettings();
                    writerSettings.NewLineChars = "\r\n";
                    writerSettings.Indent = indent;
                    writerSettings.Encoding = System.Text.Encoding.UTF8;

                    using (var stringWriter = new Utf8Writer())
                    using (var xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
                    {
                        document.Save(xmlWriter);
                        return stringWriter.ToString();
                    }
                }
            }
            catch { }

            return xml;
        }

        public static string CreateHeaderComment()
        {
            var assembly = Assembly.GetEntryAssembly();

            var title = assembly.GetName().Name;
            var version = assembly.GetName().Version;
            var copyright = "";
            var projectUrl = "";

            var customAttributes = assembly.GetCustomAttributes();
            if (!customAttributes.IsNullOrEmpty())
            {
                // Get Product Attribute
                var productAttribute = customAttributes.FirstOrDefault(o => o.GetType() == typeof(AssemblyProductAttribute));
                if (productAttribute != null) title = ((AssemblyProductAttribute)productAttribute).Product;

                // Get Copyright Attribute
                var copyrightAttribute = customAttributes.FirstOrDefault(o => o.GetType() == typeof(AssemblyCopyrightAttribute));
                if (copyrightAttribute != null) copyright = ((AssemblyCopyrightAttribute)copyrightAttribute).Copyright;

                // Get Project Url Attribute
                var metadataAttributes = customAttributes.Where(o => o.GetType() == typeof(AssemblyMetadataAttribute));
                if (!metadataAttributes.IsNullOrEmpty())
                {
                    foreach (AssemblyMetadataAttribute metadataAttribute in metadataAttributes)
                    {
                        var projectAttribute = metadataAttribute;
                        if (projectAttribute.Key == "RepositoryUrl")
                        {
                            projectUrl = projectAttribute.Value;
                        }
                    }
                }
            }

            // Add Header Comments
            var headerComments = new List<string>();

            headerComments.Add("This document was produced using the following MTConnect Agent Application :");
            headerComments.Add(" -  -  -  -  -  ");
            headerComments.Add($"{title} : Version {version}");
            headerComments.Add($"{copyright}");
            headerComments.Add($"For more information about this MTConnect Agent Application visit ({projectUrl})");
            headerComments.Add(" -  -  -  -  -  ");
            headerComments.Add("This Document was produced using the MTConnect.NET library which is licensed under the Apache Version 2.0 License (https://www.apache.org/licenses/LICENSE-2.0)");
            headerComments.Add("Source code for this Library is available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            headerComments.Add(" -  -  -  -  -  ");
            headerComments.Add("For more information about TrakHound visit (http://trakhound.com)");
            headerComments.Add("For more information about MTConnect visit (http://mtconnect.org)");
            headerComments.Add(" -  -  -  -  -  ");

            var headerXml = "";
            foreach (var headerComment in headerComments)
            {
                headerXml += $"<!-- {headerComment} -->";
            }

            return headerXml;
        }
    }
}
