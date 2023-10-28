// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Network details of a Component.
    /// </summary>
    public class NetworkDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "NETWORK";
        public const string NameId = "";
             
        public new const string DescriptionText = "Network details of a Component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16;       


        public enum SubTypes
        {
            /// <summary>
            /// IPV4 network address of the component.
            /// </summary>
            I_PV4_ADDRESS,
            
            /// <summary>
            /// IPV6 network address of the component.
            /// </summary>
            I_PV6_ADDRESS,
            
            /// <summary>
            /// Gateway for the component network.
            /// </summary>
            GATEWAY,
            
            /// <summary>
            /// SubNet mask for the component network.
            /// </summary>
            SUBNET_MASK,
            
            /// <summary>
            /// Layer2 Virtual Local Network (VLAN) ID for the component network.
            /// </summary>
            V_LAN_ID,
            
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
                case SubTypes.I_PV4_ADDRESS: return "IPV4 network address of the component.";
                case SubTypes.I_PV6_ADDRESS: return "IPV6 network address of the component.";
                case SubTypes.GATEWAY: return "Gateway for the component network.";
                case SubTypes.SUBNET_MASK: return "SubNet mask for the component network.";
                case SubTypes.V_LAN_ID: return "Layer2 Virtual Local Network (VLAN) ID for the component network.";
                case SubTypes.MAC_ADDRESS: return "Media Access Control Address. The unique physical address of the network hardware.";
                case SubTypes.WIRELESS: return "Identifies whether the connection type is wireless.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.I_PV4_ADDRESS: return "I_PV4_ADDRESS";
                case SubTypes.I_PV6_ADDRESS: return "I_PV6_ADDRESS";
                case SubTypes.GATEWAY: return "GATEWAY";
                case SubTypes.SUBNET_MASK: return "SUBNET_MASK";
                case SubTypes.V_LAN_ID: return "V_LAN_ID";
                case SubTypes.MAC_ADDRESS: return "MAC_ADDRESS";
                case SubTypes.WIRELESS: return "WIRELESS";
            }

            return null;
        }

    }
}