// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// Sequence of Condition observations in the cppagent JSON v2 shape:
    /// a flat JSON array of single-key <see cref="JsonConditionWrapper"/>
    /// objects, one wrapper per observation, in insertion order.
    /// </summary>
    /// <remarks>
    /// The type shape IS the wire shape — <see cref="JsonConditions"/>
    /// inherits <see cref="List{T}"/> of <see cref="JsonConditionWrapper"/>
    /// so System.Text.Json's default serializer handles both directions
    /// with no custom converter. Emitting bytes:
    /// <code>[{"Normal":{...}},{"Warning":{...}},{"Fault":{...}}]</code>
    /// requires only that each wrapper carries exactly one non-null
    /// property (enforced by convention, not by runtime validation) and
    /// that the four properties on <see cref="JsonConditionWrapper"/> are
    /// annotated with
    /// <c>[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]</c>.
    /// <para>
    /// Element ordering on the wire follows list insertion order. The
    /// <see cref="JsonConditions(IEnumerable{IObservationOutput})"/> ctor
    /// preserves the historical level-order emission
    /// (<c>FAULT</c> → <c>WARNING</c> → <c>NORMAL</c> → <c>UNAVAILABLE</c>,
    /// source order within each level) so this rewrite is byte-identical
    /// on the wire to the pre-refactor converter output for the same
    /// observation input; callers constructing via the copy ctor or
    /// direct <see cref="List{T}.Add(T)"/> control ordering explicitly.
    /// </para>
    /// </remarks>
    public sealed class JsonConditions : List<JsonConditionWrapper>
    {
        /// <summary>
        /// Initializes an empty container for JSON deserialization or
        /// programmatic construction via <see cref="List{T}.Add(T)"/>.
        /// </summary>
        public JsonConditions() { }

        /// <summary>
        /// Initializes the container with a pre-built sequence of
        /// wrappers. Order is preserved from
        /// <paramref name="wrappers"/>.
        /// </summary>
        public JsonConditions(IEnumerable<JsonConditionWrapper> wrappers)
            : base(wrappers ?? Enumerable.Empty<JsonConditionWrapper>())
        {
        }

        /// <summary>
        /// Initializes the container from an observation-output
        /// sequence, wrapping each observation in the single-key wrapper
        /// for its level. Preserves the historical level-order emission
        /// (<c>FAULT</c> → <c>WARNING</c> → <c>NORMAL</c> → <c>UNAVAILABLE</c>,
        /// source order within each level) so the wire output is
        /// byte-identical to the pre-refactor converter for the same
        /// input.
        /// </summary>
        public JsonConditions(IEnumerable<IObservationOutput> observations)
        {
            if (observations == null) return;

            AppendLevel(observations, ConditionLevel.FAULT,       JsonConditionWrapper.OfFault);
            AppendLevel(observations, ConditionLevel.WARNING,     JsonConditionWrapper.OfWarning);
            AppendLevel(observations, ConditionLevel.NORMAL,      JsonConditionWrapper.OfNormal);
            AppendLevel(observations, ConditionLevel.UNAVAILABLE, JsonConditionWrapper.OfUnavailable);
        }

        /// <summary>
        /// Materializes every wrapper into a flat
        /// <see cref="List{T}"/> of <see cref="IObservation"/>, in list
        /// insertion order. Wrappers whose four level properties are all
        /// <see langword="null"/> are skipped. Not serialized.
        /// </summary>
        [JsonIgnore]
        public List<IObservation> Observations
        {
            get
            {
                var result = new List<IObservation>(Count);
                foreach (var wrapper in this)
                {
                    var observation = wrapper?.ToObservation();
                    if (observation != null) result.Add(observation);
                }
                return result;
            }
        }

        private void AppendLevel(
            IEnumerable<IObservationOutput> observations,
            ConditionLevel level,
            System.Func<JsonCondition, JsonConditionWrapper> factory)
        {
            var levelName = level.ToString();
            foreach (var observation in observations.Where(o => o != null && o.GetValue(ValueKeys.Level) == levelName))
            {
                Add(factory(new JsonCondition(observation)));
            }
        }
    }
}
