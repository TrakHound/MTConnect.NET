// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text;

namespace MTConnect.Writers
{
    internal class Utf8Writer : StringWriter
    {
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false);

        // Use UTF8 encoding
        public override Encoding Encoding => encoding;
    }
}