// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Observations.Input;
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
        private readonly object _lock = new object();

        private CancellationTokenSource stop;
        private bool _isStarted;
        //private bool _exists;


        public int Interval { get; set; } = 500;

        //public int PageSize { get; set; } = DefaultPageSize;

        //public int MaxRecordsPerFile { get; set; } = 5000;

        public int MaxItemsPerWrite { get; set; } = 1000;

        public long QueuedItemCount => _items.Count;


        public MTConnectAssetFileBuffer()
        {
            _items = new MTConnectAssetQueue();

            Start();
        }

        protected override void OnAssetAdd(IAsset asset)
        {
            Add(asset);
        }


        /// <summary>
        /// Start the Buffer Read/Write thread
        /// </summary>
        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                stop = new CancellationTokenSource();

                //_ = Task.Run(() => CheckWorker(stop.Token));
                _ = Task.Run(() => WriteWorker(stop.Token));
            }
        }

        /// <summary>
        /// Stop the Buffer
        /// </summary>
        public void Stop()
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


        #region "Add"

        public bool Add(IAsset asset)
        {
            // Add to internal Queue
            return _items.Add(asset);
        }

        public bool Add(IEnumerable<IAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                var success = true;

                foreach (var asset in assets)
                {
                    success = Add(asset);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Read"

        public IEnumerable<IAsset> Read()
        {
            var assets = new List<IAsset>();

            try
            {
                // Get Assets Directory Path
                var dir = GetDirectory();

                // Get a list of all asset buffer files in directory
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    var asset = ReadAsset(file);
                    if (asset != null) assets.Add(asset);
                }
            }
            catch { }

            return assets;
        }

        private IAsset ReadAsset(string path)
        {
            var xml = ReadFile(path);
            if (!string.IsNullOrEmpty(xml))
            {

            }

            return null;
        }

        private string ReadFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    return File.ReadAllText(path);

                    //using (var fileStream = File.Open(path, FileMode.Open))
                    //{
                    //    using (var readStream = new MemoryStream())
                    //    using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    //    {
                    //        gzipStream.CopyTo(readStream);

                    //        var bytes = readStream.ToArray();

                    //        return Encoding.ASCII.GetString(bytes);
                    //    }
                    //}
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
                    await Task.Delay(Interval, cancellationToken);

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
                    await WriteToFile(item);
                }
            }
        }

        private async Task<bool> WriteToFile(IAsset asset)
        {
            if (asset != null && !string.IsNullOrEmpty(asset.AssetId))
            {
                var dir = GetDirectory();
                var filename = asset.AssetId;
                var path = Path.Combine(dir, filename);

                await File.WriteAllTextAsync(path, asset.Xml);
            }

            return false;
        }

        #endregion


        public long GetDirectorySize()
        {
            try
            {
                if (GetDirectoryExists())
                {
                    var dir = GetDirectory();
                    var files = Directory.GetFiles(dir);
                    if (!files.IsNullOrEmpty())
                    {
                        long x = 0;

                        foreach (var file in files)
                        {
                            var fileInfo = new FileInfo(file);
                            if (fileInfo != null) x += fileInfo.Length;
                        }

                        return x;
                    }
                }            
            }
            catch (Exception) { }

            return -1;
        }


        private bool GetDirectoryExists()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer);

            return Directory.Exists(dir);
        }

        private string GetDirectory(bool createIfNotExists = true)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, DirectoryAssets);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
