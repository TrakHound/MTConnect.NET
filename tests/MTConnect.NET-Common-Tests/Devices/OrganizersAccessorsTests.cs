// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// Covers `MTConnect.Devices.Organizers` accessors and the full
// `GetOrganizerType` else-if chain. Paired with `OrganizersSystemsTests`
// (which targets the System substitution-group surface) so that the
// production file `libraries/MTConnect.NET-Common/Devices/Organizers.cs`
// reaches 100% line + branch + method coverage.

using System.Collections.Generic;
using System.Linq;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Interfaces;
using NUnit.Framework;

namespace MTConnect.Tests.Common.SystemsOrganizer
{
    [TestFixture]
    [Category("OrganizersAccessors")]
    public class OrganizersAccessorsTests
    {
        [Test]
        public void Components_lists_every_first_class_organizer()
        {
            var expected = new[]
            {
                AdaptersComponent.TypeId,
                AuxiliariesComponent.TypeId,
                AxesComponent.TypeId,
                ControllersComponent.TypeId,
                InterfacesComponent.TypeId,
                MaterialsComponent.TypeId,
                PartsComponent.TypeId,
                ProcessesComponent.TypeId,
                ResourcesComponent.TypeId,
                SystemsComponent.TypeId,
                StructuresComponent.TypeId,
            };
            Assert.That(Organizers.Components, Is.EquivalentTo(expected));
        }

        [Test]
        public void Adapters_lists_AdapterComponent()
        {
            Assert.That(Organizers.Adapters, Is.EquivalentTo(new[] { AdapterComponent.TypeId }));
        }

        [Test]
        public void Auxiliaries_lists_known_auxiliary_components()
        {
            Assert.That(Organizers.Auxiliaries, Is.EquivalentTo(new[]
            {
                DepositionComponent.TypeId,
                EnvironmentalComponent.TypeId,
                LoaderComponent.TypeId,
                ToolingDeliveryComponent.TypeId,
                WasteDisposalComponent.TypeId,
            }));
        }

        [Test]
        public void Axes_lists_Linear_and_Rotary()
        {
            Assert.That(Organizers.Axes, Is.EquivalentTo(new[]
            {
                LinearComponent.TypeId,
                RotaryComponent.TypeId,
            }));
        }

        [Test]
        public void Controllers_lists_ControllerComponent()
        {
            Assert.That(Organizers.Controllers, Is.EquivalentTo(new[] { ControllerComponent.TypeId }));
        }

        [Test]
        public void Interfaces_lists_known_interface_components()
        {
            Assert.That(Organizers.Interfaces, Is.EquivalentTo(new[]
            {
                BarFeederInterface.TypeId,
                ChuckInterface.TypeId,
                DoorInterface.TypeId,
                MaterialHandlerInterface.TypeId,
            }));
        }

        [Test]
        public void Materials_lists_StockComponent()
        {
            Assert.That(Organizers.Materials, Is.EquivalentTo(new[] { StockComponent.TypeId }));
        }

        [Test]
        public void Parts_lists_PartOccurrenceComponent()
        {
            Assert.That(Organizers.Parts, Is.EquivalentTo(new[] { PartOccurrenceComponent.TypeId }));
        }

        [Test]
        public void Processes_lists_ProcessOccurrenceComponent()
        {
            Assert.That(Organizers.Processes, Is.EquivalentTo(new[] { ProcessOccurrenceComponent.TypeId }));
        }

        [Test]
        public void Resources_lists_known_resource_components()
        {
            Assert.That(Organizers.Resources, Is.EquivalentTo(new[]
            {
                ResourceComponent.TypeId,
                PersonnelComponent.TypeId,
            }));
        }

        [Test]
        public void Structures_lists_LinkComponent()
        {
            Assert.That(Organizers.Structures, Is.EquivalentTo(new[] { LinkComponent.TypeId }));
        }

        // GetOrganizerType — exhaustive branch coverage of the else-if chain.
        // Each test names one organizer family and supplies a representative
        // member from that family, so every branch in the chain executes.
        [TestCase(null)]
        public void GetOrganizerType_null_input_returns_null(string? typeId)
        {
            Assert.That(Organizers.GetOrganizerType(typeId), Is.Null);
        }

        [Test]
        public void GetOrganizerType_unknown_type_returns_null()
        {
            Assert.That(Organizers.GetOrganizerType("ThisIsNotAnyKnownComponentType"), Is.Null);
        }

        [Test]
        public void GetOrganizerType_for_adapter_returns_Adapters()
        {
            Assert.That(Organizers.GetOrganizerType(AdapterComponent.TypeId),
                Is.EqualTo(AdaptersComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_auxiliary_returns_Auxiliaries()
        {
            Assert.That(Organizers.GetOrganizerType(LoaderComponent.TypeId),
                Is.EqualTo(AuxiliariesComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_axis_returns_Axes()
        {
            Assert.That(Organizers.GetOrganizerType(LinearComponent.TypeId),
                Is.EqualTo(AxesComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_interface_returns_Interfaces()
        {
            Assert.That(Organizers.GetOrganizerType(DoorInterface.TypeId),
                Is.EqualTo(InterfacesComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_material_returns_Materials()
        {
            Assert.That(Organizers.GetOrganizerType(StockComponent.TypeId),
                Is.EqualTo(MaterialsComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_part_returns_Parts()
        {
            Assert.That(Organizers.GetOrganizerType(PartOccurrenceComponent.TypeId),
                Is.EqualTo(PartsComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_process_returns_Processes()
        {
            Assert.That(Organizers.GetOrganizerType(ProcessOccurrenceComponent.TypeId),
                Is.EqualTo(ProcessesComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_resource_returns_Resources()
        {
            Assert.That(Organizers.GetOrganizerType(PersonnelComponent.TypeId),
                Is.EqualTo(ResourcesComponent.TypeId));
        }

        [Test]
        public void GetOrganizerType_for_structure_returns_Structures()
        {
            Assert.That(Organizers.GetOrganizerType(LinkComponent.TypeId),
                Is.EqualTo(StructuresComponent.TypeId));
        }
    }
}
