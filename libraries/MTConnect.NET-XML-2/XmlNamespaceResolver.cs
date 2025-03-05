// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace MTConnect
{
    //internal class IgnoreNameTable : XmlNameTable
    //{
    
    //}


    internal class XmlExtendableReader : XmlWrappingReader
    {
        private bool _ignoreNamespace { get; set; }

        public XmlExtendableReader(TextReader input, XmlReaderSettings settings, bool ignoreNamespace = false)
        : base(XmlReader.Create(input, settings))
        {
            _ignoreNamespace = ignoreNamespace;
        }

        public override string NamespaceURI
        {
            get
            {
                return _ignoreNamespace ? String.Empty : base.NamespaceURI;
            }
        }
    }

    internal class IgnoreNamespaceXmlTextReader : XmlTextReader
    {
        public IgnoreNamespaceXmlTextReader(TextReader reader) : base(reader)
        {
        }

        public override string NamespaceURI => "";

        public override string LookupNamespace(string prefix)
        {
            return "DEBUG";
        }
    }

    internal class XmlNamespaceResolver : XmlUrlResolver
    {
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return base.ResolveUri(baseUri, relativeUri);
        }

        public override bool SupportsType(Uri absoluteUri, Type type)
        {
            return true;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            // Ignore namespace resolution
            return base.GetEntity(absoluteUri, "", ofObjectToReturn);
        }
    }


}


//------------------------------------------------------------------------------
// <copyright file="XmlWrappingReader.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
//------------------------------------------------------------------------------



namespace System.Xml
{

    internal partial class XmlWrappingReader : XmlReader, IXmlLineInfo
    {

        //
        // Fields
        //
        protected XmlReader reader;
        protected IXmlLineInfo readerAsIXmlLineInfo;

        // 
        // Constructor
        //
        internal XmlWrappingReader(XmlReader baseReader)
        {
            Debug.Assert(baseReader != null);
            this.reader = baseReader;
            this.readerAsIXmlLineInfo = baseReader as IXmlLineInfo;
        }

        //
        // XmlReader implementation
        //
        public override XmlReaderSettings Settings { get { return reader.Settings; } }
        public override XmlNodeType NodeType { get { return reader.NodeType; } }
        public override string Name { get { return reader.Name; } }
        public override string LocalName { get { return reader.LocalName; } }
        public override string NamespaceURI { get { return reader.NamespaceURI; } }
        public override string Prefix { get { return reader.Prefix; } }
        public override bool HasValue { get { return reader.HasValue; } }
        public override string Value { get { return reader.Value; } }
        public override int Depth { get { return reader.Depth; } }
        public override string BaseURI { get { return reader.BaseURI; } }
        public override bool IsEmptyElement { get { return reader.IsEmptyElement; } }
        public override bool IsDefault { get { return reader.IsDefault; } }
        public override XmlSpace XmlSpace { get { return reader.XmlSpace; } }
        public override string XmlLang { get { return reader.XmlLang; } }
        public override System.Type ValueType { get { return reader.ValueType; } }
        public override int AttributeCount { get { return reader.AttributeCount; } }
        public override bool EOF { get { return reader.EOF; } }
        public override ReadState ReadState { get { return reader.ReadState; } }
        public override bool HasAttributes { get { return reader.HasAttributes; } }
        public override XmlNameTable NameTable { get { return reader.NameTable; } }
        public override bool CanResolveEntity { get { return reader.CanResolveEntity; } }

#if !SILVERLIGHT
        public override IXmlSchemaInfo SchemaInfo { get { return reader.SchemaInfo; } }
        public override char QuoteChar { get { return reader.QuoteChar; } }
#endif

        public override string GetAttribute(string name)
        {
            return reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return reader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(int i)
        {
            return reader.GetAttribute(i);
        }

        public override bool MoveToAttribute(string name)
        {
            return reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return reader.MoveToAttribute(name, ns);
        }

        public override void MoveToAttribute(int i)
        {
            reader.MoveToAttribute(i);
        }

        public override bool MoveToFirstAttribute()
        {
            return reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return reader.MoveToNextAttribute();
        }

        public override bool MoveToElement()
        {
            return reader.MoveToElement();
        }

        public override bool Read()
        {
            return reader.Read();
        }

        public override void Close()
        {
            reader.Close();
        }

        public override void Skip()
        {
            reader.Skip();
        }

        public override string LookupNamespace(string prefix)
        {
            return reader.LookupNamespace(prefix);
        }

        public override void ResolveEntity()
        {
            reader.ResolveEntity();
        }

        public override bool ReadAttributeValue()
        {
            return reader.ReadAttributeValue();
        }

        //
        // IXmlLineInfo members
        //
        public virtual bool HasLineInfo()
        {
            return (readerAsIXmlLineInfo == null) ? false : readerAsIXmlLineInfo.HasLineInfo();
        }

        public virtual int LineNumber
        {
            get
            {
                return (readerAsIXmlLineInfo == null) ? 0 : readerAsIXmlLineInfo.LineNumber;
            }
        }

        public virtual int LinePosition
        {
            get
            {
                return (readerAsIXmlLineInfo == null) ? 0 : readerAsIXmlLineInfo.LinePosition;
            }
        }

//        //
//        //  Internal methods
//        //
//#if !SILVERLIGHT
//        internal override IDtdInfo DtdInfo
//        {
//            get
//            {
//                return reader.DtdInfo;
//            }
//        }
//#endif

    }
}

