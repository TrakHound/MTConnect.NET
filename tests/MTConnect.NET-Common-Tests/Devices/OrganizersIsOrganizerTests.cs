// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// Pins the public `Organizers.IsOrganizer(string)` predicate: callers
// outside `MTConnect.Devices` need a cheap way to test whether a
// component TypeId is one of the well-known organizer container
// surfaces (`Adapters`, `Auxiliaries`, `Axes`, `Controllers`,
// `Interfaces`, `Materials`, `Parts`, `Processes`, `Resources`,
// `Systems`, `Structures`) without poking at the private organizer
// lists or recreating the membership check inline.
//
// Source (XSD): https://schemas.mtconnect.org/schemas/MTConnectDevices_*.xsd
//   Each organizer container is declared as a top-level `<xs:element>`
//   under the Device complex type and is the parent of one
//   substitution-group of member components.
// Source (prose): https://docs.mtconnect.org/Part_3.0_DevicesInformationModel
//   §"Component Organizers" enumerates the eleven organizer surfaces.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.Tests.Common.SystemsOrganizer
{
    [TestFixture]
    [Category("OrganizersIsOrganizer")]
    public class OrganizersIsOrganizerTests
    {
        [TestCase(nameof(AdaptersComponent))]
        [TestCase(nameof(AuxiliariesComponent))]
        [TestCase(nameof(AxesComponent))]
        [TestCase(nameof(ControllersComponent))]
        [TestCase(nameof(InterfacesComponent))]
        [TestCase(nameof(MaterialsComponent))]
        [TestCase(nameof(PartsComponent))]
        [TestCase(nameof(ProcessesComponent))]
        [TestCase(nameof(ResourcesComponent))]
        [TestCase(nameof(SystemsComponent))]
        [TestCase(nameof(StructuresComponent))]
        public void IsOrganizer_returns_true_for_each_first_class_organizer(string organizerName)
        {
            // The TypeId on each `*Component` matches the organizer's
            // public surface (e.g. `AdaptersComponent.TypeId == "Adapters"`).
            var typeId = organizerName.Replace("Component", string.Empty);
            Assert.That(Organizers.IsOrganizer(typeId), Is.True,
                $"`{typeId}` is one of the eleven first-class organizers; " +
                $"`Organizers.IsOrganizer(\"{typeId}\")` must return true.");
        }

        [Test]
        public void IsOrganizer_returns_false_for_member_components()
        {
            // A SysML member of an organizer's substitution-group is NOT
            // itself an organizer.
            Assert.That(Organizers.IsOrganizer(ControllerComponent.TypeId), Is.False);
            Assert.That(Organizers.IsOrganizer(HeatingComponent.TypeId), Is.False);
            Assert.That(Organizers.IsOrganizer(LinearComponent.TypeId), Is.False);
        }

        [Test]
        public void IsOrganizer_returns_false_for_unknown_type()
        {
            Assert.That(Organizers.IsOrganizer("ThisIsNotAnyKnownComponentType"), Is.False);
        }

        [Test]
        public void IsOrganizer_returns_false_for_null()
        {
            Assert.That(Organizers.IsOrganizer(null), Is.False);
        }

        [Test]
        public void IsOrganizer_returns_false_for_empty_string()
        {
            Assert.That(Organizers.IsOrganizer(string.Empty), Is.False);
        }
    }
}
