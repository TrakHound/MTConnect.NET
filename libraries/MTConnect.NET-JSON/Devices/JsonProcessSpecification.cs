// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>ProcessSpecification</c>,
    /// extending <see cref="JsonAbstractSpecification"/> with control,
    /// specification, and alarm limit groups. Converts to and from the
    /// strongly-typed <see cref="ProcessSpecification"/> model.
    /// </summary>
    public class JsonProcessSpecification : JsonAbstractSpecification
    {
        /// <summary>
        /// The statistical control limits (sigma bands) for the process.
        /// </summary>
        [JsonPropertyName("controlLimits")]
        public JsonControlLimits ControlLimits { get; set; }

        /// <summary>
        /// The specification limits (tolerance bands) for the process.
        /// </summary>
        [JsonPropertyName("specificationLimits")]
        public JsonSpecificationLimits SpecificationLimits { get; set; }

        /// <summary>
        /// The alarm limits at which the process triggers warnings or faults.
        /// </summary>
        [JsonPropertyName("alarmLimits")]
        public JsonAlarmLimits AlarmLimits { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonProcessSpecification() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IProcessSpecification"/>, copying the common fields via
        /// the base constructor and converting each limit group.
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
        /// <see cref="ProcessSpecification"/>, parsing the originator
        /// enumeration and converting each limit group.
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