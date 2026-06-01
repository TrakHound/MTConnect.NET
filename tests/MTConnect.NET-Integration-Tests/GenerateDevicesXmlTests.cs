using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MTConnect.Tests.Integration
{
    // Regression tests for the GenerateDevicesXml helper. The helper takes a
    // fileName argument; earlier revisions hard-coded "devices.xml" inside
    // File.Create(...) so the argument was silently ignored. These tests pin
    // the contract that the file is created at the requested path.
    /// <summary>Represents the generate devices xml tests.</summary>
    public class GenerateDevicesXmlTests : IDisposable
    {
        private readonly string _tempDir;

        /// <summary>Initialises a new instance of the generate devices xml tests type.</summary>
        public GenerateDevicesXmlTests()
        {
            _tempDir = Path.Combine(
                Path.GetTempPath(),
                "mtconnect-integration-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        /// <summary>Runs the dispose operation.</summary>
        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, recursive: true);
                }
            }
            catch
            {
                // best-effort cleanup; CI temp roots get scrubbed periodically
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: generate devices xml honours file name argument.</summary>
        [Fact]
        public void GenerateDevicesXml_HonoursFileNameArgument()
        {
            var fileName = Path.Combine(_tempDir, "custom-devices.xml");
            var machineId = Guid.NewGuid().ToString();
            var machineName = "MRegressionC13";

            ClientAgentCommunicationTests.GenerateDevicesXml(
                machineId,
                machineName,
                fileName,
                NullLogger.Instance);

            Assert.True(
                File.Exists(fileName),
                $"Expected GenerateDevicesXml to write to '{fileName}', but the file was not created.");

            var content = File.ReadAllText(fileName);
            Assert.Contains(machineId, content);
            Assert.Contains(machineName, content);
        }
    }
}
