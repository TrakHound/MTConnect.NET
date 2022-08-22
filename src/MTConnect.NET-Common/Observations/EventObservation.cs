// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
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
            SetProperty("Category", DataItemCategory.EVENT);
        }


        public new static EventObservation Create(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                var observation = Create(dataItem.Type, dataItem.Representation);
                observation.DataItem = dataItem;
                observation.SetProperty(nameof(DataItemId), dataItem.Id);
                observation.SetProperty(nameof(Representation), dataItem.Representation);
                observation.SetProperty(nameof(Type), dataItem.Type);
                observation.SetProperty(nameof(SubType), dataItem.SubType);
                observation.SetProperty(nameof(Name), dataItem.Name);
                observation.SetProperty(nameof(CompositionId), dataItem.CompositionId);
                return observation;
            }

            return null;
        }

        public static EventObservation Create(IObservation observation)
        {
            if (observation != null && observation.DataItem != null)
            {
                var result = Create(observation.DataItem.Type, observation.DataItem.Representation);
                result.DataItem = observation.DataItem;
                result.SetProperty(nameof(DataItemId), observation.DataItem.Id);
                result.SetProperty(nameof(Representation), observation.DataItem.Representation);
                result.SetProperty(nameof(Type), observation.DataItem.Type);
                result.SetProperty(nameof(SubType), observation.DataItem.SubType);
                result.SetProperty(nameof(Name), observation.DataItem.Name);
                result.SetProperty(nameof(CompositionId), observation.DataItem.CompositionId);
                result.SetProperty(nameof(Sequence), observation.Sequence);
                result.SetProperty(nameof(Timestamp), observation.Timestamp);
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
                    var key = string.Intern(type + ":" + representation);

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
