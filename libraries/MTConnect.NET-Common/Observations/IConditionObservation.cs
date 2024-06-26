// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Observations
{
    /// <summary>
    /// Observation that provides the condition of a piece of equipment or a Component.
    /// </summary>
    public interface IConditionObservation : IObservation
    {
        /// <summary>
        /// Level of the Condition (Normal, Warning, Fault, or Unavailable)
        /// </summary>
        ConditionLevel Level { get; }

        /// <summary>
        /// Identifier of an individual condition activation provided by a piece of equipment.
        /// </summary>
        string ConditionId { get; }

        /// <summary>
        /// The native code (usually an alpha-numeric value) generated by the controller of a piece of equipment providing a reference identifier for a condition state or alarm.
        /// This is the same information an operator or maintenance personnel would see as a reference code designating a specific type of Condition when viewed at the piece of equipment.Usually this reference code is used to point to a more detailed description of the Condition.
        /// </summary>
        string NativeCode { get; }

        /// <summary>
        /// If the data source assigns a severity level to a Condition, nativeSeverity is used to report that severity information to a client software application.
        /// </summary>
        string NativeSeverity { get; }

        /// <summary>
        /// Qualifies the Condition and adds context or additional clarification.
        /// This optional attribute can be used to convey information such as HIGH or LOW type Warning and Fault condition to indicate differing types of condition states
        /// </summary>
        ConditionQualifier Qualifier { get; }

        /// <summary>
        /// The type of statistical calculation specified for the DataItem defined in the Device Information Model that this Condition element represents.
        /// </summary>
        DataItemStatistic Statistic { get; }

        /// <summary>
        /// Used to describe a message published as part of a Condition Fault.
        /// </summary>
        string Message { get; }
    }
}