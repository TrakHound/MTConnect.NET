// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;

namespace MTConnect.Buffers
{
    public struct FileIndex
    {
        public const string DevicesFileName = "devices";
        public const string DataItemsFileName = "dataItems";


        public int Index { get; set; }

        public string Id { get; set; }

        public bool IsValid => Index > 0 && Id != null;



        public FileIndex(int index, string id)
        {
            Index = index;
            Id = id;
        }


        public override string ToString()
        {
            return $"{Index},{Id}";
        }

        public static FileIndex FromString(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.Split(',');
                if (parts != null && parts.Length > 1)
                {
                    return new FileIndex(parts[0].ToInt(), parts[1]);
                }
            }

            return new FileIndex();
        }


        public static IEnumerable<FileIndex> Create(IDictionary<string, int> indexes)
        {
            var fileIndexes = new List<FileIndex>();

            if (!indexes.IsNullOrEmpty())
            {
                foreach (var index in indexes)
                {
                    fileIndexes.Add(new FileIndex(index.Value, index.Key));
                }
            }

            return fileIndexes;
        }

        public static IDictionary<string, int> ToDictionary(IEnumerable<FileIndex> fileIndexes)
        {
            var indexes = new Dictionary<string, int>();

            if (!fileIndexes.IsNullOrEmpty())
            {
                foreach (var fileIndex in fileIndexes)
                {
                    indexes.Add(fileIndex.Id, fileIndex.Index);
                }
            }

            return indexes;
        }


        //public static IEnumerable<FileIndex> FromFile(string filePath)
        //{
        //    return FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "buffer"), filePath);
        //}

        public static IEnumerable<FileIndex> FromFile(string basePath, string filePath)
        {
            var fileIndexes = new List<FileIndex>();

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    var baseDir = basePath;

                    if (!string.IsNullOrEmpty(baseDir))
                    {
                        if (!Path.IsPathRooted(baseDir)) baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDir);
                    }
                    else
                    {
                        baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "buffer");
                    }

                    var path = Path.Combine(baseDir, "index", filePath);
                    if (File.Exists(path))
                    {
                        var lines = File.ReadAllLines(path);
                        if (!lines.IsNullOrEmpty())
                        {
                            foreach (var line in lines)
                            {
                                var fileIndex = FromString(line);
                                if (fileIndex.IsValid) fileIndexes.Add(fileIndex);
                            }
                        }
                    }
                }
                catch { }
            }

            return fileIndexes;
        }

        //public static bool ToFile(string filePath, IEnumerable<FileIndex> fileIndexes)
        //{
        //    return ToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "buffer"), filePath, fileIndexes);
        //}

        public static bool ToFile(string basePath, string filePath, IEnumerable<FileIndex> fileIndexes)
        {
            if (!string.IsNullOrEmpty(filePath) && !fileIndexes.IsNullOrEmpty())
            {
                var s = "";

                foreach (var fileIndex in fileIndexes)
                {
                    s += fileIndex.ToString() + "\r\n";
                }

                try
                {
                    var baseDir = basePath;

                    if (!string.IsNullOrEmpty(baseDir))
                    {
                        if (!Path.IsPathRooted(baseDir)) baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDir);
                    }
                    else
                    {
                        baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "buffer");
                    }

                    var dir = Path.Combine(baseDir, "index");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    var path = Path.Combine(dir, filePath);

                    File.WriteAllText(path, s);
                    return true;
                }
                catch { }
            }

            return false;
        }

        public static void Reset(string basePath)
        {
            var baseDir = basePath;

            if (!string.IsNullOrEmpty(baseDir))
            {
                if (!Path.IsPathRooted(baseDir)) baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDir);
            }
            else
            {
                baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "buffer");
            }

            var dir = Path.Combine(baseDir, "index");
            if (Directory.Exists(dir))
            {
                try
                {
                    // Remove the entire Directory
                    // WARNING: This clears the entire index and may cause the buffered files to become corrupt
                    Directory.Delete(dir, true);
                }
                catch { }
            }
        }
    }
}