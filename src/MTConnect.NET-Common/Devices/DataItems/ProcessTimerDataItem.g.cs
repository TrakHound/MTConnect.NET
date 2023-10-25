// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.
    /// </summary>
    public class ProcessTimerDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PROCESS_TIMER";
        public const string NameId = "";
        public const string DefaultUnits = Devices.Units.SECOND;     
        public new const string DescriptionText = "Amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Time from the beginning of production of a part or product on a piece of equipment until the time that production is complete for that part or product onthat piece of equipment.This includes the time that the piece of equipment is running, producing parts or products, or in the process of producing parts.
            /// </summary>
            PROCESS,
            
            /// <summary>
            /// Elapsed time of a temporary halt of action.
            /// </summary>
            DELAY
        }


        public ProcessTimerDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public ProcessTimerDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = DefaultUnits;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.PROCESS: return "Time from the beginning of production of a part or product on a piece of equipment until the time that production is complete for that part or product onthat piece of equipment.This includes the time that the piece of equipment is running, producing parts or products, or in the process of producing parts.";
                case SubTypes.DELAY: return "Elapsed time of a temporary halt of action.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PROCESS: return "PROCESS";
                case SubTypes.DELAY: return "DELAY";
            }

            return null;
        }

    }
}