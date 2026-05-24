// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Observations
{
    /// <summary>
    /// Pins the runtime-type discriminator across the three observation
    /// factory methods:
    ///
    ///   EventObservation.Create(string type, DataItemRepresentation)
    ///   SampleObservation.Create(string type, DataItemRepresentation)
    ///   ConditionObservation.Create(string type, DataItemRepresentation)
    ///
    /// Each factory consults a static <c>_types</c> dictionary populated by
    /// <c>Observation.GetAllTypes()</c>. <c>GetAllTypes()</c> keys the dict
    /// by the class-name-minus-<c>Observation</c> suffix (e.g.
    /// <c>MessageValueObservation</c> → key <c>"MessageValue"</c>). The
    /// factory must look up the dict using the same key format the
    /// dictionary is populated with, otherwise every typed-subclass lookup
    /// misses and the factory falls through to the abstract carrier
    /// (<c>EventValueObservation</c> / <c>SampleValueObservation</c> /
    /// <c>ConditionObservation</c>) — losing runtime-type discrimination
    /// downstream, including the JSON-cppagent envelope read path that
    /// 85394a5d routed onto these factories.
    ///
    /// These tests demonstrate the bug at HEAD (the Event-typed cases
    /// fail because the factory misconstructs the lookup key); the
    /// Sample/Condition cases pin the no-typed-subclass fallback so the
    /// representation branch is locked in too.
    /// </summary>
    [TestFixture]
    [Category("ObservationFactoryRuntimeType")]
    public class ObservationFactoryRuntimeTypeTests
    {
        // ---- EventObservation.Create(string, DataItemRepresentation) ----

        [Test]
        public void EventObservation_Create_MESSAGE_VALUE_returns_MessageValueObservation()
        {
            // MessageValueObservation lives in MTConnect.Observations.Events
            // and is populated into Observation._types under the regex-stripped
            // class-name key "MessageValue". The factory must resolve
            // ("MESSAGE", VALUE) onto that key.
            var result = EventObservation.Create(
                MessageDataItem.TypeId,
                DataItemRepresentation.VALUE);

            Assert.That(result, Is.Not.Null,
                "EventObservation.Create must never return null for a known type.");
            Assert.That(result, Is.InstanceOf<MessageValueObservation>(),
                "Factory must hydrate the typed MessageValueObservation runtime "
                + "type for MESSAGE/VALUE; otherwise every downstream "
                + "`obs is MessageValueObservation` branch dead-ends.");
            // The (string, DataItemRepresentation) overload only constructs
            // the runtime carrier; the IDataItem / IObservation overloads
            // are responsible for stamping Type/DataItemId on the result.
            // So we deliberately do NOT assert result.Type here.
        }

        [Test]
        public void EventObservation_Create_ASSET_CHANGED_VALUE_returns_AssetChangedValueObservation()
        {
            // ASSET_CHANGED exercises the underscore-split branch of
            // ToPascalCase, so it widens coverage past the single-token
            // MESSAGE case. AssetChangedValueObservation also lives in
            // MTConnect.Observations.Events.
            var result = EventObservation.Create(
                AssetChangedDataItem.TypeId,
                DataItemRepresentation.VALUE);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<AssetChangedValueObservation>(),
                "Factory must hydrate AssetChangedValueObservation for "
                + "ASSET_CHANGED/VALUE — the underscore-bearing type id is "
                + "the hostile case for any key-format mismatch.");
        }

        [Test]
        public void EventObservation_Create_unknown_type_falls_back_to_EventValueObservation()
        {
            // No subclass exists for an arbitrary EVENT type id, so the
            // factory must return the abstract EventValueObservation carrier
            // — never null. Pins the no-regression fallback path.
            var result = EventObservation.Create(
                "VENDOR_SPECIFIC_EVENT_THAT_HAS_NO_TYPED_SUBCLASS",
                DataItemRepresentation.VALUE);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<EventValueObservation>(),
                "Unknown EVENT/VALUE types must land on the abstract carrier.");
        }

        [Test]
        public void EventObservation_Create_DATA_SET_returns_EventDataSetObservation()
        {
            // No typed *EventDataSetObservation subclass exists in the
            // shipped model, so the factory must fall through onto the
            // abstract representation carrier.
            var result = EventObservation.Create(
                "ANY_EVENT_TYPE",
                DataItemRepresentation.DATA_SET);

            Assert.That(result, Is.InstanceOf<EventDataSetObservation>(),
                "EVENT/DATA_SET must land on the abstract EventDataSetObservation carrier.");
        }

        // ---- SampleObservation.Create(string, DataItemRepresentation) ----

        [Test]
        public void SampleObservation_Create_VALUE_returns_SampleValueObservation()
        {
            // No typed Sample*Observation subclasses ship in the runtime, so
            // every Sample factory call must land on the representation
            // carrier. Pin every representation branch.
            var result = SampleObservation.Create(
                "POSITION",
                DataItemRepresentation.VALUE);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<SampleValueObservation>(),
                "SAMPLE/VALUE must hydrate SampleValueObservation.");
        }

        [Test]
        public void SampleObservation_Create_DATA_SET_returns_SampleDataSetObservation()
        {
            var result = SampleObservation.Create(
                "AXIS_FEEDRATE",
                DataItemRepresentation.DATA_SET);

            Assert.That(result, Is.InstanceOf<SampleDataSetObservation>(),
                "SAMPLE/DATA_SET must hydrate SampleDataSetObservation.");
        }

        [Test]
        public void SampleObservation_Create_TABLE_returns_SampleTableObservation()
        {
            var result = SampleObservation.Create(
                "AXIS_FEEDRATE",
                DataItemRepresentation.TABLE);

            Assert.That(result, Is.InstanceOf<SampleTableObservation>(),
                "SAMPLE/TABLE must hydrate SampleTableObservation.");
        }

        [Test]
        public void SampleObservation_Create_TIME_SERIES_returns_SampleTimeSeriesObservation()
        {
            var result = SampleObservation.Create(
                "AXIS_FEEDRATE",
                DataItemRepresentation.TIME_SERIES);

            Assert.That(result, Is.InstanceOf<SampleTimeSeriesObservation>(),
                "SAMPLE/TIME_SERIES must hydrate SampleTimeSeriesObservation.");
        }

        // ---- ConditionObservation.Create(string, DataItemRepresentation) -

        [Test]
        public void ConditionObservation_Create_returns_ConditionObservation_carrier()
        {
            // No typed Condition*Observation subclasses ship in the runtime,
            // so the strongest pin is "non-null, base ConditionObservation
            // carrier". Same defence-in-depth check the JSON-cppagent
            // JsonCondition.ToCondition test takes.
            var result = ConditionObservation.Create(
                "SYSTEM",
                DataItemRepresentation.VALUE);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ConditionObservation>(),
                "CONDITION/VALUE must hydrate ConditionObservation.");
        }
    }
}
