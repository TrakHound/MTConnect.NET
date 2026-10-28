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


        /// <summary>
        /// Initializes a new Event Observation with the Event category.
        /// </summary>
        public EventObservation()
        {
            _category = DataItemCategory.EVENT;
        }


        /// <summary>
        /// Creates an Event Observation for the given DataItem, copying its identifying metadata.
        /// </summary>
        /// <param name="dataItem">The DataItem the Observation is created for.</param>
        /// <returns>The Event Observation, or null when <paramref name="dataItem"/> is null.</returns>
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

        /// <summary>
        /// Creates an Event Observation from an existing Observation, copying its metadata and values.
        /// </summary>
        /// <param name="observation">The source Observation to copy.</param>
        /// <returns>The Event Observation, or null when <paramref name="observation"/> is null.</returns>
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


        /// <summary>
        /// Creates the Event Observation subclass matching the given type and representation.
        /// </summary>
        /// <param name="type">The DataItem Type identifier.</param>
        /// <param name="representation">The DataItem representation.</param>
        /// <returns>A type-specific Event Observation, or a base EventObservation when the type is not recognized.</returns>
        public static EventObservation Create(string type, DataItemRepresentation representation)
        {
            if (!string.IsNullOrEmpty(type))
            {
                // Cached constructor delegate for the type-specific Observation class (no locking or reflection per call)
                var factory = GetFactory(type, representation);
                if (factory != null)
                {
                    try
                    {
                        switch (representation)
                        {
                            case DataItemRepresentation.VALUE: return (EventValueObservation)factory();
                            case DataItemRepresentation.DATA_SET: return (EventDataSetObservation)factory();
                            case DataItemRepresentation.TABLE: return (EventTableObservation)factory();
                        }
                    }
                    catch { }
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