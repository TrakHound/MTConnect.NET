// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations.Input;
using System;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

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
        [XmlAttribute("resetTriggered")]
        [JsonPropertyName("resetTriggered")]
        public ResetTriggered ResetTriggered
        {
            get => GetValue(ValueKeys.ResetTriggered).ConvertEnum<ResetTriggered>();
            set => AddValue(new ObservationValue(ValueKeys.ResetTriggered, value));
        }

        internal bool ResetTriggeredOutput => ResetTriggered != ResetTriggered.NOT_SPECIFIED;

        /// <summary>
        /// The rate at which successive samples of the value of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a decimal type floating point number.
        /// For example, a rate of 1 per 10 seconds would be 0.1.
        /// The SampleRate attribute MUST be provided when the representation attribute for the data item is TIME_SERIES.
        /// For data items where the representation attribute for the data item IS NOT TIME_SERIES, it may be assumed that the SampleRate is constant and SampleRate does not need to be reported in the MTConnectStreams document.
        /// </summary>
        [XmlAttribute("sampleRate")]
        [JsonPropertyName("sampleRate")]
        public double SampleRate
        {
            get => GetValue(ValueKeys.SampleRate).ToDouble();
            set => AddValue(new ObservationValue(ValueKeys.SampleRate, value));
        }

        internal bool SampleRateOutput => SampleRate > 0;

        /// <summary>
        /// The type of statistical calculation specified in the Device Information Model that this Sample element represents.
        /// </summary>
        [XmlAttribute("statistic")]
        [JsonPropertyName("statistic")]
        public DataItemStatistic Statistic
        {
            get => GetValue(ValueKeys.Statistic).ConvertEnum<DataItemStatistic>();
            set => AddValue(new ObservationValue(ValueKeys.Statistic, value));
        }

        internal bool StatisticOutput => Statistic != DataItemStatistic.NONE;

        /// <summary>
        /// The time-period over which the data was collected.
        /// </summary>
        [XmlAttribute("duration")]
        [JsonPropertyName("duration")]
        public double Duration
        {
            get => GetValue(ValueKeys.Duration).ToDouble();
            set => AddValue(new ObservationValue(ValueKeys.Duration, value));
        }

        internal bool DurationOutput => Duration > 0;


        public SampleObservation()
        {
            SetProperty("Category", DataItemCategory.SAMPLE);
        }


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        protected ObservationValidationResult Validate<T>(Version mtconnectVersion, IObservationInput observation) where T : struct
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the CDATA Value for the Observation
                var cdata = observation.GetValue(ValueKeys.CDATA);
                if (cdata != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(T));
                    foreach (var validValue in validValues)
                    {
                        if (cdata == validValue.ToString())
                        {
                            return new ObservationValidationResult(true);
                        }
                    }

                    return new ObservationValidationResult(false, "'" + cdata + "' is not a valid value");
                }
                else
                {
                    return new ObservationValidationResult(false, "No CDATA is specified for the Observation");
                }
            }

            return new ObservationValidationResult(false, "No Observation is Specified");
        }


        public static SampleObservation Create(DataItem dataItem)
        {
            if (dataItem != null)
            {
                var observation = Create(dataItem.Type, dataItem.Representation);
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

        public static SampleObservation Create(string type, DataItemRepresentation representation)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    var key = $"{type.ToPascalCase()}{representation.ToString().ToPascalCase()}";

                    if (_types.TryGetValue(key, out Type t))
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
