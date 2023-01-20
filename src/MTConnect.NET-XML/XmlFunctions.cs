// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace MTConnect
{
    public static class XmlFunctions
    {
        public const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
        public const string NewLine = "\n";
        public const char NewLineCharacter = '\n';
        public const string Tab = "  ";


        private static readonly XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings()
        {
            Encoding = new UTF8Encoding(false),
            Indent = false,
            NewLineChars = NewLine,
            IndentChars = Tab,
            NamespaceHandling = NamespaceHandling.Default
        };
        private static readonly XmlWriterSettings _xmlWriterSettingsIndent = new XmlWriterSettings()
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
            NewLineChars = NewLine,
            IndentChars = Tab,
            NamespaceHandling = NamespaceHandling.Default
        };


        public static XmlWriterSettings XmlWriterSettings => _xmlWriterSettings;
        public static XmlWriterSettings XmlWriterSettingsIndent => _xmlWriterSettingsIndent;


        public static byte[] SanitizeBytes(byte[] inputBytes)
        {
            if (inputBytes != null)
            {
                var bytes = new byte[inputBytes.Length];
                inputBytes.CopyTo(bytes, 0);

                // Look for Whitespace bytes
                // For some reason the XmlReaderSettings.IgnoreWhitespace doesn't cover prefix whitespace
                var i = 0;
                var j = 0;
                while (i < bytes.Length)
                {
                    if (bytes[i] != 13 && bytes[i] != 10) break;
                    i++;
                    j++;
                }

                if (j > 0)
                {
                    // Shift Array over past the initial Whitespace bytes
                    var s = bytes.Length - j;
                    Array.Copy(bytes, j, bytes, 0, bytes.Length - j);
                    Array.Resize(ref bytes, s);
                }

                // Detect Encoding Byte Mark (if found then remove)
                var preamble = Encoding.UTF8.GetPreamble();
                if (bytes.Length >= preamble.Length && preamble.SequenceEqual(bytes.Take(preamble.Length)))
                {
                    var s = bytes.Length - preamble.Length;
                    Array.Copy(bytes, preamble.Length, bytes, 0, s);
                    Array.Resize(ref bytes, s);
                }

                // Look for trailing Whitespace bytes
                i = bytes.Length - 1;
                j = 0;
                while (i > 0)
                {
                    if (bytes[i] > 20) break;
                    i--;
                    j++;
                }

                if (j > 0)
                {
                    // Shift Array over past the trailing Whitespace bytes
                    Array.Resize(ref bytes, bytes.Length - j);
                }

                return bytes;
            }

            return null;
        }

        public static string FormatXml(string xml, bool indent = true, bool outputComments = false, bool omitXmlDeclaration = false)
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
                    writerSettings.Encoding = Encoding.UTF8;
                    writerSettings.OmitXmlDeclaration = omitXmlDeclaration;

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

        public static void WriteHeaderComment(XmlWriter writer, bool indentOutput)
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

            writer.WriteComment("This document was produced using the following MTConnect Agent Application :");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment(" -  -  -  -  -  ");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment($"{title} : Version {version}");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment($"{copyright}");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment($"For more information about this MTConnect Agent Application visit ({projectUrl})");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment(" -  -  -  -  -  ");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment("This Document was produced using the MTConnect.NET library which is licensed under the Apache Version 2.0 License (https://www.apache.org/licenses/LICENSE-2.0)");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment("Source code for this Library is available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment(" -  -  -  -  -  ");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment("For more information about TrakHound visit (http://trakhound.com)");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment("For more information about MTConnect visit (http://mtconnect.org)");
            if (indentOutput) writer.WriteWhitespace(NewLine);
            writer.WriteComment(" -  -  -  -  -  ");
            if (indentOutput) writer.WriteWhitespace(NewLine);
        }
    }
}