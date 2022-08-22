// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Buffers
{
    struct BufferFile
    {
        public long SequenceTop { get; set; }

        public long SequenceBottom { get; set; }

        public string Path { get; set; }

        public BufferFile(string path, int pageSize)
        {
            SequenceTop = System.IO.Path.GetFileName(path).ToLong();
            SequenceBottom = SequenceTop - pageSize;
            Path = path;
        }
    }
}
