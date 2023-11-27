// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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