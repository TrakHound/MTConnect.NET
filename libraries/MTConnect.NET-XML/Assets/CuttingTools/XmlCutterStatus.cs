// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for a single <c>Status</c> entry within a
    /// CuttingTool <c>CutterStatus</c> collection. Each entry records one
    /// life-cycle state of the cutter (for example <c>NEW</c>, <c>USED</c>,
    /// <c>EXPIRED</c>, or <c>BROKEN</c>).
    /// </summary>
    public class XmlCutterStatus
    {
        /// <summary>
        /// The cutter status value carried by the <c>Status</c> element.
        /// </summary>
        [XmlElement("Status")]
        public string Status { get; set; }
    }
}