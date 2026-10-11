// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using MTConnect.Devices.Configurations;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Enums
{
    // Version-gated enum-arm assertions. Each fixture pins a specific
    // enum-value addition against its introducing MTConnect Standard
    // version.
    //
    //   - XMI: https://github.com/mtconnect/mtconnect_sysml_model tag list
    //          — every enum in this file traces to a UML enum extension in
    //          a specific v2.x tag.
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_<v>.xsd
    //          — the simpleType enumerations mirror the XMI additions.
    //   - Prose: MTConnect Standard Part_2.0_Devices/Streams — each enum
    //          extension is described in the part that owns the enum.
    //
    // Every fixture below is matrix-parameterised over
    // MTConnectVersionMatrix.All per plan Design Decision D1
    // (2026-08-19); Assume.That gates each row to versions where the arm
    // shipped.
    /// <summary>Pins the behavior expressed by the test name: enum arm tests.</summary>
    [TestFixture]
    public class EnumArmTests
    {
        // Source: XMI v2.6 enum `MediaTypeEnum` member `QIF_MBD`.
        // XSD v2.6 lists QIF_MBD inside the MediaType simpleType
        // enumeration. Prose Part_3.0_Devices_v2.6 section 4.7.2.5
        // introduces "ISO 10303 QIF model-based design" as the rationale.
        /// <summary>Pins the behavior expressed by the test name: media type q i f m b d value present.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void MediaType_QIF_MBD_value_present(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "MediaType.QIF_MBD was introduced in MTConnect v2.6.");

            Assert.That(Enum.IsDefined(typeof(MediaType), "QIF_MBD"), Is.True);
        }
    }
}
