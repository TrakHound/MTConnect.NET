// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218420_715490_2100

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Name of the logic or motion program being executed by the Controller component.
    /// </summary>
    public class ProgramDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PROGRAM";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "program";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Name of the logic or motion program being executed by the Controller component.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version10;


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
            OPERATION,
            
            /// <summary>
            /// Identity of the logic or motion program currently executing.
            /// </summary>
            ACTIVE,
            
            /// <summary>
            /// Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.
            /// </summary>
            MAIN,
            
            /// <summary>
            /// Identity of a control program that is used to specify the order of execution of other programs.
            /// </summary>
            SCHEDULE
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public ProgramDataItem()
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
        public ProgramDataItem(
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
                case SubTypes.ACTIVE: return "Identity of the logic or motion program currently executing.";
                case SubTypes.MAIN: return "Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.";
                case SubTypes.SCHEDULE: return "Identity of a control program that is used to specify the order of execution of other programs.";
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
                case SubTypes.ACTIVE: return "ACTIVE";
                case SubTypes.MAIN: return "MAIN";
                case SubTypes.SCHEDULE: return "SCHEDULE";
            }

            return null;
        }

    }
}