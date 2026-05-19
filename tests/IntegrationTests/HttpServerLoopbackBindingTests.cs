// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text.RegularExpressions;
using MTConnect.Tests.Shared;
using Xunit;

namespace IntegrationTests
{
    /// <summary>
    /// Source-grep regression guard for the loopback-only binding contract.
    /// Pins that <see cref="MTConnect.Configurations.HttpServerConfiguration"/>
    /// in <c>ClientAgentCommunicationTests</c> binds the embedded HTTP
    /// server to loopback so an in-process integration run cannot
    /// accidentally expose the test agent on a non-loopback interface of
    /// the dev machine.
    ///
    /// Pinned via file content rather than a runtime assertion because
    /// the fixture brings the server up in its constructor and we want
    /// the guard to fail at unit-test time (not when the server is
    /// already bound to the wrong interface).
    /// </summary>
    public class HttpServerLoopbackBindingTests
    {
        /// <summary>
        /// Asserts the loopback-only binding contract by source-grep.
        /// Locates <c>ClientAgentCommunicationTests.cs</c> by recursive
        /// search under <c>tests/</c> rather than a hard-coded directory
        /// segment, so the guard survives a rename of the integration
        /// test project directory and fails loudly (rather than silently
        /// missing the file) if a second copy is ever introduced.
        /// </summary>
        [Fact]
        public void ClientAgentCommunicationTests_HttpServerConfiguration_binds_to_loopback()
        {
            var testsRoot = Path.Combine(RepoRootLocator.LocateRoot(), "tests");

            var matches = Directory.GetFiles(testsRoot,
                "ClientAgentCommunicationTests.cs", SearchOption.AllDirectories);

            Assert.True(matches.Length == 1,
                $"Expected exactly one ClientAgentCommunicationTests.cs under " +
                $"'{testsRoot}', found {matches.Length}: {string.Join(", ", matches)}");

            var src = File.ReadAllText(matches[0]);

            // The fixture must initialise HttpServerConfiguration with the
            // Server property pinned to a loopback literal inside the same
            // object initialiser that sets Port. Both the bare "127.0.0.1"
            // string and the numerically-equivalent IPAddress.Loopback
            // forms satisfy the pin; any other host string would let the
            // embedded HTTP server bind beyond loopback.
            var loopbackInitializer = new Regex(
                @"new\s+HttpServerConfiguration\s*\{[^}]*Server\s*=\s*" +
                @"(?:""127\.0\.0\.1""|IPAddress\.Loopback(?:\.ToString\(\))?|""::1"")" +
                @"[^}]*\}",
                RegexOptions.Singleline);

            Assert.True(loopbackInitializer.IsMatch(src),
                "ClientAgentCommunicationTests.cs must construct HttpServerConfiguration with " +
                "Server pinned to a loopback literal so the embedded HTTP server binds to loopback only.");
        }
    }
}
