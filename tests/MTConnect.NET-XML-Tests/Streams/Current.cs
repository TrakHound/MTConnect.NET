using MTConnect.Streams.Output;
using MTConnect.Streams.Xml;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MTConnect.Tests.XML.Streams
{
    public class Current
    {
        [Test]
        public void ReadStreamTestFiles()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Streams-Files");
            var files = Directory.GetFiles(dir);
            if (files.Any())
            {
                foreach (var file in files)
                {
                    var originalBytes = File.ReadAllBytes(file);

                    var doc = XmlStreamsResponseDocument.FromXml(originalBytes);
                    
                    Assert.That(doc != null, $"Read Original : {file}");
                }
            }
        }
    }
}
