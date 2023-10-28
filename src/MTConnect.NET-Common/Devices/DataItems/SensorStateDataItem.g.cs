// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Detection result of a sensor.
    /// </summary>
    public class SensorStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SENSOR_STATE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Detection result of a sensor.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public enum SubTypes
        {
            /// <summary>
            /// EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::BINARY
            /// </summary>
            BINARY,
            
            /// <summary>
            /// EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::BOOLEAN
            /// </summary>
            BOOLEAN,
            
            /// <summary>
            /// EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::ENUMERATED
            /// </summary>
            ENUMERATED,
            
            /// <summary>
            /// EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::DETECT
            /// </summary>
            DETECT
        }


        public SensorStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public SensorStateDataItem(
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
                case SubTypes.BINARY: return "EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::BINARY";
                case SubTypes.BOOLEAN: return "EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::BOOLEAN";
                case SubTypes.ENUMERATED: return "EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::ENUMERATED";
                case SubTypes.DETECT: return "EventEnum:SENSOR_STATE where the state is DataItemSubTypeEnum::DETECT";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.BINARY: return "BINARY";
                case SubTypes.BOOLEAN: return "BOOLEAN";
                case SubTypes.ENUMERATED: return "ENUMERATED";
                case SubTypes.DETECT: return "DETECT";
            }

            return null;
        }

    }
}