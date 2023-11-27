// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605277188545_673734_476

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Reference to the target Device for this File.
    /// </summary>
    public class Destination : IDestination
    {
        public const string DescriptionText = "Reference to the target Device for this File.";


        /// <summary>
        /// `uuid` of the target device or application.
        /// </summary>
        public string DeviceUuid { get; set; }
    }
}