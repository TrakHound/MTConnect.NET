// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580315898400_607214_47155

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Design characteristics for a piece of equipment.
    /// </summary>
    public class Specification : ISpecification
    {
        public const string DescriptionText = "Design characteristics for a piece of equipment.";


        /// <summary>
        /// Reference to the id attribute of the Composition associated with this element.
        /// </summary>
        public string CompositionIdRef { get; set; }
        
        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }
        
        /// <summary>
        /// Reference to the id attribute of the DataItem associated with this element.
        /// </summary>
        public string DataItemIdRef { get; set; }
        
        /// <summary>
        /// Unique identifier for this Specification.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double LowerLimit { get; set; }
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double LowerWarning { get; set; }
        
        /// <summary>
        /// Numeric upper constraint.
        /// </summary>
        public double Maximum { get; set; }
        
        /// <summary>
        /// Numeric lower constraint.
        /// </summary>
        public double Minimum { get; set; }
        
        /// <summary>
        /// Name provides additional meaning and differentiates between Specification elements.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        public double Nominal { get; set; }
        
        /// <summary>
        /// Reference to the creator of the Specification.
        /// </summary>
        public MTConnect.Devices.Configurations.Originator Originator { get; set; }
        
        /// <summary>
        /// Same as DataItem DataItem::subType. See DataItem.
        /// </summary>
        public DataItemSubType SubType { get; set; }
        
        /// <summary>
        /// Same as DataItem type. See DataItem Types.
        /// </summary>
        public DataItemType Type { get; set; }
        
        /// <summary>
        /// Same as DataItem DataItem::units. See DataItem.
        /// </summary>
        public string Units { get; set; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double UpperLimit { get; set; }
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double UpperWarning { get; set; }
    }
}