// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for an EVENT observation carrying a
    /// scalar value in the cppagent-compatible Streams shape. The
    /// value is emitted under the <c>value</c> property as a free-form
    /// JSON value (string, number, or boolean) to match cppagent's
    /// loose event typing.
    /// </summary>
    public class JsonEventValue : JsonObservation
    {
        /// <summary>
        /// The scalar value of the event.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEventValue() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed event
        /// <see cref="IObservation"/>, optionally surfacing category and
        /// instance-id, and pulling the value, reset trigger, native
        /// code, and asset type from the observation's value bag.
        /// </summary>
        public JsonEventValue(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                if (categoryOutput) Category = observation.Category.ToString();
                if (instanceIdOutput) InstanceId = observation.InstanceId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                Sequence = observation.Sequence;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                Value = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);
            }
        }

        /// <summary>
        /// Initializes the surrogate from a streaming
        /// <see cref="IObservationOutput"/>, pulling the same value-bag
        /// fields as the model-level constructor.
        /// </summary>
        public JsonEventValue(IObservationOutput observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                InstanceId = observation.InstanceId;
                Sequence = observation.Sequence;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                Value = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);
            }
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IEventValueObservation"/>, restoring the data-item
        /// type from the supplied dictionary key.
        /// </summary>
        public IEventValueObservation ToObservation(string type)
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path. A naked
            // `new EventValueObservation()` collapses typed event subclasses
            // (e.g. MessageValueObservation, AssetChangedValueObservation)
            // back to the abstract value carrier, breaking `obs is
            // MessageValueObservation`-style branching downstream.
            var e = EventObservation.Create(type, DataItemRepresentation.VALUE) as EventValueObservation;
            if (e == null) e = new EventValueObservation();
            e.DataItemId = DataItemId;
            e.Timestamp = Timestamp;
            e.Name = Name;
            e.InstanceId = InstanceId;
            e.Sequence = Sequence;
            //e.Category = Category.ConvertEnum<DataItemCategory>();
            e.Type = type;
            e.SubType = SubType;
            e.CompositionId = CompositionId;
            e.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            if (Value != null) e.Result = Value.ToString();
            return e;
        }
    }
}