// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// Pins the `Controller` carve-out: even though the SysML model treats
// `Controller` as a member of the `System` substitution-group, this
// library deliberately routes `Controller` through its own dedicated
// `Controllers` organizer (see `Device.AddComponent()` near the
// `organizerType != ControllersComponent.TypeId` guard). The carve-out
// must be expressed in the source-of-truth (`Organizers._systems`) and
// not as a side-effect of the inner-list iteration order inside
// `GetOrganizerType` — otherwise reordering the implementation silently
// regresses the public `Organizers.Systems` surface and the auto-wrap
// behavior in `Device.AddComponent()`.

using System.Linq;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.Tests.Common.SystemsOrganizer
{
    /// <summary>Pins the behaviour expressed by the test name: organizers controller carve out tests.</summary>
    [TestFixture]
    [Category("OrganizersControllerCarveOut")]
    public class OrganizersControllerCarveOutTests
    {
        /// <summary>Pins the behaviour expressed by the test name: organizers systems does not list controller component.</summary>
        [Test]
        public void Organizers_Systems_does_not_list_ControllerComponent()
        {
            // `Controller` has its own `Controllers` organizer; including it
            // in `Organizers.Systems` would advertise two organizers for the
            // same component type and make the resolution order-dependent.
            Assert.That(Organizers.Systems, Has.No.Member(ControllerComponent.TypeId),
                $"`Organizers.Systems` must NOT enumerate `{ControllerComponent.TypeId}` — " +
                $"it has its own `{ControllersComponent.TypeId}` organizer. Listing it under " +
                $"both makes `Organizers.GetOrganizerType()` order-dependent.");
        }

        /// <summary>Pins the behaviour expressed by the test name: organizers controllers lists controller component.</summary>
        [Test]
        public void Organizers_Controllers_lists_ControllerComponent()
        {
            // The complement of the carve-out: `Controller` IS the only
            // member of `Organizers.Controllers`.
            Assert.That(Organizers.Controllers, Has.Member(ControllerComponent.TypeId),
                $"`Organizers.Controllers` must enumerate `{ControllerComponent.TypeId}` so " +
                $"`Organizers.GetOrganizerType(\"{ControllerComponent.TypeId}\")` resolves to " +
                $"`{ControllersComponent.TypeId}`.");
        }

        /// <summary>Pins the behaviour expressed by the test name: get organizer type for controller resolves to controllers independent of ordering.</summary>
        [Test]
        public void GetOrganizerType_for_Controller_resolves_to_Controllers_independent_of_ordering()
        {
            // Even if the implementation is rewritten (e.g. to a dictionary
            // lookup or to scan organizer lists in a different order), the
            // public contract for `Controller` MUST remain `Controllers` —
            // the contract is independent of iteration order inside
            // `GetOrganizerType` because `Controller` belongs to exactly
            // one organizer list (`Controllers`), never `Systems`.
            Assert.That(Organizers.GetOrganizerType(ControllerComponent.TypeId),
                Is.EqualTo(ControllersComponent.TypeId),
                $"`Organizers.GetOrganizerType(\"{ControllerComponent.TypeId}\")` must always return " +
                $"`{ControllersComponent.TypeId}` — never `{SystemsComponent.TypeId}` — regardless of how " +
                $"the underlying organizer lookup is implemented.");
        }

        /// <summary>Pins the behaviour expressed by the test name: controller appears in exactly one organizer list.</summary>
        [Test]
        public void Controller_appears_in_exactly_one_organizer_list()
        {
            // Cross-cutting invariant: every component-TypeId that appears in
            // any `Organizers.*` list must appear in EXACTLY ONE of them.
            // Otherwise the mapping `member → organizer` is ambiguous and
            // resolution becomes implementation-detail-dependent.
            int matches = 0;
            if (Organizers.Adapters.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Auxiliaries.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Axes.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Controllers.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Interfaces.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Materials.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Parts.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Processes.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Resources.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Systems.Contains(ControllerComponent.TypeId)) matches++;
            if (Organizers.Structures.Contains(ControllerComponent.TypeId)) matches++;

            Assert.That(matches, Is.EqualTo(1),
                $"`{ControllerComponent.TypeId}` must appear in exactly one `Organizers.*` member " +
                $"list (Controllers). Found in {matches} lists — duplicates make " +
                $"`Organizers.GetOrganizerType()` resolution order-dependent.");
        }
    }
}
