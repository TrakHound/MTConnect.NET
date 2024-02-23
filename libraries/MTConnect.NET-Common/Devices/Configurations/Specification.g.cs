// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580315898400_607214_47155

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Design characteristics for a piece of equipment.
    /// </summary>
    public class Specification : ISpecification
    {
        public const string DescriptionText = "Design characteristics for a piece of equipment.";


        /// <summary>
        /// Id associated with this entity.
        /// </summary>
        public string CompositionIdRef { get; set; }
        
        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }
        
        /// <summary>
        /// Id associated with this entity.
        /// </summary>
        public string DataItemIdRef { get; set; }
        
        /// <summary>
        /// Unique identifier for this Specification.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double? LowerLimit { get; set; }
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? LowerWarning { get; set; }
        
        /// <summary>
        /// Numeric upper constraint.
        /// </summary>
        public double? Maximum { get; set; }
        
        /// <summary>
        /// Numeric lower constraint.
        /// </summary>
        public double? Minimum { get; set; }
        
        /// <summary>
        /// Name provides additional meaning and differentiates between Specification entities.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        public double? Nominal { get; set; }
        
        /// <summary>
        /// Reference to the creator of the Specification.
        /// </summary>
        public MTConnect.Devices.Configurations.Originator Originator { get; set; }
        
        /// <summary>
        /// SubType. See DataItem.
        /// </summary>
        public string SubType { get; set; }
        
        /// <summary>
        /// Type. See DataItem Types.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Units. See DataItem.
        /// </summary>
        public string Units { get; set; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double? UpperLimit { get; set; }
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? UpperWarning { get; set; }
    }
}