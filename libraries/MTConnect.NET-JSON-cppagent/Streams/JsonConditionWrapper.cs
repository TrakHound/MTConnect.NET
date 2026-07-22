// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// One Condition observation as it appears on the wire in the cppagent
    /// JSON v2 shape: an object with exactly one of <see cref="Fault"/>,
    /// <see cref="Warning"/>, <see cref="Normal"/>, or <see cref="Unavailable"/>
    /// set and the other three <see langword="null"/>. The three null members
    /// are suppressed on serialization by
    /// <see cref="JsonIgnoreCondition.WhenWritingNull"/>, producing the
    /// single-key envelope cppagent emits (e.g. <c>{"Normal": {...}}</c>).
    /// </summary>
    /// <remarks>
    /// The four properties are <b>data-carriers only</b>: setting more than
    /// one at a time produces a wire shape cppagent will reject on read but
    /// the type does not enforce single-key invariants at runtime. Callers
    /// should prefer the <c>Of*</c> factory methods, which construct a
    /// wrapper with exactly one property populated.
    /// <para>
    /// <see cref="Value"/> and <see cref="Level"/> are convenience read-side
    /// accessors for consumers that iterate a <see cref="JsonConditions"/>
    /// list without pattern-matching on which property is non-null; both are
    /// suppressed from serialization.
    /// </para>
    /// </remarks>
    public sealed class JsonConditionWrapper
    {
        /// <summary>
        /// Condition entry at <c>FAULT</c> level, or <see langword="null"/>
        /// when this wrapper carries a different level. Serializes as the
        /// wire property <c>Fault</c>.
        /// </summary>
        [JsonPropertyName("Fault")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonCondition Fault { get; set; }

        /// <summary>
        /// Condition entry at <c>WARNING</c> level, or <see langword="null"/>
        /// when this wrapper carries a different level. Serializes as the
        /// wire property <c>Warning</c>.
        /// </summary>
        [JsonPropertyName("Warning")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonCondition Warning { get; set; }

        /// <summary>
        /// Condition entry at <c>NORMAL</c> level, or <see langword="null"/>
        /// when this wrapper carries a different level. Serializes as the
        /// wire property <c>Normal</c>.
        /// </summary>
        [JsonPropertyName("Normal")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonCondition Normal { get; set; }

        /// <summary>
        /// Condition entry at <c>UNAVAILABLE</c> level, or <see langword="null"/>
        /// when this wrapper carries a different level. Serializes as the
        /// wire property <c>Unavailable</c>.
        /// </summary>
        [JsonPropertyName("Unavailable")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonCondition Unavailable { get; set; }

        /// <summary>
        /// The single non-null condition carried by this wrapper, or
        /// <see langword="null"/> when the wrapper is empty. Precedence
        /// order on multi-populated wrappers is
        /// Fault → Warning → Normal → Unavailable, matching
        /// <see cref="Level"/>. Never serialized.
        /// </summary>
        [JsonIgnore]
        public JsonCondition Value => Fault ?? Warning ?? Normal ?? Unavailable;

        /// <summary>
        /// The wire property name of the single non-null level carried by
        /// this wrapper (<c>"Fault"</c>, <c>"Warning"</c>, <c>"Normal"</c>,
        /// or <c>"Unavailable"</c>), or <see langword="null"/> when the
        /// wrapper is empty. Never serialized.
        /// </summary>
        [JsonIgnore]
        public string Level =>
            Fault       != null ? "Fault"       :
            Warning     != null ? "Warning"     :
            Normal      != null ? "Normal"      :
            Unavailable != null ? "Unavailable" : null;

        /// <summary>
        /// Constructs a wrapper carrying the given condition at
        /// <c>FAULT</c> level.
        /// </summary>
        public static JsonConditionWrapper OfFault(JsonCondition condition)       => new JsonConditionWrapper { Fault       = condition };

        /// <summary>
        /// Constructs a wrapper carrying the given condition at
        /// <c>WARNING</c> level.
        /// </summary>
        public static JsonConditionWrapper OfWarning(JsonCondition condition)     => new JsonConditionWrapper { Warning     = condition };

        /// <summary>
        /// Constructs a wrapper carrying the given condition at
        /// <c>NORMAL</c> level.
        /// </summary>
        public static JsonConditionWrapper OfNormal(JsonCondition condition)      => new JsonConditionWrapper { Normal      = condition };

        /// <summary>
        /// Constructs a wrapper carrying the given condition at
        /// <c>UNAVAILABLE</c> level.
        /// </summary>
        public static JsonConditionWrapper OfUnavailable(JsonCondition condition) => new JsonConditionWrapper { Unavailable = condition };

        /// <summary>
        /// Materializes this wrapper's condition into a strongly-typed
        /// <see cref="IConditionObservation"/> at the level indicated by
        /// which property is non-null, or <see langword="null"/> when the
        /// wrapper is empty. Precedence on multi-populated wrappers
        /// matches <see cref="Level"/>.
        /// </summary>
        public IConditionObservation ToObservation()
        {
            if (Fault       != null) return Fault.ToCondition(ConditionLevel.FAULT);
            if (Warning     != null) return Warning.ToCondition(ConditionLevel.WARNING);
            if (Normal      != null) return Normal.ToCondition(ConditionLevel.NORMAL);
            if (Unavailable != null) return Unavailable.ToCondition(ConditionLevel.UNAVAILABLE);
            return null;
        }
    }
}
