// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleObservation : Observation, ISampleObservation
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
        /// The rate at which successive samples of the value of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a decimal type floating point number.
        /// For example, a rate of 1 per 10 seconds would be 0.1.
        /// The SampleRate attribute MUST be provided when the representation attribute for the data item is TIME_SERIES.
        /// For data items where the representation attribute for the data item IS NOT TIME_SERIES, it may be assumed that the SampleRate is constant and SampleRate does not need to be reported in the MTConnectStreams document.
        /// </summary>
        public double SampleRate
        {
            get => GetValue(ValueKeys.SampleRate).ToDouble();
            set
            {
                if (value > 0)
                {
                    AddValue(new ObservationValue(ValueKeys.SampleRate, value));
                }
            }
        }

        /// <summary>
        /// The type of statistical calculation specified in the Device Information Model that this Sample element represents.
        /// </summary>
        public DataItemStatistic Statistic
        {
            get => GetValue(ValueKeys.Statistic).ConvertEnum<DataItemStatistic>();
            set
            {
                if (value != DataItemStatistic.NONE)
                {
                    AddValue(new ObservationValue(ValueKeys.Statistic, value));
                }
            }
        }

        /// <summary>
        /// The time-period over which the data was collected.
        /// </summary>
        public double Duration
        {
            get => GetValue(ValueKeys.Duration).ToDouble();
            set
            {
                if (value > 0)
                {
                    AddValue(new ObservationValue(ValueKeys.Duration, value));
                }
            }
        }


        public SampleObservation()
        {
            _category = DataItemCategory.SAMPLE;
        }


        public new static SampleObservation Create(IDataItem dataItem)
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

        public static SampleObservation Create(IObservation observation)
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


        public static SampleObservation Create(string type, DataItemRepresentation representation)
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

                    if (_types.TryGetValue(typeId, out Type t))
                    {
                        var constructor = t.GetConstructor(System.Type.EmptyTypes);
                        if (constructor != null)
                        {
                            try
                            {
                                switch (representation)
                                {
                                    case DataItemRepresentation.VALUE: return (SampleValueObservation)Activator.CreateInstance(t);
                                    case DataItemRepresentation.DATA_SET: return (SampleDataSetObservation)Activator.CreateInstance(t);
                                    case DataItemRepresentation.TABLE: return (SampleTableObservation)Activator.CreateInstance(t);
                                    case DataItemRepresentation.TIME_SERIES: return (SampleTimeSeriesObservation)Activator.CreateInstance(t);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }

            switch (representation)
            {
                case DataItemRepresentation.DATA_SET: return new SampleDataSetObservation();
                case DataItemRepresentation.TABLE: return new SampleTableObservation();
                case DataItemRepresentation.TIME_SERIES: return new SampleTimeSeriesObservation();
                default: return new SampleValueObservation();
            }
        }
    }
}