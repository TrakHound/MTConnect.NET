// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlCutterStatus
    {
        [XmlElement("Status")]
        public string Status { get; set; }
    }
}