// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_91b028d_1587752011247_360664_3696

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Network details of a Component.
    /// </summary>
    public class NetworkDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "NETWORK";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "network";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Network details of a Component.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
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


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public NetworkDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
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

        /// <summary>
        /// The MTConnect Standard description of this DataItem's current <c>subType</c>.
        /// </summary>
        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        /// <summary>
        /// Returns the MTConnect Standard description for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
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

        /// <summary>
        /// Returns the string identifier for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
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