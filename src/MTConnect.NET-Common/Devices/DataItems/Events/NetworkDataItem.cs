// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Network details of a component.
    /// </summary>
    public class NetworkDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "NETWORK";
        public const string NameId = "network";
        public new const string DescriptionText = "Network details of a component.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;

        public enum SubTypes
        {
            /// <summary>
            /// The IPV4 network address of the component.
            /// </summary>
            IPV4_ADDRESS,

            /// <summary>
            /// The IPV6 network address of the component.
            /// </summary>
            IPV6_ADDRESS,

            /// <summary>
            /// The Gateway for the component network.
            /// </summary>
            GATEWAY,

            /// <summary>
            /// The SubNet mask for the component network.
            /// </summary>
            SUBNET_MASK,

            /// <summary>
            /// The layer2 Virtual Local Network (VLAN) ID for the component network.
            /// </summary>
            VLAN_ID,

            /// <summary>
            /// Media Access Control Address. The unique physical address of the network hardware.
            /// </summary>
            MAC_ADDRESS,

            /// <summary>
            /// Identifies whether the connection type is wireless.
            /// </summary>
            WIRELESS
        }


        public NetworkDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public NetworkDataItem(
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
                case SubTypes.IPV4_ADDRESS: return "The IPV4 network address of the component.";
                case SubTypes.IPV6_ADDRESS: return "The IPV6 network address of the component.";
                case SubTypes.GATEWAY: return "The Gateway for the component network.";
                case SubTypes.SUBNET_MASK: return "The SubNet mask for the component network.";
                case SubTypes.VLAN_ID: return "The layer2 Virtual Local Network (VLAN) ID for the component network.";
                case SubTypes.MAC_ADDRESS: return "Media Access Control Address. The unique physical address of the network hardware.";
                case SubTypes.WIRELESS: return "Identifies whether the connection type is wireless.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.IPV4_ADDRESS: return "ipv4Addr";
                case SubTypes.IPV6_ADDRESS: return "ipv6Addr";
                case SubTypes.GATEWAY: return "gateway";
                case SubTypes.SUBNET_MASK: return "subnet";
                case SubTypes.VLAN_ID: return "vlanId";
                case SubTypes.MAC_ADDRESS: return "macAddr";
                case SubTypes.WIRELESS: return "wireless";
            }

            return null;
        }
    }
}