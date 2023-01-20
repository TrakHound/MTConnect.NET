// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
