// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices.DataItems;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Persistent File backed Buffer used to store MTConnect Observation Data
    /// </summary>
    public class MTConnectObservationFileBuffer : MTConnectObservationBuffer, IDisposable
    {
        public const int DefaultPageSize = 100;
        public const string DirectoryBuffer = "buffer";
        public const string DirectoryObservations = "observations";
        public const string DirectoryCurrent = "current";

        private readonly MTConnectObservationQueue _items;
        private readonly MTConnectObservationQueue _currentItems;
        private CancellationTokenSource stop;
        private bool _isStarted;
        private bool _isLoading;


        public int WriteInterval { get; set; } = 1000;

        public int RetentionInterval { get; set; } = 5000;

        public int PageSize { get; set; } = DefaultPageSize;

        public int MaxRecordsPerFile { get; set; } = 5000;

        public int MaxItemsPerWrite { get; set; } = 50000;

        public long QueuedItemCount => _items.Count;

        public bool UseCompression { get; set; } = true;

        public EventHandler BufferLoadStarted { get; set; }

        public EventHandler<ObservationBufferLoadArgs> BufferLoadCompleted { get; set; }

        public EventHandler<ObservationBufferRetentionArgs> BufferRetentionCompleted { get; set; }


        public MTConnectObservationFileBuffer()
        {
            _items = new MTConnectObservationQueue();

            Start();
        }

        public MTConnectObservationFileBuffer(AgentConfiguration configuration) : base(configuration)
        {
            _items = new MTConnectObservationQueue();
            _currentItems = new MTConnectObservationQueue();

            Start();
        }


        protected override void OnCurrentChange(IEnumerable<StoredObservation> observations)
        {
            if (!_isLoading)
            {
                AddCurrent(observations);
            }
        }

        protected override void OnBufferObservationAdd(long bufferIndex, StoredObservation observation)
        {
            if (!_isLoading)
            {
                Add(observation);
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
                _ = Task.Run(() => RetentionWorker(stop.Token));
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


        #region "Add"

        public bool Add(StoredObservation observation)
        {
            // Add to internal Queue
            return _items.Add(observation);
        }

        public bool Add(IEnumerable<StoredObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var success = true;

                foreach (var observation in observations)
                {
                    success = Add(observation);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }


        public bool AddCurrent(StoredObservation observation)
        {
            // Add to internal Queue
            return _currentItems.Add(observation);
        }

        public bool AddCurrent(IEnumerable<StoredObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var success = true;

                foreach (var observation in observations)
                {
                    success = AddCurrent(observation);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Load"

        public bool Load()
        {
            var found = false;
            _isLoading = true;

            var stpw = System.Diagnostics.Stopwatch.StartNew();
            if (BufferLoadStarted != null) BufferLoadStarted.Invoke(this, new EventArgs());

            // Read Observations from Page Files
            var observations = ReadStoredObservations();
            if (!observations.IsNullOrEmpty())
            {
                var oObservations = observations.OrderBy(o => o.Sequence);
                var sequence = oObservations.LastOrDefault().Sequence;

                SetSequence(1);
                AddBufferObservations(oObservations);
                SetSequence(sequence + 1);

                found = true;
            }

            // Read Current Observations from Device Files
            var currentObservations = ReadStoredCurrentObservations();
            if (!currentObservations.IsNullOrEmpty())
            {
                foreach (var observation in currentObservations)
                {
                    if (observation.DataItemCategory == DataItemCategory.CONDITION)
                    {
                        // Add to Current Conditions
                        AddCurrentCondition(observation);
                    }
                    else
                    {
                        // Add to Current Observations
                        AddCurrentObservation(observation);
                    }
                }
            }

            stpw.Stop();
            if (found && BufferLoadCompleted != null) BufferLoadCompleted.Invoke(this, new ObservationBufferLoadArgs(observations.Count(), stpw.ElapsedMilliseconds));

            _isLoading = false;
            return found;
        }

        #endregion

        #region "Read"

        private IEnumerable<StoredObservation> ReadStoredObservations()
        {
            var items = new ConcurrentBag<StoredObservation>();

            try
            {
                // Get Observations Directory Path
                var dir = GetDirectory();

                // Get a list of all observation buffer files in directory
                var files = Directory.GetFiles(dir);
                if (!files.IsNullOrEmpty())
                {
                    Parallel.ForEach(files, file =>
                    {
                        var fileItems = ReadFile(file);
                        foreach (var item in fileItems) items.Add(item);
                    });
                }
            }
            catch { }

            return items;
        }

        private IEnumerable<StoredObservation> ReadStoredCurrentObservations()
        {
            var items = new List<StoredObservation>();

            try
            {
                // Get a list of all observation buffer files in directory
                var files = GetCurrentFiles();
                foreach (var file in files)
                {
                    var fileItems = ReadFile(file);
                    items.AddRange(fileItems);
                }
            }
            catch { }

            return items;
        }


        private static long GetSequenceTop(long n, int pageSize = DefaultPageSize)
        {
            if (pageSize > 0)
            {
                var x = n / pageSize;
                x = x * pageSize;

                var y = 0;
                if (n % pageSize > 0) y = pageSize;

                return x + y;
            }

            return 0;
        }

        private static long GetSequenceBottom(long n, int pageSize = DefaultPageSize)
        {
            var top = GetSequenceTop(n, pageSize);
            if (top > 0)
            {
                return top - pageSize;
            }

            return 0;
        }


        private IEnumerable<string> GetFiles(long from, long to, int pageSize = DefaultPageSize)
        {
            var found = new List<string>();

            // Get Observations Directory Path
            var dir = GetDirectory();

            // Get a list of all observation buffer files in directory
            var files = Directory.GetFiles(dir);
            if (!files.IsNullOrEmpty())
            {
                var x = new ConcurrentDictionary<long, string>();
                Parallel.ForEach(files, file =>
                {
                    var sequence = long.Parse(Path.GetFileName(file));
                    x.TryAdd(sequence, file);
                });

                long m = GetSequenceTop(from);
                var n = GetSequenceTop(to);

                string s;
                if (x.TryGetValue(m, out s)) found.Add(s);

                do
                {
                    m += pageSize;

                    if (x.TryGetValue(m, out s)) found.Add(s);
                }
                while (m < n);
            }

            return found;
        }

        private IEnumerable<string> GetCurrentFiles()
        {
            // Get Current Observations Directory Path
            var dir = GetCurrentDirectory();

            // Get a list of all current observation buffer files in directory
            return Directory.GetFiles(dir);
        }


        private IEnumerable<StoredObservation> ReadFile(string path)
        {
            var items = new List<StoredObservation>();

            if (File.Exists(path))
            {
                var lines = ReadFileLines(path);
                items.AddRange(ReadObservations(lines));
            }

            return items;
        }

        private string[] ReadFileLines(string path)
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

                                var s = Encoding.ASCII.GetString(bytes);
                                if (!string.IsNullOrEmpty(s))
                                {
                                    return s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                                }
                            }
                        }
                    }
                    else
                    {
                        return File.ReadAllLines(path);
                    }
                }
                catch { }
            }

            return null;
        }

        private static IEnumerable<StoredObservation> ReadObservations(string[] lines)
        {
            if (!lines.IsNullOrEmpty())
            {
                var items = new List<StoredObservation>();

                foreach (var line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            var a = JsonSerializer.Deserialize<object[]>(line);
                            var fileObservation = FileObservation.FromArray(a);
                            var observation = fileObservation.ToStoredObservation();
                            if (observation.IsValid)
                            {
                                items.Add(observation);
                            }
                        }
                        catch { }
                    }
                }

                return items;
            }

            return Enumerable.Empty<StoredObservation>();
        }

        #endregion

        #region "Retention"

        private async Task RetentionWorker(CancellationToken cancellationToken)
        {
            // Delay half the Interval to offset from WriteWorker
            await Task.Delay(WriteInterval / 2, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(RetentionInterval, cancellationToken);

                    RunRetention();
                }
                catch (TaskCanceledException) { }
                catch { }
            }
        }

        private void RunRetention()
        {
            if (FirstSequence > 1)
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var from = 0;
                var to = GetSequenceBottom(FirstSequence - 1);

                var sequenceFiles = GetFiles(from, to);
                if (!sequenceFiles.IsNullOrEmpty())
                {
                    foreach (var sequenceFile in sequenceFiles)
                    {
                        File.Delete(sequenceFile);
                    }
                }

                stpw.Stop();
                if (BufferRetentionCompleted != null) BufferRetentionCompleted.Invoke(this, new ObservationBufferRetentionArgs(from, to, sequenceFiles.Count(), stpw.ElapsedMilliseconds));
            }
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
                    await WriteCurrentItems(MaxItemsPerWrite);
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
                await WriteToFiles(queueItems);
            }
        }

        private async Task WriteCurrentItems(int maxItems)
        {
            var queueItems = _currentItems.Take(maxItems);
            if (!queueItems.IsNullOrEmpty())
            {
                await WriteToCurrentFiles(queueItems);
            }
        }


        /// <summary>
        /// Write the list of Observations to to files with unique filenames
        /// </summary>
        private async Task<int> WriteToFiles(IEnumerable<StoredObservation> observations)
        {
            int writtenTotal = 0;

            if (!observations.IsNullOrEmpty())
            {
                try
                {
                    var lines = new StringBuilder();

                    var oObservations = observations.OrderBy(o => o.Sequence);
                    var firstSequence = oObservations.FirstOrDefault().Sequence;
                    var lastSequence = oObservations.LastOrDefault().Sequence;

                    var skip = 0;
                    var m = firstSequence;
                    var n = lastSequence;

                    do
                    {
                        // Get Observations to write to Page file
                        var pageObservations = oObservations.Skip(skip).Take(PageSize);
                        if (!pageObservations.IsNullOrEmpty())
                        {
                            // Get the Observations Directory
                            var dir = GetDirectory();

                            // Filename is the largest Sequence within the PageSize
                            var file = GetSequenceTop(m, PageSize).ToString();
                            var path = Path.Combine(dir, file);

                            // Write the Observations to the Page File
                            await WriteToFile(path, pageObservations);
                        }

                        m += PageSize;
                        skip += PageSize;
                    }
                    while (m < n);
                }
                catch { }
            }

            return writtenTotal;
        }

        /// <summary>
        /// Write the list of Current Observations to individual files for each Device
        /// </summary>
        private async Task<int> WriteToCurrentFiles(IEnumerable<StoredObservation> observations)
        {
            int writtenTotal = 0;

            if (!observations.IsNullOrEmpty())
            {
                var dir = GetCurrentDirectory();

                var deviceUuids = observations.Select(o => o.DeviceUuid).Distinct();
                foreach (var deviceUuid in deviceUuids)
                {
                    var path = Path.Combine(dir, deviceUuid);

                    // Get List of Observations for Device UUID
                    var deviceObservations = observations.Where(o => o.DeviceUuid == deviceUuid);

                    var currentObservations = new List<StoredObservation>();
                    var dataItemIds = deviceObservations.Select(o => o.DataItemId).Distinct();
                    foreach (var dataItemId in dataItemIds)
                    {
                        currentObservations.Add(deviceObservations.Where(o => o.DataItemId == dataItemId).OrderByDescending(o => o.Sequence).FirstOrDefault());
                    }

                    // Write the Observations to the Page File
                    await WriteToFile(path, currentObservations, false);
                }
            }

            return writtenTotal;
        }

        private async Task<bool> WriteToFile(string path, IEnumerable<StoredObservation> observations, bool append = true)
        {
            if (!string.IsNullOrEmpty(path) && !observations.IsNullOrEmpty())
            {
                var fileObservations = new List<FileObservation>();

                if (append)
                {
                    // Read File Contents
                    var fileLines = ReadFileLines(path);
                    if (!fileLines.IsNullOrEmpty())
                    {
                        foreach (var line in fileLines)
                        {
                            try
                            {
                                var a = JsonSerializer.Deserialize<object[]>(line);
                                fileObservations.Add(FileObservation.FromArray(a));
                            }
                            catch { }
                        }
                    }
                }

                // Append new Contents
                foreach (var observation in observations)
                {
                    fileObservations.Add(new FileObservation(observation));
                }

                // Order File Observations by Sequence (the order they will appear in the Page File)
                var oFileObservations = fileObservations.OrderBy(o => o.Sequence);

                // Serialize File Observations to JSON
                var sb = new StringBuilder();
                foreach (var fileObservation in oFileObservations)
                {
                    try
                    {
                        // Append new JSON line
                        var json = JsonSerializer.Serialize(fileObservation.ToArray());
                        if (!string.IsNullOrEmpty(json))
                        {
                            sb.Append(json);
                            sb.Append("\r\n");
                        }
                    }
                    catch { }
                }

                try
                {
                    if (UseCompression)
                    {
                        // Get Bytes
                        var bytes = Encoding.ASCII.GetBytes(sb.ToString());

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
                        File.WriteAllText(path, sb.ToString());
                    }

                    return true;
                }
                catch { }
            }

            return false;
        }

        #endregion


        private string GetDirectory(bool createIfNotExists = true)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, DirectoryObservations);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }

        private string GetCurrentDirectory(bool createIfNotExists = true)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, DirectoryObservations, DirectoryCurrent);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
