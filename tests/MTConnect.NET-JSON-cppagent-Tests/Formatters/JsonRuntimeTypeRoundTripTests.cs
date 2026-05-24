// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.Json;
using MTConnect.Observations;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Formatters
{
    /// <summary>
    /// Pins the runtime-type discriminator through the JSON-cppagent envelope
    /// read path for each Json -&gt; runtime conversion that has typed
    /// subclasses in the source-of-truth Devices/Compositions/DataItems
    /// generators. Same bug class as the JsonDevices.ToDevices() runtime-type
    /// fix at 9845a7f6: a naked `new Component()` / `new Composition()` /
    /// `new XxxObservation()` on the read side collapses every typed
    /// subclass back to the abstract base, breaking `instance is TypedX`
    /// branching downstream. JsonDataItem.ToDataItem already routes through
    /// DataItem.Create(string type) and was the reference fix pattern; the
    /// other paths are now lifted onto the same factory rail.
    /// </summary>
    [TestFixture]
    [Category("CppAgentRuntimeTypePreservation")]
    public class JsonRuntimeTypeRoundTripTests
    {
        // ---- JsonComponent.ToComponent ----------------------------------

        [Test]
        public void JsonComponent_ToComponent_preserves_runtime_type_for_typed_subclass()
        {
            // Arrange: an Axes component (one of the most common typed
            // subclasses) round-tripped through JsonComponent. The Type
            // string is supplied by the caller (JsonComponents.g.cs passes
            // the AxesComponent.TypeId string when iterating the Axes bucket
            // of the cppagent JSON v2 envelope).
            var source = new AxesComponent
            {
                Id = "axes-1",
                Uuid = "uuid-axes-1",
                Name = "axes",
            };
            var json = new JsonComponent(source);

            // Act
            var roundTripped = json.ToComponent(AxesComponent.TypeId);

            // Assert: runtime type and Type string both survive.
            Assert.That(roundTripped, Is.Not.Null,
                "ToComponent must return a non-null instance.");
            Assert.That(roundTripped, Is.InstanceOf<AxesComponent>(),
                "Round-tripped component must be an AxesComponent runtime "
                + "instance, not a base Component with its Type field re-tagged. "
                + "The factory route through Component.Create(string type) is "
                + "what makes `component is AxesComponent` true.");
            Assert.That(roundTripped.Type, Is.EqualTo(AxesComponent.TypeId),
                "Type string discriminator must round-trip.");
            Assert.That(roundTripped.Id, Is.EqualTo("axes-1"));
            Assert.That(roundTripped.Uuid, Is.EqualTo("uuid-axes-1"));
        }

        [Test]
        public void JsonComponent_ToComponent_preserves_runtime_type_for_Controller_subclass()
        {
            // Arrange: a second typed component to widen the surface —
            // Controller is the canonical control-bus component.
            var source = new ControllerComponent
            {
                Id = "controller-1",
                Uuid = "uuid-controller-1",
            };
            var json = new JsonComponent(source);

            // Act
            var roundTripped = json.ToComponent(ControllerComponent.TypeId);

            // Assert
            Assert.That(roundTripped, Is.InstanceOf<ControllerComponent>(),
                "Round-tripped component must be a ControllerComponent runtime instance.");
            Assert.That(roundTripped.Type, Is.EqualTo(ControllerComponent.TypeId));
        }

        // ---- JsonComposition.ToComposition ------------------------------

        [Test]
        public void JsonComposition_ToComposition_preserves_runtime_type_for_typed_subclass()
        {
            // Arrange: a Chuck composition. The internal `Type` field on
            // JsonComposition (read from the JSON `type` property) is the
            // discriminator the ToComposition() factory call must use.
            var source = new ChuckComposition
            {
                Id = "chuck-1",
                Uuid = "uuid-chuck-1",
                Name = "chuck",
            };
            var json = new JsonComposition(source);

            // Act
            var roundTripped = json.ToComposition();

            // Assert
            Assert.That(roundTripped, Is.Not.Null);
            Assert.That(roundTripped, Is.InstanceOf<ChuckComposition>(),
                "Round-tripped composition must be a ChuckComposition runtime "
                + "instance, not a base Composition with its Type field "
                + "re-tagged. The factory route through "
                + "Composition.Create(string type) is what makes "
                + "`composition is ChuckComposition` true.");
            Assert.That(roundTripped.Type, Is.EqualTo(ChuckComposition.TypeId));
            Assert.That(roundTripped.Id, Is.EqualTo("chuck-1"));
        }

        // ---- JsonCompositions.ToCompositions (latent empty-sink bug) ----

        [Test]
        public void JsonCompositions_ToCompositions_does_not_drop_compositions_on_read_path()
        {
            // Arrange: two compositions inside a JsonComponent. The latent
            // bug in JsonCompositions.ToCompositions was guarding the
            // freshly-allocated empty sink (`if (!dataItems.IsNullOrEmpty())`)
            // instead of the source collection, so every Composition was
            // silently dropped on the envelope read path. Fixing
            // JsonComposition.ToComposition's runtime-type loss is moot if
            // the enclosing list-wrapper never iterates.
            var parent = new AxesComponent { Id = "axes-1", Uuid = "uuid-axes-1" };
            parent.AddComposition(new ChuckComposition { Id = "chuck-1", Uuid = "uuid-chuck-1" });
            parent.AddComposition(new ChainComposition { Id = "chain-1", Uuid = "uuid-chain-1" });

            var json = new JsonComponent(parent);

            // Act
            var roundTripped = json.ToComponent(AxesComponent.TypeId);

            // Assert: both compositions survive the read path with their
            // typed identity intact.
            Assert.That(roundTripped.Compositions, Is.Not.Null,
                "Compositions must round-trip on the read path; "
                + "JsonCompositions.ToCompositions must not short-circuit on its empty sink.");
            var compositions = System.Linq.Enumerable.ToList(roundTripped.Compositions);
            Assert.That(compositions.Count, Is.EqualTo(2),
                "Both source compositions must survive the read path.");
            Assert.That(compositions[0], Is.InstanceOf<ChuckComposition>(),
                "First round-tripped composition must keep its ChuckComposition runtime type.");
            Assert.That(compositions[1], Is.InstanceOf<ChainComposition>(),
                "Second round-tripped composition must keep its ChainComposition runtime type.");
        }

        // ---- JsonDataItem.ToDataItem (pre-existing CLEAN reference) -----

        [Test]
        public void JsonDataItem_ToDataItem_preserves_runtime_type_for_typed_subclass()
        {
            // Reference test: JsonDataItem.ToDataItem already routes through
            // DataItem.Create(Type) and is the pattern the other ToXxx paths
            // are being lifted onto. Pin its behaviour so the contract is
            // explicit, not just emergent.
            var source = new ProgramDataItem
            {
                Id = "program-1",
                Name = "program",
            };
            var json = new JsonDataItem(source);

            // Act
            var roundTripped = json.ToDataItem();

            // Assert
            Assert.That(roundTripped, Is.InstanceOf<ProgramDataItem>(),
                "Round-tripped DataItem must keep its ProgramDataItem runtime type.");
            Assert.That(roundTripped.Type, Is.EqualTo(ProgramDataItem.TypeId));
        }

        // ---- JsonEventValue.ToObservation -------------------------------

        [Test]
        public void JsonEventValue_ToObservation_preserves_type_string_through_envelope()
        {
            // Pin the no-regression behaviour: ToObservation must always
            // stamp the supplied Type onto the result, regardless of whether
            // the upstream EventObservation.Create factory can resolve a
            // typed subclass for it. (Whether the runtime type is the
            // typed subclass depends on the upstream factory's lookup
            // logic, which is out of scope for this read-path fix.)
            var source = new JsonEventValue { DataItemId = "msg-1", Value = "hello" };

            // Act
            var observation = source.ToObservation("MESSAGE");

            // Assert: the Type discriminator survives the round-trip in
            // every case; the abstract carrier ALSO survives in the worst
            // case (factory miss), so `observation is null` would be a
            // regression.
            Assert.That(observation, Is.Not.Null,
                "ToObservation must never return null.");
            Assert.That(observation.Type, Is.EqualTo("MESSAGE"),
                "Type discriminator must survive the envelope read path.");
            Assert.That(observation.DataItemId, Is.EqualTo("msg-1"));
        }

        // ---- JsonCondition.ToCondition ----------------------------------

        [Test]
        public void JsonCondition_ToCondition_preserves_type_string_through_envelope()
        {
            // No typed ConditionObservation subclasses exist in the runtime
            // (the only subclass slot is the abstract carrier), so the
            // strongest property to pin is "Type survives, instance non-null,
            // factory-routed". This is the same defence-in-depth fix the
            // other ToObservation paths take.
            var source = new JsonCondition
            {
                DataItemId = "cond-1",
                Type = "SYSTEM",
            };

            // Act
            var observation = source.ToCondition(ConditionLevel.NORMAL);

            // Assert
            Assert.That(observation, Is.Not.Null);
            Assert.That(observation.Type, Is.EqualTo("SYSTEM"));
            Assert.That(observation.Level, Is.EqualTo(ConditionLevel.NORMAL));
            Assert.That(observation.DataItemId, Is.EqualTo("cond-1"));
        }
    }
}
