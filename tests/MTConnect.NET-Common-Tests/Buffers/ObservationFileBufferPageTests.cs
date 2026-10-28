// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Buffers
{
    /// <summary>
    /// Pins that observation page files keep every written observation when later writes append to an
    /// existing page (compressed and uncompressed), including the observation whose sequence is an
    /// exact multiple of the page size.
    /// </summary>
    [TestFixture]
    [Category("ObservationFileBuffer")]
    public class ObservationFileBufferPageTests
    {
        private string _dir = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _dir = Path.Combine(Path.GetTempPath(), "mtconnect-filebuffer-" + Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { }
        }

        /// <summary>Pins the behaviour expressed by the test name: two writes to the same page keep all observations.</summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Writes_to_an_existing_page_keep_all_observations(bool useCompression)
        {
            // Sequences 1..100 all belong to page "100" (the page-size multiple included)
            WriteBatch(useCompression, 1, 50);
            WaitForLines(useCompression, 50);

            WriteBatch(useCompression, 51, 100);
            WaitForLines(useCompression, 100);
        }

        private void WriteBatch(bool useCompression, ulong first, ulong last)
        {
            var buffer = new MTConnectObservationFileBuffer(new AgentConfiguration { ObservationBufferSize = 1000 }, _dir);
            buffer.UseCompression = useCompression;

            for (var sequence = first; sequence <= last; sequence++)
            {
                var values = new[] { new ObservationValue(ValueKeys.Result, sequence.ToString()) };
                buffer.Add(new BufferObservation(1, DataItemCategory.EVENT, DataItemRepresentation.VALUE, values, sequence, UnixDateTime.Now, Quality.VALID, false, false));
            }

            // Dispose flushes the queued observations
            buffer.Dispose();
        }

        private void WaitForLines(bool useCompression, int expected)
        {
            var sw = Stopwatch.StartNew();
            var lines = 0;
            while (sw.ElapsedMilliseconds < 10000)
            {
                lines = CountLines(useCompression);
                if (lines == expected) return;
                Thread.Sleep(50);
            }

            Assert.That(lines, Is.EqualTo(expected), "Page file should contain every written observation.");
        }

        private int CountLines(bool useCompression)
        {
            if (!Directory.Exists(_dir)) return 0;

            var page = Directory.GetFiles(_dir, "100", SearchOption.AllDirectories).FirstOrDefault();
            if (page == null) return 0;

            try
            {
                string text;
                if (useCompression)
                {
                    using (var file = new FileStream(page, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var gzip = new GZipStream(file, CompressionMode.Decompress))
                    using (var reader = new StreamReader(gzip, Encoding.ASCII))
                    {
                        text = reader.ReadToEnd();
                    }
                }
                else
                {
                    using (var file = new FileStream(page, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(file, Encoding.ASCII))
                    {
                        text = reader.ReadToEnd();
                    }
                }

                return text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
            catch (IOException)
            {
                return -1; // Still being written
            }
            catch (InvalidDataException)
            {
                return -1;
            }
        }
    }
}
