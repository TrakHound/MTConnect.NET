// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a class of processes or process definition.
    /// </summary>
    public class ProcessKindIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_KIND_ID";
        public const string NameId = "";
             
        public new const string DescriptionText = "Identifier given to link the individual occurrence to a class of processes or process definition.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public enum SubTypes
        {
            /// <summary>
            /// Universally unique identifier as specified in ISO 11578 or RFC 4122.
            /// </summary>
            UUID,
            
            /// <summary>
            /// Reference to a ISO 10303 Executable.
            /// </summary>
            I_S_O_STEP_EXECUTABLE,
            
            /// <summary>
            /// Word or set of words by which a process being executed (process occurrence) by the device is known, addressed, or referred to.
            /// </summary>
            PROCESS_NAME
        }


        public ProcessKindIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ProcessKindIdDataItem(
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
                case SubTypes.UUID: return "Universally unique identifier as specified in ISO 11578 or RFC 4122.";
                case SubTypes.I_S_O_STEP_EXECUTABLE: return "Reference to a ISO 10303 Executable.";
                case SubTypes.PROCESS_NAME: return "Word or set of words by which a process being executed (process occurrence) by the device is known, addressed, or referred to.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.UUID: return "UUID";
                case SubTypes.I_S_O_STEP_EXECUTABLE: return "I_S_O_STEP_EXECUTABLE";
                case SubTypes.PROCESS_NAME: return "PROCESS_NAME";
            }

            return null;
        }

    }
}