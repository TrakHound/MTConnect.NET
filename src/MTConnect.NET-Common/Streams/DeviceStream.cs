// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;

namespace MTConnect.Streams
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    public class DeviceStream : IDeviceStream
    {
        /// <summary>
        /// The name of an element or a piece of equipment. The name associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The uuid associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// An XML container type element that organizes data returned from an Agent in response to a current or sample HTTP request.
        /// </summary>
        public IEnumerable<IComponentStream> ComponentStreams { get; set; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        public IEnumerable<IObservation> Observations
        {
            get
            {
                var l = new List<IObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Observations.IsNullOrEmpty())
                        {
                            l.AddRange(componentStream.Observations);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE
        /// </summary>
        public IEnumerable<ISampleObservation> Samples
        {
            get
            {
                var l = new List<ISampleObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Samples.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.Samples) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of VALUE
        /// </summary>
        public IEnumerable<ISampleValueObservation> SampleValues
        {
            get
            {
                var l = new List<ISampleValueObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleValues.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.SampleValues) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TIME_SERIES
        /// </summary>
        public IEnumerable<ISampleTimeSeriesObservation> SampleTimeSeries
        {
            get
            {
                var l = new List<ISampleTimeSeriesObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleTimeSeries.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.SampleTimeSeries) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of DATA_SET
        /// </summary>
        public IEnumerable<ISampleDataSetObservation> SampleDataSets
        {
            get
            {
                var l = new List<ISampleDataSetObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleDataSets.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.SampleDataSets) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TABLE
        /// </summary>
        public IEnumerable<ISampleTableObservation> SampleTables
        {
            get
            {
                var l = new List<ISampleTableObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.SampleTables.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.SampleTables) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }


        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT
        /// </summary>
        public IEnumerable<IEventObservation> Events
        {
            get
            {
                var l = new List<IEventObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Events.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.Events) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of VALUE
        /// </summary>
        public IEnumerable<IEventValueObservation> EventValues
        {
            get
            {
                var l = new List<IEventValueObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.EventValues.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.EventValues) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of DATA_SET
        /// </summary>
        public IEnumerable<IEventDataSetObservation> EventDataSets
        {
            get
            {
                var l = new List<IEventDataSetObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.EventDataSets.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.EventDataSets) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of TABLE
        /// </summary>
        public IEnumerable<IEventTableObservation> EventTables
        {
            get
            {
                var l = new List<IEventTableObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.EventTables.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.EventTables) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }


        /// <summary>
        /// Condition organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of CONDITION in the MTConnectDevices document.
        /// </summary>
        public IEnumerable<IConditionObservation> Conditions
        {
            get
            {
                var l = new List<IConditionObservation>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Conditions.IsNullOrEmpty())
                        {
                            foreach (var observation in componentStream.Conditions) l.Add(observation);
                        }
                    }
                }

                return l;
            }
        }
    }
}
