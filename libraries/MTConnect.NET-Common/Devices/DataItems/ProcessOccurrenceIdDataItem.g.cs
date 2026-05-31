// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605549692001_598671_1420

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of a process being executed by the device.
    /// </summary>
    public class ProcessOccurrenceIdDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PROCESS_OCCURRENCE_ID";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "processOccurrenceId";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Identifier of a process being executed by the device.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// Phase or segment of a recipe or program.
            /// </summary>
            ACTIVITY,
            
            /// <summary>
            /// Phase of a recipe process.
            /// </summary>
            SEGMENT,
            
            /// <summary>
            /// Process as part of product production; can be a subprocess of a larger process.
            /// </summary>
            RECIPE,
            
            /// <summary>
            /// Step of a discrete manufacturing process.
            /// </summary>
            OPERATION
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public ProcessOccurrenceIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
        public ProcessOccurrenceIdDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
             
            
        }

        /// <summary>
        /// The MTConnect Standard description of this DataItem's current <c>subType</c>.
        /// </summary>
        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        /// <summary>
        /// Returns the MTConnect Standard description for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTIVITY: return "Phase or segment of a recipe or program.";
                case SubTypes.SEGMENT: return "Phase of a recipe process.";
                case SubTypes.RECIPE: return "Process as part of product production; can be a subprocess of a larger process.";
                case SubTypes.OPERATION: return "Step of a discrete manufacturing process.";
            }

            return null;
        }

        /// <summary>
        /// Returns the string identifier for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTIVITY: return "ACTIVITY";
                case SubTypes.SEGMENT: return "SEGMENT";
                case SubTypes.RECIPE: return "RECIPE";
                case SubTypes.OPERATION: return "OPERATION";
            }

            return null;
        }

    }
}