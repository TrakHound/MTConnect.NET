// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218144_231993_1515

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// **DEPRECATED:** Replaced with `CONDITION` category data items in Version 1.1.0.
    /// </summary>
    public class AlarmDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "ALARM";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "alarm";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "**DEPRECATED:** Replaced with `CONDITION` category data items in Version 1.1.0.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The maximum MTConnect Version that this DataItem is valid for; set when the type has been deprecated.
        /// </summary>
        public override System.Version MaximumVersion => MTConnectVersions.Version11;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version10;


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public AlarmDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        /// <summary>
        /// Initializes a new instance scoped to the given device.
        /// </summary>
        /// <param name="deviceId">The Id of the device this DataItem belongs to.</param>
        public AlarmDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            
            
        }
    }
}