// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations.Output;
using System;
using System.Collections.Generic;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// Groups observations by Type and Representation in a single pass so the generated
    /// <see cref="JsonSamples"/> and <see cref="JsonEvents"/> constructors can look up each
    /// typed collection directly instead of scanning every observation once per type.
    /// Observations keep their original relative order within each group.
    /// </summary>
    internal sealed class JsonObservationLookup
    {
        private static readonly int _representationCount = GetRepresentationCount();

        private readonly Dictionary<string, List<IObservationOutput>[]> _groups = new Dictionary<string, List<IObservationOutput>[]>();


        public JsonObservationLookup(IEnumerable<IObservationOutput> observations)
        {
            if (observations != null)
            {
                foreach (var observation in observations)
                {
                    if (observation != null && observation.Type != null)
                    {
                        var representation = (int)observation.Representation;
                        if (representation < 0 || representation >= _representationCount) continue;

                        if (!_groups.TryGetValue(observation.Type, out var representations))
                        {
                            representations = new List<IObservationOutput>[_representationCount];
                            _groups.Add(observation.Type, representations);
                        }

                        var group = representations[representation];
                        if (group == null)
                        {
                            group = new List<IObservationOutput>();
                            representations[representation] = group;
                        }

                        group.Add(observation);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the observations matching <paramref name="type"/> and <paramref name="representation"/>,
        /// or <c>null</c> when there are none.
        /// </summary>
        public IEnumerable<IObservationOutput> Get(string type, DataItemRepresentation representation)
        {
            if (type != null && _groups.TryGetValue(type, out var representations))
            {
                var index = (int)representation;
                if (index >= 0 && index < representations.Length && representations[index] != null)
                {
                    HasReturned = true;
                    return representations[index];
                }
            }

            return null;
        }

        /// <summary>
        /// True once <see cref="Get"/> has returned observations (i.e. the typed container will not be empty).
        /// </summary>
        public bool HasReturned { get; private set; }


        private static int GetRepresentationCount()
        {
            var max = 0;
            foreach (var value in Enum.GetValues(typeof(DataItemRepresentation)))
            {
                var i = (int)value;
                if (i > max) max = i;
            }
            return max + 1;
        }
    }
}
