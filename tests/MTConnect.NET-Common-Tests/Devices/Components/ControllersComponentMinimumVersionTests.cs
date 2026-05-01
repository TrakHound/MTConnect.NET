// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Devices.Components
{
    /// <summary>
    /// Pins the class-level contract that ControllersComponent reports
    /// MTConnect v2.0 as its MinimumVersion. The MTConnect SysML model
    /// introduces the <c>Controllers</c> organizer AssociationClass at
    /// v2.0 — before that release, controller groupings were ad-hoc.
    /// The generated component class must surface the v2.0 introduction
    /// year through its MinimumVersion override so version-gating
    /// callers can reject a Controllers parent on a pre-v2.0 stream.
    ///
    /// Sources:
    /// - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    ///   v2.7. The <c>Controllers</c> AssociationClass
    ///   (UML ID _19_0_3_68e0225_1648551529939_657918_1127) carries
    ///   <c>Profile:normative introduced='2.0'</c> at XMI line 68730,
    ///   pinning the v2.0 introduction year.
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.0.xsd
    ///   declares the <c>Controllers</c> element under the parent
    ///   complex type with a substitutionGroup of <c>Component</c>.
    /// </summary>
    [TestFixture]
    [Category("ControllersComponentMinimumVersion")]
    public class ControllersComponentMinimumVersionTests
    {
        [Test]
        public void Default_Constructor_Reports_Version20()
        {
            var component = new ControllersComponent();

            Assert.That(component.MinimumVersion, Is.EqualTo(MTConnectVersions.Version20));
        }
    }
}
