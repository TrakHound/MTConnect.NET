// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using MTConnect.Devices.Components;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Devices.Components
{
    // Version-gated shape assertions for the Component subclasses introduced
    // across the v2.6 and v2.7 MTConnect Standard releases.
    //
    //   - XMI: https://github.com/mtconnect/mtconnect_sysml_model tags
    //          v2.6 (SHA 08185447bf86…) — CuttingTorch, Electrode
    //          v2.7 (SHA 25796ac591bb…) — PinTool, ToolHolder
    //          UML classes under Device Information Model > Components.
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.6.xsd
    //                                                 MTConnectDevices_2.7.xsd
    //          (each TypeId appears in the ComponentType enumeration.)
    //   - Prose: MTConnect Standard Part_2.0_Devices_v2.6 section 3.4.18
    //          "CuttingTorch" / section 3.4.21 "Electrode";
    //          Part_2.0_Devices_v2.7 section 7 "Component types" (PinTool,
    //          ToolHolder).
    //
    // Every fixture below is matrix-parameterised over
    // MTConnectVersionMatrix.All per plan Design Decision D1
    // (2026-08-19); Assume.That gates each assertion to versions where the
    // spec introduced the type. Rows below the floor surface as
    // Inconclusive in the test explorer, which is the D1-ruled shape for
    // "gated out" versus "ran and passed".
    /// <summary>Pins the behavior expressed by the test name: component tests.</summary>
    [TestFixture]
    public class ComponentTests
    {
        // Source: XMI v2.6 UML `CuttingTorch` (Component Types); XSD v2.6
        // `<xs:element name="CuttingTorch">`.
        /// <summary>Pins the behavior expressed by the test name: cutting torch component constructs with correct type.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void CuttingTorchComponent_constructs_with_correct_type(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "CuttingTorch was introduced in MTConnect v2.6.");

            var c = new CuttingTorchComponent();
            Assert.That(c.Type, Is.EqualTo("CuttingTorch"));
            Assert.That(c.Name, Is.Null);
            Assert.That(CuttingTorchComponent.TypeId, Is.EqualTo("CuttingTorch"));
            Assert.That(CuttingTorchComponent.NameId, Is.EqualTo("cuttingTorch"));
        }

        // Source: XMI v2.6 UML `Electrode` (Component Types); XSD v2.6
        // `<xs:element name="Electrode">`.
        /// <summary>Pins the behavior expressed by the test name: electrode component constructs with correct type.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void ElectrodeComponent_constructs_with_correct_type(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "Electrode was introduced in MTConnect v2.6.");

            var c = new ElectrodeComponent();
            Assert.That(c.Type, Is.EqualTo("Electrode"));
            Assert.That(c.Name, Is.Null);
            Assert.That(ElectrodeComponent.TypeId, Is.EqualTo("Electrode"));
            Assert.That(ElectrodeComponent.NameId, Is.EqualTo("electrode"));
        }

        // Source: XMI v2.7 UML `PinTool` (Component Types); XSD v2.7
        // ComponentType enumeration value `PinTool`.
        /// <summary>Pins the behavior expressed by the test name: pin tool component constructs with correct type.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void PinToolComponent_constructs_with_correct_type(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "PinTool was introduced in MTConnect v2.7.");

            var c = new PinToolComponent();
            Assert.That(c.Type, Is.EqualTo("PinTool"));
            Assert.That(c.Name, Is.Null);
            Assert.That(PinToolComponent.TypeId, Is.EqualTo("PinTool"));
            Assert.That(PinToolComponent.NameId, Is.EqualTo("pinTool"));
        }

        // Source: XMI v2.7 UML `ToolHolder` (Component Types); XSD v2.7
        // ComponentType enumeration value `ToolHolder`.
        /// <summary>Pins the behavior expressed by the test name: tool holder component constructs with correct type.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void ToolHolderComponent_constructs_with_correct_type(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "ToolHolder was introduced in MTConnect v2.7.");

            var c = new ToolHolderComponent();
            Assert.That(c.Type, Is.EqualTo("ToolHolder"));
            Assert.That(c.Name, Is.Null);
            Assert.That(ToolHolderComponent.TypeId, Is.EqualTo("ToolHolder"));
            Assert.That(ToolHolderComponent.NameId, Is.EqualTo("toolHolder"));
        }
    }
}
