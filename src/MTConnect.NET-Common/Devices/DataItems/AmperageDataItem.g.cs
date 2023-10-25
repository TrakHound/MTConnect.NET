// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Strength of electrical current.**DEPRECATED** in *Version 1.6*. Replaced by `AMPERAGE_AC` and `AMPERAGE_DC`.
    /// </summary>
    public class AmperageDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "AMPERAGE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Strength of electrical current.**DEPRECATED** in *Version 1.6*. Replaced by `AMPERAGE_AC` and `AMPERAGE_DC`.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version16;
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public enum SubTypes
        {
            /// <summary>
            /// Measurement of alternating voltage or current. If not specified further in statistic, defaults to RMS voltage. **DEPRECATED** in *Version 1.6*.
            /// </summary>
            ALTERNATING,
            
            /// <summary>
            /// Measurement of DC current or voltage.**DEPRECATED** in *Version 1.6*.
            /// </summary>
            DIRECT,
            
            /// <summary>
            /// Measured or reported value of an observation.**DEPRECATED** in *Version 1.6*.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Goal of the operation or process.**DEPRECATED** in *Version 1.6*.
            /// </summary>
            TARGET
        }


        public AmperageDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AmperageDataItem(
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
                case SubTypes.ALTERNATING: return "Measurement of alternating voltage or current. If not specified further in statistic, defaults to RMS voltage. **DEPRECATED** in *Version 1.6*.";
                case SubTypes.DIRECT: return "Measurement of DC current or voltage.**DEPRECATED** in *Version 1.6*.";
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.**DEPRECATED** in *Version 1.6*.";
                case SubTypes.TARGET: return "Goal of the operation or process.**DEPRECATED** in *Version 1.6*.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ALTERNATING: return "ALTERNATING";
                case SubTypes.DIRECT: return "DIRECT";
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.TARGET: return "TARGET";
            }

            return null;
        }

    }
}