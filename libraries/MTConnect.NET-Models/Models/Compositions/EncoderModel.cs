// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Conditions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism to measure position.
    /// </summary>
    public class EncoderModel : CompositionModel, IEncoderModel
    {
        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        public Observations.IConditionObservation SystemCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.SystemCondition.NameId);
            set => AddCondition(new SystemCondition(Id), value);
        }

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        public Observations.IConditionObservation HardwareCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.HardwareCondition.NameId);
            set => AddCondition(new HardwareCondition(Id), value);
        }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        public Observations.IConditionObservation CommunicationsCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.CommunicationsCondition.NameId);
            set => AddCondition(new CommunicationsCondition(Id), value);
        }


        public EncoderModel() 
        {
            Type = EncoderComposition.TypeId;
        }

        public EncoderModel(string compositionId)
        {
            Id = compositionId;
            Type = EncoderComposition.TypeId;
        }
    }
}