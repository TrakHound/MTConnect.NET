// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlLocation
    {
        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("positiveOverlap")]
        public string PositiveOverlap { get; set; }

        [XmlAttribute("negativeOverlap")]
        public string NegativeOverlap { get; set; }

        [XmlAttribute("turret")]
        public string Turret { get; set; }

        [XmlAttribute("toolMagazine")]
        public string ToolMagazine { get; set; }

        [XmlAttribute("toolBar")]
        public string ToolBar { get; set; }

        [XmlAttribute("toolRack")]
        public string ToolRack { get; set; }

        [XmlAttribute("automaticToolChanger")]
        public string AutomaticToolChanger { get; set; }


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