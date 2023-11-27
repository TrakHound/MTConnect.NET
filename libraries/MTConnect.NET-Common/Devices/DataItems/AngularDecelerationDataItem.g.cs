// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Negative rate of change of angular velocity.
    /// </summary>
    public class AngularDecelerationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ANGULAR_DECELERATION";
        public const string NameId = "angularDeceleration";
        public const string DefaultUnits = Devices.Units.DEGREE_PER_SECOND_SQUARED;     
        public new const string DescriptionText = "Negative rate of change of angular velocity.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,
            
            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED
        }


        public AngularDecelerationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public AngularDecelerationDataItem(
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
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.";
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.PROGRAMMED: return "PROGRAMMED";
                case SubTypes.COMMANDED: return "COMMANDED";
            }

            return null;
        }

    }
}