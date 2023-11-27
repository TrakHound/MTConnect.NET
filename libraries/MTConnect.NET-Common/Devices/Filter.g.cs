// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_8A5219C3_747D_4904_A58A_A312D13DAEE9

namespace MTConnect.Devices
{
    /// <summary>
    /// Provides a means to control when an agent records updated information for a DataItem.
    /// </summary>
    public class Filter : IFilter
    {
        public const string DescriptionText = "Provides a means to control when an agent records updated information for a DataItem.";


        /// <summary>
        /// Type of Filter.
        /// </summary>
        public MTConnect.Devices.DataItemFilterType Type { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public double Value { get; set; }
    }
}