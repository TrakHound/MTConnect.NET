// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Difference between the commanded encoder/resolver position and the actual encoder/resolver position at any specified point in time during a motion.
    /// </summary>
    public class FollowingErrorLinearDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "FOLLOWING_ERROR_LINEAR";
        public const string NameId = "followingErrorLinear";
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Difference between the commanded encoder/resolver position and the actual encoder/resolver position at any specified point in time during a motion.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL
        }


        public FollowingErrorLinearDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public FollowingErrorLinearDataItem(
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
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
            }

            return null;
        }

    }
}