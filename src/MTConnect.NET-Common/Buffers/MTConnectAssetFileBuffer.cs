// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents.Configuration;
using MTConnect.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Buffers
{
    public class MTConnectAssetFileBuffer : MTConnectAssetBuffer, IDisposable
    {
        public const int DefaultPageSize = 100;
        public const string DirectoryBuffer = "buffer";
        public const string DirectoryAssets = "assets";

        private readonly MTConnectAssetQueue _items;
        private readonly Regex _regex = new Regex("([0-9]*)_(.*)");

        private CancellationTokenSource stop;
        private bool _isStarted;
        private bool _isLoading;
        private long _pageIndex = 0;

        public int WriteInterval { get; set; } = 1000;

        public int RetentionInterval { get; set; } = 10000;

        public int MaxItemsPerWrite { get; set; } = 1000;

        public long QueuedItemCount => _items.Count;

        public bool UseCompression { get; set; } = true;

        public EventHandler BufferLoadStarted { get; set; }

        public EventHandler<AssetBufferLoadArgs> BufferLoadCompleted { get; set; }


        public MTConnectAssetFileBuffer()
        {
            _items = new MTConnectAssetQueue();

            Start();
        }

        public MTConnectAssetFileBuffer(MTConnectAgentConfiguration configuration) : base(configuration)
        {
            _items = new MTConnectAssetQueue();

            Start();
        }

        protected override void OnAssetAdd(int bufferIndex, IAsset asset, int originalIndex)
        {
            if (!_isLoading)
            {
                Add(bufferIndex, asset, originalIndex);
            }
        }


        /// <summary>
        /// Start the Buffer Read/Write thread
        /// </summary>
        private void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                stop = new CancellationTokenSource();

                _ = Task.Run(() => WriteWorker(stop.Token));
            }
        }

        /// <summary>
        /// Stop the Buffer
        /// </summary>
        private void Stop()
        {
            if (_isStarted)
            {
                if (stop != null) stop.Cancel();

                WriteAllItems();

                _isStarted = false;
            }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }


        public bool Add(int index, IAsset asset, int originalIndex)
        {
            // Add to internal Queue
            return _items.Add(index, asset, originalIndex);
        }


        #region "Load"

        public bool Load()
        {
            var found = false;
            _isLoading = true;

            var stpw = System.Diagnostics.Stopwatch.StartNew();
            if (BufferLoadStarted != null) BufferLoadStarted.Invoke(this, new EventArgs());

            var items = Read();
            if (!items.IsNullOrEmpty())
            {
                var oItems = items.OrderBy(o => o.Index);

                foreach (var item in oItems)
                {
                    if (AddAsset(item.Asset))
                    {
                        _pageIndex++;
                    }
                }

                found = true;
            }

            stpw.Stop();
            if (found && BufferLoadCompleted != null) BufferLoadCompleted.Invoke(this, new AssetBufferLoadArgs(items.Count(), stpw.ElapsedMilliseconds));

            _isLoading = false;
            return found;
        }

        #endregion

        #region "Read"

        private IEnumerable<AssetReadItem> Read()
        {
            var items = new List<AssetReadItem>();

            try
            {
                // Get Assets Directory Path
                var dir = GetDirectory();
                var files = Directory.GetFiles(dir);
                if (!files.IsNullOrEmpty())
                {
                    foreach (var file in files)
                    {
                        var match = _regex.Match(Path.GetFileName(file));
                        if (match.Success && match.Groups.Count > 2)
                        {
                            var index = match.Groups[1].ToInt();
                            var assetType = match.Groups[2].ToString();

                            // Read the Asset from the File
                            var asset = ReadAsset(assetType, file);
                            if (asset != null)
                            {
                                items.Add(new AssetReadItem(index, asset));
                            }
                        }
                    }
                }
            }
            catch { }

            return items;
        }

        private IAsset ReadAsset(string assetType, string path)
        {
            var json = ReadFile(path);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var type = Asset.GetAssetType(assetType);
                    var asset = JsonSerializer.Deserialize(json, type);
                    if (asset != null) return (IAsset)asset;
                }
                catch { }
            }

            return null;
        }

        private string ReadFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (UseCompression)
                    {
                        using (var fileStream = File.Open(path, FileMode.Open))
                        {
                            using (var readStream = new MemoryStream())
                            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                            {
                                gzipStream.CopyTo(readStream);

                                var bytes = readStream.ToArray();

                                return Encoding.ASCII.GetString(bytes);
                            }
                        }
                    }
                    else
                    {
                        return File.ReadAllText(path);
                    }
                }
                catch { }
            }

            return null;
        }

        #endregion

        #region "Write"

        private async Task WriteWorker(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(WriteInterval, cancellationToken);

                    await WriteItems(MaxItemsPerWrite);
                }
                catch (TaskCanceledException) { }
                catch { }
            }
        }

        public void WriteAllItems()
        {
            _ = Task.Run(async () =>
             {
                 await WriteItems(int.MaxValue);
             });
        }

        private async Task WriteItems(int maxItems)
        {
            var queueItems = _items.Take(maxItems);
            if (!queueItems.IsNullOrEmpty())
            {
                foreach (var item in queueItems)
                {
                    await WriteToFile(item.Index, item.Asset, item.OriginalIndex);
                }
            }
        }

        private void UpdateFileIndexes(int index)
        {
            var dir = GetDirectory();
            var files = Directory.GetFiles(dir);
            if (!files.IsNullOrEmpty())
            {
                foreach (var file in files)
                {
                    var fileDirectory = Path.GetDirectoryName(file);
                    var filename = Path.GetFileName(file);

                    var match = _regex.Match(filename);
                    if (match.Success && match.Groups.Count > 2)
                    {
                        var fileIndex = match.Groups[1].ToInt();
                        var fileAssetType = match.Groups[2].ToString();

                        if (fileIndex > index)
                        {
                            var newFilename = $"{fileIndex - 1}_{fileAssetType}";
                            var newPath = Path.Combine(fileDirectory, newFilename);

                            // Rename File to use the new Index
                            if (File.Exists(newPath)) File.Delete(newPath);
                            File.Move(file, newPath);
                        }
                        else if (fileIndex == index)
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
        }

        private async Task<bool> WriteToFile(int index, IAsset asset, int originalIndex)
        {
            if (asset != null && !string.IsNullOrEmpty(asset.AssetId) && !string.IsNullOrEmpty(asset.Type))
            {
                var dir = GetDirectory();
                var filename = $"{index}_{asset.Type}";
                var path = Path.Combine(dir, filename);

                try
                {
                    // Shift File Indexes (if needed)
                    if (originalIndex >= 0)
                    {
                        UpdateFileIndexes(originalIndex);
                    }
                    else if (_pageIndex >= BufferSize && index >= BufferSize - 1)
                    {
                        UpdateFileIndexes(0);
                    }

                    // Remove Existing File with the same Index
                    // (run for multiple files just to be safe)
                    var existingFiles = Directory.GetFiles(dir);
                    if (!existingFiles.IsNullOrEmpty())
                    {
                        foreach (var file in existingFiles)
                        {
                            var match = _regex.Match(Path.GetFileName(file));
                            if (match.Success && match.Groups.Count > 2)
                            {
                                var fileIndex = match.Groups[1].ToInt();
                                if (fileIndex == index)
                                {
                                    File.Delete(file);
                                }
                            }
                        }
                    }

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                    };

                    var assetType = Asset.GetAssetType(asset.Type);

                    var json = JsonSerializer.Serialize(asset, assetType, options);
                    if (!string.IsNullOrEmpty(json))
                    {
                        if (UseCompression)
                        {
                            // Get Bytes
                            var bytes = Encoding.ASCII.GetBytes(json);

                            using (var stream = new MemoryStream())
                            {
                                using (var gzipStream = new GZipStream(stream, CompressionMode.Compress))
                                {
                                    await gzipStream.WriteAsync(bytes, 0, bytes.Length);
                                    await gzipStream.FlushAsync();
                                }

                                var fileBytes = stream.ToArray();
                                File.WriteAllBytes(path, fileBytes);
                            }
                        }
                        else
                        {
                            File.WriteAllText(path, json);
                        }

                        _pageIndex++;
                        if (_pageIndex > BufferSize) _pageIndex = BufferSize;
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        #endregion


        private string GetDirectory(string assetType = null, bool createIfNotExists = true)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, DirectoryAssets);
            if (!string.IsNullOrEmpty(assetType)) dir = Path.Combine(dir, assetType);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
