using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Serialization
{
    public class MTConnectStreamsSerializer : XmlSerializer
    {
        public MTConnectStreamsSerializer(Type type)
        {

        }

        protected override object Deserialize(XmlSerializationReader reader)
        {


            return new MTConnect.MTConnectStreams.Document();
        }

    }

    //public class Reader : XmlSerializationReader
    //{
    //    public Reader()
    //    {
    //        base.
    //    }
    //}

}
