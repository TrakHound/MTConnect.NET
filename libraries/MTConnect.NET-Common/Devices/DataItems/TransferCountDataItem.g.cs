// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1622197600319_862998_718

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Accumulation of the number of times an operation has attempted to, or is planned to attempt to, transfer materials, parts, or other items from one location to another.
    /// </summary>
    public class TransferCountDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TRANSFER_COUNT";
        public const string NameId = "transferCount";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Accumulation of the number of times an operation has attempted to, or is planned to attempt to, transfer materials, parts, or other items from one location to another.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version18;       


        public enum SubTypes
        {
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that conform to specification or expectation.
            /// </summary>
            GOOD,
            
            /// <summary>
            /// Accumulation of actions or activities that were attempted, but terminated before they could be completed.
            /// </summary>
            ABORTED,
            
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.
            /// </summary>
            FAILED,
            
            /// <summary>
            /// Accumulation of all actions, items, or activities being counted independent of the outcome.
            /// </summary>
            ALL,
            
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.
            /// </summary>
            BAD,
            
            /// <summary>
            /// Accumulation of actions, items, or activities yet to be counted.
            /// </summary>
            REMAINING,
            
            /// <summary>
            /// Accumulation of actions, items, or activities that have been completed, independent of the outcome.
            /// </summary>
            COMPLETE,
            
            /// <summary>
            /// Goal of the operation or process.
            /// </summary>
            TARGET
        }


        public TransferCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public TransferCountDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.GOOD: return "Accumulation of actions, items, or activities being counted that conform to specification or expectation.";
                case SubTypes.ABORTED: return "Accumulation of actions or activities that were attempted, but terminated before they could be completed.";
                case SubTypes.FAILED: return "Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.";
                case SubTypes.ALL: return "Accumulation of all actions, items, or activities being counted independent of the outcome.";
                case SubTypes.BAD: return "Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.";
                case SubTypes.REMAINING: return "Accumulation of actions, items, or activities yet to be counted.";
                case SubTypes.COMPLETE: return "Accumulation of actions, items, or activities that have been completed, independent of the outcome.";
                case SubTypes.TARGET: return "Goal of the operation or process.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.GOOD: return "GOOD";
                case SubTypes.ABORTED: return "ABORTED";
                case SubTypes.FAILED: return "FAILED";
                case SubTypes.ALL: return "ALL";
                case SubTypes.BAD: return "BAD";
                case SubTypes.REMAINING: return "REMAINING";
                case SubTypes.COMPLETE: return "COMPLETE";
                case SubTypes.TARGET: return "TARGET";
            }

            return null;
        }

    }
}