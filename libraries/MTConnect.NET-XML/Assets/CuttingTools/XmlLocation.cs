// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for a cutting tool's <c>Location</c> within
    /// a tool-handling system. Mirrors the on-the-wire element and converts to
    /// the strongly-typed <see cref="Location"/> model.
    /// </summary>
    public class XmlLocation
    {
        /// <summary>
        /// The location's position value, carried as the element text.
        /// </summary>
        [XmlAttribute("value")]
        public string Value { get; set; }

        /// <summary>
        /// The kind of location, such as <c>POT</c> or <c>SPINDLE</c>, as the
        /// raw attribute text.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The number of pots beyond this one, in the positive direction, the
        /// tool occupies, as the raw attribute text.
        /// </summary>
        [XmlAttribute("positiveOverlap")]
        public string PositiveOverlap { get; set; }

        /// <summary>
        /// The number of pots beyond this one, in the negative direction, the
        /// tool occupies, as the raw attribute text.
        /// </summary>
        [XmlAttribute("negativeOverlap")]
        public string NegativeOverlap { get; set; }

        /// <summary>
        /// The identifier of the turret holding the tool.
        /// </summary>
        [XmlAttribute("turret")]
        public string Turret { get; set; }

        /// <summary>
        /// The identifier of the tool magazine holding the tool.
        /// </summary>
        [XmlAttribute("toolMagazine")]
        public string ToolMagazine { get; set; }

        /// <summary>
        /// The identifier of the tool bar holding the tool.
        /// </summary>
        [XmlAttribute("toolBar")]
        public string ToolBar { get; set; }

        /// <summary>
        /// The identifier of the tool rack holding the tool.
        /// </summary>
        [XmlAttribute("toolRack")]
        public string ToolRack { get; set; }

        /// <summary>
        /// The identifier of the automatic tool changer handling the tool.
        /// </summary>
        [XmlAttribute("automaticToolChanger")]
        public string AutomaticToolChanger { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Location"/>, parsing the overlap values and the location
        /// type.
        /// </summary>
        public ILocation ToLocation()
        {
            var location = new Location();
            location.Value = Value;
            location.Type = Type.ConvertEnum<LocationType>();
            if (PositiveOverlap != null) location.PositiveOverlap = PositiveOverlap.ToInt();
            if (NegativeOverlap != null) location.NegativeOverlap = NegativeOverlap.ToInt();
            location.Turret = Turret;
            location.ToolMagazine = ToolMagazine;
            location.ToolBar = ToolBar;
            location.ToolRack = ToolRack;
            location.AutomaticToolChanger = AutomaticToolChanger;
            return location;
        }

        /// <summary>
        /// Writes the given <see cref="ILocation"/> to <paramref name="writer"/>
        /// as a <c>Location</c> element, omitting the optional tool-handling
        /// identifiers that are not set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ILocation location)
        {
            if (location != null)
            {
                writer.WriteStartElement("Location");
                writer.WriteAttributeString("type", location.Type.ToString());
                if (location.PositiveOverlap != null) writer.WriteAttributeString("positiveOverlap", location.PositiveOverlap.ToString());
                if (location.NegativeOverlap != null) writer.WriteAttributeString("negativeOverlap", location.NegativeOverlap.ToString());
                if (!string.IsNullOrEmpty(location.Turret)) writer.WriteAttributeString("turret", location.Turret);
                if (!string.IsNullOrEmpty(location.ToolMagazine)) writer.WriteAttributeString("toolMagazine", location.ToolMagazine);
                if (!string.IsNullOrEmpty(location.ToolBar)) writer.WriteAttributeString("toolBar", location.ToolBar);
                if (!string.IsNullOrEmpty(location.ToolRack)) writer.WriteAttributeString("toolRack", location.ToolRack);
                if (!string.IsNullOrEmpty(location.AutomaticToolChanger)) writer.WriteAttributeString("automaticToolChanger", location.AutomaticToolChanger);
                writer.WriteString(location.Value);
                writer.WriteEndElement();
            }
        }
    }
}