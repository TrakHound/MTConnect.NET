// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Specifications;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonProcessSpecification : JsonAbstractSpecification
    {
        [JsonPropertyName("controlLimits")]
        public JsonControlLimits ControlLimits { get; set; }

        [JsonPropertyName("specificationLimits")]
        public JsonSpecificationLimits SpecificationLimits { get; set; }

        [JsonPropertyName("alarmLimits")]
        public JsonAlarmLimits AlarmLimits { get; set; }


        public JsonProcessSpecification() { }

        public JsonProcessSpecification(IProcessSpecification specification) : base(specification)
        {
            if (specification != null)
            {
                if (specification.ControlLimits != null) ControlLimits = new JsonControlLimits(specification.ControlLimits);
                if (specification.SpecificationLimits != null) SpecificationLimits = new JsonSpecificationLimits(specification.SpecificationLimits);
                if (specification.AlarmLimits != null) AlarmLimits = new JsonAlarmLimits(specification.AlarmLimits);
            }
        }


        public override IAbstractSpecification ToSpecification()
        {
            var specification = new ProcessSpecification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateIdRef = CoordinateIdRef;
            specification.Originator = Originator.ConvertEnum<Originator>();

            if (ControlLimits != null) specification.ControlLimits = ControlLimits.ToControlLimits();
            if (SpecificationLimits != null) specification.SpecificationLimits = SpecificationLimits.ToSpecificationLimits();
            if (AlarmLimits != null) specification.AlarmLimits = AlarmLimits.ToAlarmLimits();

            return specification;
        }
    }
}