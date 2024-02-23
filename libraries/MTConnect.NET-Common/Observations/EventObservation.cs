// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class EventObservation : Observation, IEventObservation
    {
        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        public ResetTriggered ResetTriggered
        {
            get => GetValue(ValueKeys.ResetTriggered).ConvertEnum<ResetTriggered>();
            set
            {
                if (value != ResetTriggered.NOT_SPECIFIED)
                {
                    AddValue(new ObservationValue(ValueKeys.ResetTriggered, value));
                }
            }
        }


        public EventObservation()
        {
            _category = DataItemCategory.EVENT;
        }


        public new static EventObservation Create(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                var observation = Create(dataItem.Type, dataItem.Representation);
                observation._dataItem = dataItem;
                observation._dataItemId = dataItem.Id;
                observation._representation = dataItem.Representation;
                observation._type = dataItem.Type;
                observation._subType = dataItem.SubType;
                observation._name = dataItem.Name;
                observation._compositionId = dataItem.CompositionId;
                return observation;
            }

            return null;
        }

        public static EventObservation Create(IObservation observation)
        {
            if (observation != null)
            {
                var result = Create(observation.Type, observation.Representation);
                result._dataItem = observation.DataItem;
                result._dataItemId = observation.DataItemId;
                result._representation = observation.Representation;
                result._type = observation.Type;
                result._subType = observation.SubType;
                result._name = observation.Name;
                result._compositionId = observation.CompositionId;
                result._instanceId = observation.InstanceId;
                result._sequence = observation.Sequence;
                result._timestamp = observation.Timestamp;
                result.AddValues(observation.Values);
                return result;
            }

            return null;
        }


        public static EventObservation Create(string type, DataItemRepresentation representation)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    var key = string.Intern(type + ":" + (int)representation);

                    // Lookup Type ID (Type as PascalCase)
                    _typeIds.TryGetValue(key, out var typeId);
                    if (typeId == null)
                    {
                        typeId = $"{type.ToPascalCase()}{representation.ToString().ToPascalCase()}";
                        _typeIds.Add(key, typeId);
                    }

                    if (_types.TryGetValue(key, out Type t))
                    {
                        var constructor = t.GetConstructor(System.Type.EmptyTypes);
                        if (constructor != null)
                        {
                            try
                            {
                                switch (representation)
                                {
                                    case DataItemRepresentation.VALUE: return (EventValueObservation)Activator.CreateInstance(t);
                                    case DataItemRepresentation.DATA_SET: return (EventDataSetObservation)Activator.CreateInstance(t);
                                    case DataItemRepresentation.TABLE: return (EventTableObservation)Activator.CreateInstance(t);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }

            switch (representation)
            {
                case DataItemRepresentation.DATA_SET: return new EventDataSetObservation();
                case DataItemRepresentation.TABLE: return new EventTableObservation();
                default: return new EventValueObservation();
            }
        }
    }
}