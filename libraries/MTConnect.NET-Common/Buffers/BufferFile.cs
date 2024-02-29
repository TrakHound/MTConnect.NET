// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    struct BufferFile
    {
        public ulong SequenceTop { get; set; }

        public ulong SequenceBottom { get; set; }

        public string Path { get; set; }


        public BufferFile(string path, uint pageSize)
        {
            SequenceTop = System.IO.Path.GetFileName(path).ToULong();
            SequenceBottom = SequenceTop - pageSize;
            Path = path;
        }
    }
}