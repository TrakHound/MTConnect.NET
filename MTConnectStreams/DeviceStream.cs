// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.MTConnectStreams
{
    /// <summary>
    /// An XML container element provided in each MTConnectStreams XML document provided by a MTConnect Agent in response to a Sample or Current request.
    /// </summary>
    [XmlRoot("DeviceStream")]
    public class DeviceStream
    {

        #region "Required"

        /// <summary>
        /// Name attribute of the Devices defined in the Device Information Model for which data is provided.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Uuid attribute of the Device defined in the Device Information Model for which data is provided
        /// </summary>
        [XmlAttribute("uuid")]
        public string Uuid { get; set; }

        #endregion

        #region "Sub-Elements"

        /// <summary>
        /// An XML container type element that may be provided in a MTConnectStreams XML document returned from a MTConnect Agent in response to a Current or Sample request that organizes data for a specific Structural Element of a device.
        /// </summary>
        [XmlElement("ComponentStream")]
        public List<ComponentStream> ComponentStreams { get; set; }

        [XmlIgnore]
        public List<DataItem> DataItems
        {
            get
            {
                var l = new List<DataItem>();

                if (ComponentStreams != null)
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        l.AddRange(componentStream.DataItems);
                    }
                }

                return l;
            }
        }

        #endregion
        
    }
}
