// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = uml-drr-1

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// A data item with rules.
    /// </summary>
    public class RuledDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "RULED_DI";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "ruledDi";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "A data item with rules.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The OCL constraint bodies attached to this DataItem in the
        /// source SysML model, preserved verbatim so downstream consumers
        /// can inspect the spec's raw validation rules at runtime.
        /// </summary>
        public new static readonly string[] Rules = new[]
        {
            "self.value >= 0"
        };


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public RuledDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        /// <summary>
        /// Initializes a new instance scoped to the given device.
        /// </summary>
        /// <param name="deviceId">The Id of the device this DataItem belongs to.</param>
        public RuledDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            
            
        }
    }
}
