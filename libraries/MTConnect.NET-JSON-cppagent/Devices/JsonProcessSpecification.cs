// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a
    /// <c>ProcessSpecification</c> in the cppagent-compatible shape,
    /// extending the shared <see cref="JsonAbstractSpecification"/>
    /// envelope with separate sub-objects for control, specification,
    /// and alarm limits. Converts to and from the strongly-typed
    /// <see cref="ProcessSpecification"/> model.
    /// </summary>
    public class JsonProcessSpecification : JsonAbstractSpecification
    {
        /// <summary>
        /// Control limits that the process should be operated within.
        /// </summary>
        [JsonPropertyName("controlLimits")]
        public JsonControlLimits ControlLimits { get; set; }

        /// <summary>
        /// Specification limits the process must remain inside to be
        /// conforming.
        /// </summary>
        [JsonPropertyName("specificationLimits")]
        public JsonSpecificationLimits SpecificationLimits { get; set; }

        /// <summary>
        /// Alarm limits whose breach should raise an alarm condition.
        /// </summary>
        [JsonPropertyName("alarmLimits")]
        public JsonAlarmLimits AlarmLimits { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonProcessSpecification() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IProcessSpecification"/>, suppressing any limit
        /// sub-object when the source has none.
        /// </summary>
        public JsonProcessSpecification(IProcessSpecification specification) : base(specification)
        {
            if (specification != null)
            {
                if (specification.ControlLimits != null) ControlLimits = new JsonControlLimits(specification.ControlLimits);
                if (specification.SpecificationLimits != null) SpecificationLimits = new JsonSpecificationLimits(specification.SpecificationLimits);
                if (specification.AlarmLimits != null) AlarmLimits = new JsonAlarmLimits(specification.AlarmLimits);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ProcessSpecification"/>, applying the limit
        /// sub-objects in addition to the shared attributes parsed from
        /// the base.
        /// </summary>
        public override ISpecification ToSpecification()
        {
            var specification = new ProcessSpecification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateSystemIdRef = CoordinateSystemIdRef;
            specification.Originator = Originator.ConvertEnum<Originator>();

            if (ControlLimits != null) specification.ControlLimits = ControlLimits.ToControlLimits();
            if (SpecificationLimits != null) specification.SpecificationLimits = SpecificationLimits.ToSpecificationLimits();
            if (AlarmLimits != null) specification.AlarmLimits = AlarmLimits.ToAlarmLimits();

            return specification;
        }
    }
}