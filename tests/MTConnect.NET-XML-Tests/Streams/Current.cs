using MTConnect.Streams.Output;
using MTConnect.Streams.Xml;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

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
                    using var fileStream = File.OpenRead(file);
                    using var xmlReader = XmlReader.Create(fileStream);

                    var doc = XmlStreamsResponseDocument.ReadXml(xmlReader);
                    
                    Assert.That(doc != null, $"Read Original : {file}");
                }
            }
        }
    }
}
