// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to close a door.
    /// </summary>
    public class CloseDoorDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CLOSE_DOOR";
        public const string NameId = "closeDoor";
        
        public new const string DescriptionText = "Operating state of the service to close a door.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public new enum SubTypes
        {
            /// <summary>
            /// Operating state of the request to close a door.
            /// </summary>
            REQUEST,
            
            /// <summary>
            /// Operating state of the response to a request to close a door.
            /// </summary>
            RESPONSE
        }


        public CloseDoorDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public CloseDoorDataItem(
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

        public new static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.REQUEST: return "Operating state of the request to close a door.";
                case SubTypes.RESPONSE: return "Operating state of the response to a request to close a door.";
            }

            return null;
        }

        public new static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.REQUEST: return "REQUEST";
                case SubTypes.RESPONSE: return "RESPONSE";
            }

            return null;
        }

    }
}
