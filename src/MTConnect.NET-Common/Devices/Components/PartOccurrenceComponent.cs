// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// PartOccurrence is a Component that organizes information about a specific part as it exists at a specific place and time, 
    /// such as a specific instance of a bracket at a specific timestamp. Part is defined as a discrete item that has both defined
    /// and measurable physical characteristics including mass, material and features and is created by applying one or more manufacturing process steps to a workpiece.
    /// </summary>
    public class PartOccurrenceComponent : Component 
    {
        public const string TypeId = "PartOccurrence";
        public const string NameId = "partocc";
        public new const string DescriptionText = "PartOccurrence is a Component that organizes information about a specific part as it exists at a specific place and time, such as a specific instance of a bracket at a specific timestamp. Part is defined as a discrete item that has both defined and measurable physical characteristics including mass, material and features and is created by applying one or more manufacturing process steps to a workpiece.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public PartOccurrenceComponent()  { Type = TypeId; }
    }
}