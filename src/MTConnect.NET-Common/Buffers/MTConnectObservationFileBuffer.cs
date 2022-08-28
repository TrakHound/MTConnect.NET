// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Configurations;
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
        private const int DefaultPageSize = 100;
        private const string DirectoryBuffer = "buffer";
        private const string DirectoryObservations = "observations";
        private const string DirectoryCurrent = "current";

        private readonly object _lock = new object();
        private readonly MTConnectObservationQueue _items;
        private CancellationTokenSource stop;
        private bool _isStarted;
        private bool _isLoading;
        private bool _currentUpdated;
        private bool _currentConditionUpdated;


        public int WriteInterval { get; set; } = 5000;

        public int RetentionInterval { get; set; } = 10000;

        public int PageSize { get; set; } = DefaultPageSize;

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

        public MTConnectObservationFileBuffer(IAgentConfiguration configuration) : base(configuration)
        {
            _items = new MTConnectObservationQueue();
            Start();
        }


        protected override void OnCurrentObservationAdd(ref BufferObservation observation)
        {
            if (!_isLoading)
            {
                lock (_lock) _currentUpdated = true;
            }
        }

        protected override void OnCurrentConditionAdd(ref IEnumerable<BufferObservation> observations)
        {
            if (!_isLoading)
            {
                lock (_lock) _currentConditionUpdated = true;
            }
        }

        protected override void OnBufferObservationAdd(ref BufferObservation observation)
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

                _ = Task.Run(WriteWorker, stop.Token);
                _ = Task.Run(RetentionWorker, stop.Token);
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
            base.Dispose();
        }


        public static void Reset()
        {
            var dir = GetDirectory(false);
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


        #region "Add"

        public bool Add(BufferObservation observation)
        {
            // Add to internal Queue
            return _items.Add(observation);
        }

        public bool Add(IEnumerable<BufferObservation> observations)
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

        #endregion

        #region "Load"

        public bool Load()
        {
            var found = false;
            _isLoading = true;

            var stpw = System.Diagnostics.Stopwatch.StartNew();
            if (BufferLoadStarted != null) BufferLoadStarted.Invoke(this, new EventArgs());

            // Read Observations from Page Files
            var observations = ReadBufferObservations();
            if (!observations.IsNullOrEmpty())
            {
                // Get last Sequence
                var sequence = observations[observations.Length - 1].Sequence;

                SetSequence(1);

                // Add Observations to the Buffer
                AddBufferObservations(ref observations);

                SetSequence(sequence + 1);

                found = true;
            }

            // Read Current Observations from Device Files
            var currentObservations = ReadBufferCurrentObservations();
            if (!currentObservations.IsNullOrEmpty())
            {
                for (var i = 0; i < currentObservations.Length; i++)
                {
                    // Add to Current Observations
                    AddCurrentObservation(currentObservations[i]);
                }
            }

            // Read Current Observations from Device Files
            var currentConditions = ReadBufferCurrentConditions();
            if (!currentConditions.IsNullOrEmpty())
            {
                for (var i = 0; i < currentConditions.Length; i++)
                {
                    // Add to Current Conditions
                    AddCurrentObservation(currentConditions[i]);
                }
            }

            stpw.Stop();
            if (found && BufferLoadCompleted != null) BufferLoadCompleted.Invoke(this, new ObservationBufferLoadArgs(observations.Length, stpw.ElapsedMilliseconds));

            observations = null;
            _isLoading = false;

            if (found) GarbageCollector.HighPriorityCollect();

            return found;
        }

        #endregion

        #region "Read"

        private BufferObservation[] ReadBufferObservations()
        {
            try
            {
                // Get Observations Directory Path
                var dir = GetDirectory();

                // Get a list of all observation buffer files in directory
                var files = Directory.GetFiles(dir);
                if (!files.IsNullOrEmpty())
                {
                    // Create a list of BufferFile objects (contains sequence information)
                    var bufferFiles = new List<BufferFile>();
                    foreach (var file in files)
                    {
                        bufferFiles.Add(new BufferFile(file, PageSize));
                    }

                    // Sort by Sequence (this is important as the buffer needs to be loaded in the same order it was written)
                    var oBufferFiles = bufferFiles.OrderBy(o => o.SequenceBottom);

                    var items = new BufferObservation[files.Length * PageSize];
                    var startSequence = oBufferFiles.FirstOrDefault().SequenceBottom;
                    int itemCount = 0;

                    foreach (var file in oBufferFiles)
                    {
                        // Read the file and add the observations to the Items array
                        itemCount += ReadFile(ref items, file.Path, itemCount);
                    }

                    // Resize the return array
                    Array.Resize(ref items, itemCount);

                    return items;
                }
            }
            catch { }

            return null;
        }

        private BufferObservation[] ReadBufferCurrentObservations()
        {
            var items = new List<BufferObservation>();

            try
            {
                // Get a list of all observation buffer files in directory
                var files = GetCurrentFiles();
                foreach (var file in files)
                {
                    var fileItems = ReadCurrentFile(file);
                    items.AddRange(fileItems);
                }
            }
            catch { }

            return items.ToArray();
        }

        private BufferObservation[] ReadBufferCurrentConditions()
        {
            var items = new List<BufferObservation>();

            try
            {
                // Get a list of all observation buffer files in directory
                var files = GetCurrentConditionFiles();
                foreach (var file in files)
                {
                    var fileItems = ReadCurrentFile(file);
                    items.AddRange(fileItems);
                }
            }
            catch { }

            return items.ToArray();
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
            return Directory.GetFiles(dir, "*_observations");
        }

        private IEnumerable<string> GetCurrentConditionFiles()
        {
            // Get Current Observations Directory Path
            var dir = GetCurrentDirectory();

            // Get a list of all current observation buffer files in directory
            return Directory.GetFiles(dir, "*_conditions");
        }


        private int ReadFile(ref BufferObservation[] observations, string path, int startIndex)
        {
            if (File.Exists(path))
            {
                var lines = ReadFileLines(path);
                if (lines != null) return ReadObservations(ref observations, lines, startIndex);
            }

            return 0;
        }

        private IEnumerable<BufferObservation> ReadCurrentFile(string path)
        {
            var items = new List<BufferObservation>();

            if (File.Exists(path))
            {
                var lines = ReadFileLines(path);
                if (lines != null) items.AddRange(ReadCurrentObservations(lines));
            }

            return items;
        }

        private string[] ReadFileLines(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {          
                try
                {
                    if (File.Exists(path))
                    {
                        if (UseCompression)
                        {
                            using (var fileStream = File.Open(path, FileMode.Open))
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
                        else
                        {
                            return File.ReadAllLines(path);
                        }
                    }                 
                }
                catch { }
            }

            return null;
        }

        private static int ReadObservations(ref BufferObservation[] observations, string[] lines, int startIndex)
        {
            if (lines != null && lines.Length > 0)
            {
                var index = startIndex;
                var count = 0;

                foreach (var line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            var a = JsonSerializer.Deserialize<object[]>(line);
                            var fileObservation = FileObservation.FromArray(a);
                            var observation = fileObservation.ToBufferObservation();
                            if (observation.IsValid)
                            {
                                observations[index] = observation;
                                index++;
                                count++;
                            }
                        }
                        catch { }
                    }
                }

                return count;
            }

            return 0;
        }

        private static IEnumerable<BufferObservation> ReadCurrentObservations(string[] lines)
        {
            if (!lines.IsNullOrEmpty())
            {
                var items = new List<BufferObservation>();

                foreach (var line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            var a = JsonSerializer.Deserialize<object[]>(line);
                            var fileObservation = FileCurrentObservation.FromArray(a);
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

            return Enumerable.Empty<BufferObservation>();
        }

        #endregion

        #region "Retention"

        private async Task RetentionWorker()
        {
            // Delay half the Interval to offset from WriteWorker
            await Task.Delay(WriteInterval / 2, stop.Token);

            while (!stop.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(RetentionInterval, stop.Token);

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

        private async Task WriteWorker()
        {
            while (!stop.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(WriteInterval, stop.Token);

                    await WriteItems(MaxItemsPerWrite);
                    await WriteCurrentItems();
                    await WriteCurrentConditionItems();
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

        private async Task WriteCurrentItems()
        {
            bool update;
            lock (_lock)
            {
                update = _currentUpdated;
                _currentUpdated = false;
            }

            if (update)
            {
                var writeItems = GetCurrentObservations();
                if (!writeItems.IsNullOrEmpty())
                {
                    await WriteToCurrentFiles(writeItems);
                }
            }
        }

        private async Task WriteCurrentConditionItems()
        {
            bool update;
            lock (_lock)
            {
                update = _currentConditionUpdated;
                _currentConditionUpdated = false;
            }
            
            if (update)
            {
                var writeItems = GetCurrentConditions();
                if (!writeItems.IsNullOrEmpty())
                {
                    await WriteToCurrentConditionFiles(writeItems);
                }
            }
        }


        /// <summary>
        /// Write the list of Observations to to files with unique filenames
        /// </summary>
        private async Task<int> WriteToFiles(IEnumerable<BufferObservation> observations)
        {
            int writtenTotal = 0;

            if (!observations.IsNullOrEmpty())
            {
                try
                {
                    var lines = new StringBuilder();

                    // Get the Observations Directory
                    var dir = GetDirectory();

                    var files = observations.Select(o => GetSequenceTop(o.Sequence, PageSize)).Distinct();
                    foreach (var file in files)
                    {
                        var bottom = GetSequenceBottom(file, PageSize);

                        var fileObservations = observations.Where(o => o.Sequence >= bottom && o.Sequence < file);
                        var oObservations = fileObservations.OrderBy(o => o.Sequence);

                        var path = Path.Combine(dir, file.ToString());

                        // Write the Observations to the Page File
                        await WriteToFile(path, oObservations);
                    }
                }
                catch { }
            }

            return writtenTotal;
        }

        /// <summary>
        /// Write the list of Current Observations to individual files for each Device
        /// </summary>
        private async Task<int> WriteToCurrentFiles(IEnumerable<BufferObservation> observations)
        {
            int writtenTotal = 0;

            if (!observations.IsNullOrEmpty())
            {
                var dir = GetCurrentDirectory();

                var deviceIndexes = observations.Select(o => o.DeviceIndex).Distinct();
                foreach (var deviceIndex in deviceIndexes)
                {
                    var filename = $"{deviceIndex}_observations";
                    var path = Path.Combine(dir, filename);

                    // Get List of Observations for Device UUID
                    var deviceObservations = observations.Where(o => o.DeviceIndex == deviceIndex);

                    var currentObservations = new List<BufferObservation>();
                    var dataItemIds = deviceObservations.Select(o => o.DataItemIndex).Distinct();
                    foreach (var dataItemId in dataItemIds)
                    {
                        currentObservations.Add(deviceObservations.Where(o => o.DataItemIndex == dataItemId).OrderByDescending(o => o.Sequence).FirstOrDefault());
                    }

                    // Write the Observations to the Page File
                    await WriteToCurrentFile(path, currentObservations, false);
                }
            }

            return writtenTotal;
        }

        /// <summary>
        /// Write the list of Current Observations to individual files for each Device
        /// </summary>
        private async Task<int> WriteToCurrentConditionFiles(IEnumerable<BufferObservation> observations)
        {
            int writtenTotal = 0;

            if (!observations.IsNullOrEmpty())
            {
                var dir = GetCurrentDirectory();

                var deviceIndexes = observations.Select(o => o.DeviceIndex).Distinct();
                foreach (var deviceIndex in deviceIndexes)
                {
                    var filename = $"{deviceIndex}_conditions";
                    var path = Path.Combine(dir, filename);

                    // Get List of Observations for Device UUID
                    var deviceObservations = observations.Where(o => o.DeviceIndex == deviceIndex);

                    var currentObservations = new List<BufferObservation>();
                    var dataItemIds = deviceObservations.Select(o => o.DataItemIndex).Distinct();
                    foreach (var dataItemId in dataItemIds)
                    {
                        currentObservations.AddRange(deviceObservations.Where(o => o.DataItemIndex == dataItemId).OrderByDescending(o => o.Sequence));
                    }

                    // Write the Observations to the Page File
                    await WriteToCurrentFile(path, currentObservations, false);
                }
            }

            return writtenTotal;
        }

        private async Task<bool> WriteToFile(string path, IEnumerable<BufferObservation> observations, bool append = true)
        {
            if (!string.IsNullOrEmpty(path) && !observations.IsNullOrEmpty())
            {
                var fileObservations = new List<FileObservation>();
                var sb = new StringBuilder();

                if (append)
                {
                    // Read File Contents
                    var fileLines = ReadFileLines(path);
                    if (!fileLines.IsNullOrEmpty())
                    {
                        foreach (var line in fileLines)
                        {
                            sb.Append(line);
                            sb.Append("\r\n");
                        }
                    }
                }

                // Append new Observations
                foreach (var observation in observations)
                {
                    fileObservations.Add(new FileObservation(observation));
                }

                // Serialize File Observations to JSON
                foreach (var fileObservation in fileObservations)
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

        private async Task<bool> WriteToCurrentFile(string path, IEnumerable<BufferObservation> observations, bool append = true)
        {
            if (!string.IsNullOrEmpty(path) && !observations.IsNullOrEmpty())
            {
                var fileObservations = new List<FileCurrentObservation>();

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
                                fileObservations.Add(FileCurrentObservation.FromArray(a));
                            }
                            catch { }
                        }
                    }
                }

                // Append new Contents
                foreach (var observation in observations)
                {
                    fileObservations.Add(new FileCurrentObservation(observation));
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


        private static string GetDirectory(bool createIfNotExists = true)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, DirectoryObservations);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }

        private static string GetCurrentDirectory(bool createIfNotExists = true)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, DirectoryObservations, DirectoryCurrent);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
