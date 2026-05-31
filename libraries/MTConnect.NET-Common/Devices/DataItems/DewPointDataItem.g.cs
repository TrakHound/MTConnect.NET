// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1659032383380_200012_108

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Temperature at which moisture begins to condense, corresponding to saturation for a given absolute humidity.
    /// </summary>
    public class DewPointDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "DEW_POINT";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "dewPoint";

        /// <summary>
        /// The default <c>units</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const string DefaultUnits = Devices.Units.CELSIUS;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Temperature at which moisture begins to condense, corresponding to saturation for a given absolute humidity.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version21;


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public DewPointDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        /// <summary>
        /// Initializes a new instance scoped to the given device.
        /// </summary>
        /// <param name="deviceId">The Id of the device this DataItem belongs to.</param>
        public DewPointDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            
            Units = DefaultUnits;
        }
    }
}