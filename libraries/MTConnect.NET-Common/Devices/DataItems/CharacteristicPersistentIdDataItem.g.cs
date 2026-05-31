// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1678250721451_170871_18540

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// UUID of the characteristic.
    /// </summary>
    public class CharacteristicPersistentIdDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "CHARACTERISTIC_PERSISTENT_ID";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "characteristicPersistentId";

        /// <summary>
        /// The default <c>representation</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "UUID of the characteristic.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version22;


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public CharacteristicPersistentIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        /// <summary>
        /// Initializes a new instance scoped to the given device.
        /// </summary>
        /// <param name="deviceId">The Id of the device this DataItem belongs to.</param>
        public CharacteristicPersistentIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;
            
        }
    }
}