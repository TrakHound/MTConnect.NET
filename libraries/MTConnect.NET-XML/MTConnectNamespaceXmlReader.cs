// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Xml;

namespace MTConnect
{
    /// <summary>
    /// Reads XML as if the MTConnect namespaces (<c>urn:mtconnect.org:*</c>) were absent, so the
    /// namespace-less serialization surrogates match documents of any MTConnect version. Other
    /// namespaces (such as an embedded QIF document's own) are reported unchanged, and the input
    /// text is never rewritten, so prefixed content stays well formed.
    /// </summary>
    internal sealed class MTConnectNamespaceXmlReader : XmlTextReader
    {
        private const string MTConnectNamespacePrefix = "urn:mtconnect.org:";


        private MTConnectNamespaceXmlReader(Stream input) : base(input)
        {
            DtdProcessing = DtdProcessing.Prohibit;
            XmlResolver = null;
        }

        /// <summary>
        /// The namespace of the current node, or an empty string when it is an MTConnect namespace.
        /// </summary>
        public override string NamespaceURI
        {
            get
            {
                var ns = base.NamespaceURI;
                return ns.StartsWith(MTConnectNamespacePrefix, StringComparison.Ordinal) ? string.Empty : ns;
            }
        }


        /// <summary>
        /// Creates a reader over <paramref name="bytes"/>. The encoding comes from the byte-order mark
        /// or XML declaration; a UTF-8 byte-order mark and whitespace ahead of the first markup are skipped
        /// (whitespace ahead of an XML declaration is otherwise an error).
        /// </summary>
        public static XmlReader Create(byte[] bytes)
        {
            var offset = 0;
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) offset = 3;
            while (offset < bytes.Length && IsWhitespace(bytes[offset])) offset++;

            return new MTConnectNamespaceXmlReader(new MemoryStream(bytes, offset, bytes.Length - offset, false));
        }

        private static bool IsWhitespace(byte b)
        {
            return b == 0x20 || b == 0x09 || b == 0x0A || b == 0x0D;
        }
    }
}
