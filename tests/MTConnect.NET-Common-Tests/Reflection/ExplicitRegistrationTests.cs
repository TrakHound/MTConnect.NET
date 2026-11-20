// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Streams;
using MTConnect.Streams.Output;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Reflection
{
    /// <summary>
    /// Pins the explicit registration API that sits beside the application-directory
    /// DLL scan: a registered formatter or module type is used even when the scan would
    /// not find it, a registered formatter wins over a scanned one with the same Id, and a
    /// module type that is both registered and scanned is loaded once.
    /// </summary>
    [TestFixture]
    public class ExplicitRegistrationTests
    {
        [Test]
        public void ResponseDocumentFormatter_Register_RoutesFormatCallsToRegisteredInstance()
        {
            var formatter = new MarkerResponseDocumentFormatter("registration-test-format", "marker-a");
            ResponseDocumentFormatter.Register(formatter);

            var result = ResponseDocumentFormatter.Format("REGISTRATION-TEST-FORMAT", (IDevicesResponseDocument)null!);

            Assert.That(result.Success, Is.True);
            Assert.That(ReadContent(result), Is.EqualTo("marker-a"));
            Assert.That(ResponseDocumentFormatter.GetContentType("registration-test-format"), Is.EqualTo("text/x-marker-a"));
        }

        [Test]
        public void ResponseDocumentFormatter_Register_ReplacesScannedFormatterWithSameId()
        {
            // ScannedResponseDocumentFormatter lives in this test assembly, so the
            // directory scan finds it and registers it under "scanned-test-format".
            Assert.That(ReadContent(ResponseDocumentFormatter.Format(ScannedResponseDocumentFormatter.FormatId, (IDevicesResponseDocument)null!)), Is.EqualTo("scanned"));

            ResponseDocumentFormatter.Register(new MarkerResponseDocumentFormatter(ScannedResponseDocumentFormatter.FormatId, "registered"));

            Assert.That(ReadContent(ResponseDocumentFormatter.Format(ScannedResponseDocumentFormatter.FormatId, (IDevicesResponseDocument)null!)), Is.EqualTo("registered"));
        }

        [Test]
        public void ResponseDocumentFormatter_Register_RejectsNullAndMissingId()
        {
            Assert.Throws<ArgumentNullException>(() => ResponseDocumentFormatter.Register(null!));
            Assert.Throws<ArgumentException>(() => ResponseDocumentFormatter.Register(new MarkerResponseDocumentFormatter(null!, "x")));
        }

        [Test]
        public void AgentModules_RegisteredTypeAlsoFoundByScan_IsLoadedOnce()
        {
            // RegistrationTestModule is in this test assembly, so the scan finds it too.
            MTConnectAgentModules.Register<RegistrationTestModule>();
            MTConnectAgentModules.Register<RegistrationTestModule>();

            Assert.That(MTConnectAgentModules.RegisteredTypes, Has.Exactly(1).EqualTo(typeof(RegistrationTestModule)));

            var configuration = new AgentApplicationConfiguration
            {
                Modules = new List<object>
                {
                    new Dictionary<object, object> { { RegistrationTestModule.ConfigurationTypeId, new Dictionary<object, object>() } }
                }
            };

            var agent = new MTConnectAgentBroker();
            var modules = new MTConnectAgentModules(configuration, agent);
            var loaded = new List<IMTConnectAgentModule>();
            modules.ModuleLoaded += (s, module) => loaded.Add(module);

            try
            {
                modules.Load();

                Assert.That(loaded, Has.Count.EqualTo(1));
                Assert.That(loaded[0], Is.InstanceOf<RegistrationTestModule>());
            }
            finally
            {
                modules.Stop();
            }
        }

        [Test]
        public void AgentModules_Register_RejectsTypesThatAreNotConcreteModules()
        {
            Assert.Throws<ArgumentNullException>(() => MTConnectAgentModules.Register(null!));
            Assert.Throws<ArgumentException>(() => MTConnectAgentModules.Register(typeof(string)));
            Assert.Throws<ArgumentException>(() => MTConnectAgentModules.Register(typeof(MTConnectAgentModule)));
        }


        private static string ReadContent(FormatWriteResult result)
        {
            using (var reader = new StreamReader(result.Content, Encoding.UTF8)) return reader.ReadToEnd();
        }
    }


    /// <summary>
    /// Agent module used only by <see cref="ExplicitRegistrationTests"/>; it is only
    /// instantiated when its configuration key is present, so other tests never load it.
    /// </summary>
    public class RegistrationTestModule : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "explicit-registration-test-module";

        public RegistrationTestModule(IMTConnectAgentBroker agent, object configuration) : base(agent) { }
    }


    /// <summary>
    /// Formatter with a parameterless constructor so the directory scan picks it up.
    /// </summary>
    public class ScannedResponseDocumentFormatter : MarkerResponseDocumentFormatter
    {
        public const string FormatId = "scanned-test-format";

        public ScannedResponseDocumentFormatter() : base(FormatId, "scanned") { }
    }


    /// <summary>
    /// Formatter whose output is a fixed marker string, so a test can tell which
    /// instance served a call. It has no parameterless constructor, so the scan skips it.
    /// </summary>
    public class MarkerResponseDocumentFormatter : IResponseDocumentFormatter
    {
        private readonly string _marker;

        public MarkerResponseDocumentFormatter(string id, string marker)
        {
            Id = id;
            _marker = marker;
        }

        public string Id { get; }

        public string ContentType => "text/x-" + _marker;

        private FormatWriteResult Write() => FormatWriteResult.Successful(new MemoryStream(Encoding.UTF8.GetBytes(_marker)), ContentType);

        public FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>>? options = null) => Write();
        public FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>>? options = null) => Write();
        public FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>>? options = null) => Write();
        public FormatWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>>? options = null) => Write();

        public FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>>? options = null) => FormatReadResult<IDevicesResponseDocument>.Successful(null!);
        public FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>>? options = null) => FormatReadResult<IStreamsResponseDocument>.Successful(null!);
        public FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>>? options = null) => FormatReadResult<IAssetsResponseDocument>.Successful(null!);
        public FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>>? options = null) => FormatReadResult<IErrorResponseDocument>.Successful(null!);
    }
}
