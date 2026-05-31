// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218441_671480_2166

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Serial number associated with a Component, Asset, or Device.
    /// </summary>
    public class SerialNumberDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "SERIAL_NUMBER";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "serialNumber";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Serial number associated with a Component, Asset, or Device.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public SerialNumberDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        /// <summary>
        /// Initializes a new instance scoped to the given device.
        /// </summary>
        /// <param name="deviceId">The Id of the device this DataItem belongs to.</param>
        public SerialNumberDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            
            
        }
    }
}