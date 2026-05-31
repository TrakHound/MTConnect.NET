// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
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
    /// <summary>
    /// A durable <see cref="MTConnectAssetBuffer"/> that persists assets to disk so the buffer survives restarts, writing changes on a background thread and reloading them on startup.
    /// </summary>
    public class MTConnectAssetFileBuffer : MTConnectAssetBuffer, IDisposable
    {
        /// <summary>
        /// The default number of assets read or written per page during load and flush operations.
        /// </summary>
        public const int DefaultPageSize = 100;

        /// <summary>
        /// The root directory name under the application base directory that holds all durable buffer data.
        /// </summary>
        public const string DirectoryBuffer = "buffer";

        /// <summary>
        /// The subdirectory name under the buffer root that holds persisted asset files.
        /// </summary>
        public const string DirectoryAssets = "assets";

        private readonly string _basePath;
        private readonly MTConnectAssetQueue _items;
        private readonly Regex _regex = new Regex("([0-9]*)_(.*)");

        private CancellationTokenSource stop;
        private bool _isStarted;
        private bool _isLoading;
        private ulong _pageIndex = 0;

        /// <summary>
        /// The interval, in milliseconds, between background flushes of queued asset changes to disk.
        /// </summary>
        public int WriteInterval { get; set; } = 1000;

        /// <summary>
        /// The interval, in milliseconds, between retention passes that prune persisted assets no longer in the buffer.
        /// </summary>
        public int RetentionInterval { get; set; } = 10000;

        /// <summary>
        /// The maximum number of queued asset changes written to disk in a single flush.
        /// </summary>
        public uint MaxItemsPerWrite { get; set; } = 1000;

        /// <summary>
        /// The number of asset changes currently queued and awaiting a flush to disk.
        /// </summary>
        public long QueuedItemCount => _items.Count;

        /// <summary>
        /// Indicates whether persisted asset files are GZip-compressed.
        /// </summary>
        public bool UseCompression { get; set; } = true;

        /// <summary>
        /// Raised when the buffer begins reloading persisted assets from disk on startup.
        /// </summary>
        public event EventHandler BufferLoadStarted;

        /// <summary>
        /// Raised when the startup reload completes, reporting the number of assets recovered and the elapsed time.
        /// </summary>
        public event EventHandler<AssetBufferLoadArgs> BufferLoadCompleted;


        /// <summary>
        /// Initializes the durable asset buffer with default settings and starts the background write worker.
        /// </summary>
        public MTConnectAssetFileBuffer()
        {
            _items = new MTConnectAssetQueue();

            Start();
        }

        /// <summary>
        /// Initializes the durable asset buffer from the supplied agent configuration, optionally rooting persistence at a custom base path, and starts the background write worker.
        /// </summary>
        /// <param name="configuration">The agent configuration controlling buffer capacity.</param>
        /// <param name="basePath">An optional custom buffer root directory; when null the default location is used.</param>
        public MTConnectAssetFileBuffer(IAgentConfiguration configuration, string basePath = null) : base(configuration)
        {
            _basePath = basePath;
            _items = new MTConnectAssetQueue();

            Start();
        }

        /// <summary>
        /// Queues the added asset for persistence, unless the addition originated from the startup reload itself.
        /// </summary>
        /// <param name="bufferIndex">The slot the asset was stored at.</param>
        /// <param name="asset">The asset that was added.</param>
        /// <param name="originalIndex">The slot the asset previously occupied when it replaced an existing asset.</param>
        protected override void OnAssetAdd(uint bufferIndex, IAsset asset, uint originalIndex)
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

        /// <summary>
        /// Stops the background write worker, flushing any queued asset changes to disk before releasing resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deletes the entire durable asset directory under the given buffer root. WARNING: this permanently clears all persisted assets.
        /// </summary>
        /// <param name="basePath">The buffer root directory; when null the default location is used.</param>
        public static void Reset(string basePath)
        {
            var dir = GetDirectory(basePath, false);
            if (Directory.Exists(dir))
            {
                try
                {
                    // Remove the entire Directory
                    // WARNING: This clears the entire buffer
                    Directory.Delete(dir, true);
                }
                catch { }
            }
        }


        /// <summary>
        /// Enqueues an asset change to be flushed to disk by the background write worker.
        /// </summary>
        /// <param name="index">The buffer slot the asset occupies.</param>
        /// <param name="asset">The asset to persist.</param>
        /// <param name="originalIndex">The slot the asset previously occupied when it replaced an existing asset.</param>
        /// <returns>True if the change was queued.</returns>
        public bool Add(uint index, IAsset asset, uint originalIndex)
        {
            // Add to internal Queue
            return _items.Add(index, asset, originalIndex);
        }


        #region "Load"

        /// <summary>
        /// Reloads all persisted assets from disk back into the in-memory buffer, raising <see cref="BufferLoadStarted"/> and <see cref="BufferLoadCompleted"/> around the operation.
        /// </summary>
        /// <returns>True if at least one asset was recovered.</returns>
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

        /// <summary>
        /// Flushes every queued asset change to disk immediately, bypassing the timed write interval.
        /// </summary>
        public void WriteAllItems()
        {
            _ = Task.Run(async () =>
             {
                 await WriteItems(int.MaxValue);
             });
        }

        private async Task WriteItems(uint maxItems)
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

        private void UpdateFileIndexes(uint index)
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

        private async Task<bool> WriteToFile(uint index, IAsset asset, uint originalIndex)
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
                        WriteIndented = true
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


        private string GetDirectory(bool createIfNotExists = true)
        {
            return GetDirectory(_basePath, createIfNotExists);
        }

        private static string GetDirectory(string basePath, bool createIfNotExists = true)
        {
            var baseDir = basePath;

            if (!string.IsNullOrEmpty(baseDir))
            {
                if (!Path.IsPathRooted(baseDir)) baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDir);
            }
            else
            {
                baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer);
            }

            string dir = Path.Combine(baseDir, DirectoryAssets);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }
    }
}