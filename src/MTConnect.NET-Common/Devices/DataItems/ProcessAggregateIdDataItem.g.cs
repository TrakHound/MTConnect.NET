// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a group of related occurrences, such as a process step in a process plan.
    /// </summary>
    public class ProcessAggregateIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_AGGREGATE_ID";
        public const string NameId = "";
             
        public new const string DescriptionText = "Identifier given to link the individual occurrence to a group of related occurrences, such as a process step in a process plan.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public enum SubTypes
        {
            /// <summary>
            /// Identifier of the authorization of the process occurrence. Synonyms include 'job id', 'work order'.
            /// </summary>
            ORDER_NUMBER,
            
            /// <summary>
            /// Identifier of the step in the process plan that this occurrence corresponds to. Synonyms include 'operation id'.
            /// </summary>
            PROCESS_STEP,
            
            /// <summary>
            /// Identifier of the process plan that this occurrence belongs to. Synonyms include 'routing id', 'job id'.
            /// </summary>
            PROCESS_PLAN
        }


        public ProcessAggregateIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ProcessAggregateIdDataItem(
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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ORDER_NUMBER: return "Identifier of the authorization of the process occurrence. Synonyms include 'job id', 'work order'.";
                case SubTypes.PROCESS_STEP: return "Identifier of the step in the process plan that this occurrence corresponds to. Synonyms include 'operation id'.";
                case SubTypes.PROCESS_PLAN: return "Identifier of the process plan that this occurrence belongs to. Synonyms include 'routing id', 'job id'.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ORDER_NUMBER: return "ORDER_NUMBER";
                case SubTypes.PROCESS_STEP: return "PROCESS_STEP";
                case SubTypes.PROCESS_PLAN: return "PROCESS_PLAN";
            }

            return null;
        }

    }
}