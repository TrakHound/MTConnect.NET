// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Design characteristics for a piece of equipment.
    /// </summary>
    public interface ISpecification
    {
        /// <summary>
        /// Id associated with this entity.
        /// </summary>
        string CompositionIdRef { get; }
        
        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        string CoordinateSystemIdRef { get; }
        
        /// <summary>
        /// Id associated with this entity.
        /// </summary>
        string DataItemIdRef { get; }
        
        /// <summary>
        /// Unique identifier for this Specification.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        double? LowerLimit { get; }
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        double? LowerWarning { get; }
        
        /// <summary>
        /// Numeric upper constraint.
        /// </summary>
        double? Maximum { get; }
        
        /// <summary>
        /// Numeric lower constraint.
        /// </summary>
        double? Minimum { get; }
        
        /// <summary>
        /// Name provides additional meaning and differentiates between Specification entities.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        double? Nominal { get; }
        
        /// <summary>
        /// Reference to the creator of the Specification.
        /// </summary>
        MTConnect.Devices.Configurations.Originator Originator { get; }
        
        /// <summary>
        /// SubType. See DataItem.
        /// </summary>
        string SubType { get; }
        
        /// <summary>
        /// Type. See DataItem Types.
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// Units. See DataItem.
        /// </summary>
        string Units { get; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        double? UpperLimit { get; }
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        double? UpperWarning { get; }
    }
}