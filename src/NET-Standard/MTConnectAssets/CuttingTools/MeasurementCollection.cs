// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectAssets.CuttingTools
{
    public class MeasurementCollection
    {
        [XmlElement("CuttingDiameter")]
        public Measurements.CuttingDiameter CuttingDiameter { get; set; }

        [XmlElement("FunctionalLength")]
        public Measurements.FunctionalLength FunctionalLength { get; set; }
    }
}
