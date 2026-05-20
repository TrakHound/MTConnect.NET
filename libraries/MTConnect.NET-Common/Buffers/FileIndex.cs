// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Maps a stable string identifier (such as a Device UUID or DataItem Id) to the integer slot number used to name its file within a durable buffer's on-disk index.
    /// </summary>
    public struct FileIndex
    {
        /// <summary>
        /// The index file name holding the Device UUID to slot mapping.
        /// </summary>
        public const string DevicesFileName = "devices";

        /// <summary>
        /// The index file name holding the DataItem Id to slot mapping.
        /// </summary>
        public const string DataItemsFileName = "dataItems";


        /// <summary>
        /// The integer slot number assigned to the identifier; slots are one-based.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The stable string identifier mapped to <see cref="Index"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Indicates whether the entry is usable, requiring a positive slot number and a non-null identifier.
        /// </summary>
        public bool IsValid => Index > 0 && Id != null;



        /// <summary>
        /// Initializes a mapping entry pairing the given slot number with the given identifier.
        /// </summary>
        /// <param name="index">The integer slot number.</param>
        /// <param name="id">The stable string identifier.</param>
        public FileIndex(int index, string id)
        {
            Index = index;
            Id = id;
        }


        /// <summary>
        /// Returns the entry serialized as a single <c>index,id</c> line, the on-disk representation used in index files.
        /// </summary>
        public override string ToString()
        {
            return $"{Index},{Id}";
        }

        /// <summary>
        /// Parses an <c>index,id</c> line back into a <see cref="FileIndex"/>; returns a default (invalid) entry when the input is empty or malformed.
        /// </summary>
        /// <param name="s">The serialized line to parse.</param>
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


        /// <summary>
        /// Builds a sequence of entries from an identifier-to-slot dictionary.
        /// </summary>
        /// <param name="indexes">The map of identifiers to slot numbers.</param>
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

        /// <summary>
        /// Collapses a sequence of entries back into an identifier-to-slot dictionary.
        /// </summary>
        /// <param name="fileIndexes">The entries to convert.</param>
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

        /// <summary>
        /// Reads and parses the index file at <c>{basePath}/index/{filePath}</c>, skipping malformed lines. Returns an empty sequence when the file is absent or unreadable. A relative or empty base path resolves under the application base directory's <c>buffer</c> folder.
        /// </summary>
        /// <param name="basePath">The buffer root directory; when empty defaults to the <c>buffer</c> folder under the application base directory.</param>
        /// <param name="filePath">The index file name to read.</param>
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

        /// <summary>
        /// Writes the given entries to <c>{basePath}/index/{filePath}</c>, creating the index directory if needed and overwriting any existing file. Returns true on success, false on failure or empty input.
        /// </summary>
        /// <param name="basePath">The buffer root directory; when empty defaults to the <c>buffer</c> folder under the application base directory.</param>
        /// <param name="filePath">The index file name to write.</param>
        /// <param name="fileIndexes">The entries to persist.</param>
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

        /// <summary>
        /// Deletes the entire index directory under the given buffer root. WARNING: this clears all identifier-to-slot mappings and can leave the buffered data files unrecoverable.
        /// </summary>
        /// <param name="basePath">The buffer root directory; when empty defaults to the <c>buffer</c> folder under the application base directory.</param>
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