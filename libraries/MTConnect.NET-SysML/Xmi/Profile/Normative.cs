﻿using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.Profile
{
    /// <summary>
    /// <c>&lt;Profile:normative /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.ProfileStructure.NORMATIVE, Namespace = XmiHelper.ProfileNamespace)]
    public class Normative : ProfileElement
    {
        /// <summary>
        /// <c>base_Element</c> attribute
        /// </summary>
        /// <remarks>Foreign key to the <see cref="XmiElement.Id"/> of the object this applies to.</remarks>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.baseElement, Namespace = "")]
        public string? BaseElement { get; set; }

        /// <summary>
        /// <c>version</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.introduced, Namespace = "")]
        public string? Introduced { get; set; }

        ///// <summary>
        ///// <c>version</c> attribute
        ///// </summary>
        //[XmlAttribute(AttributeName = XmiHelper.XmiStructure.version, Namespace = "")]
        //public string? Version { get; set; }
    }
}