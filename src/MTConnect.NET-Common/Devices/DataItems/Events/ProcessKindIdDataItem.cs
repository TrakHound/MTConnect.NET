// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a class of processes or process definition.
    /// </summary>
    public class ProcessKindIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_KIND_ID";
        public const string NameId = "processKindId";
        public new const string DescriptionText = "Identifier given to link the individual occurrence to a class of processes or process definition.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;

        public enum SubTypes
        {
            /// <summary>
            /// A reference to a ISO 10303 Executable.
            /// </summary>
            ISO_STEP_EXECUTABLE,

            /// <summary>
            /// A word or set of words by which a process being executed (process occurrence) by the device is known, addressed, or referred to.
            /// </summary>
            PROCESS_NAME,

            /// <summary>
            /// The globally unique identifier as specified in ISO 11578 or RFC 4122.
            /// </summary>
            UUID
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
                case SubTypes.ISO_STEP_EXECUTABLE: return "A reference to a ISO 10303 Executable.";
                case SubTypes.PROCESS_NAME: return "A word or set of words by which a process being executed (process occurrence) by the device is known, addressed, or referred to.";
                case SubTypes.UUID: return "The globally unique identifier as specified in ISO 11578 or RFC 4122.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ISO_STEP_EXECUTABLE: return "isoStepExe";
                case SubTypes.PROCESS_NAME: return "processName";
                case SubTypes.UUID: return "uuid";
            }

            return null;
        }
    }
}