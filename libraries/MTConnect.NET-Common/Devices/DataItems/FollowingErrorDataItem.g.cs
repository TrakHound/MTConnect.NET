// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Difference between actual and commanded position at any specific point in time during a motion.
    /// </summary>
    public class FollowingErrorDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "FOLLOWING_ERROR";
        public const string NameId = "followingError";
        public const string DefaultUnits = Devices.Units.COUNT;     
        public new const string DescriptionText = "Difference between actual and commanded position at any specific point in time during a motion.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public enum SubTypes
        {
            /// <summary>
            /// Measured or reported value of an observation.
            /// </summary>
            ACTUAL
        }


        public FollowingErrorDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public FollowingErrorDataItem(
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