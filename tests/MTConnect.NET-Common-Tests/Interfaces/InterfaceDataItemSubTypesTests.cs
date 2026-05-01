// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using MTConnect.Interfaces;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Interfaces
{
    /// <summary>
    /// Pins the class-level contract that every Interface DataItem
    /// exposes a nested <c>SubTypes</c> enum carrying at minimum
    /// <c>REQUEST</c> and <c>RESPONSE</c> members. Per the MTConnect
    /// SysML model, every Interface DataItem class declares two
    /// subtype classes named <c>&lt;Name&gt;.Request</c> and
    /// <c>&lt;Name&gt;.Response</c>; the C# generator must emit a
    /// <c>SubTypes</c> enum mirroring those subtype literals so
    /// consumers can construct the DataItem with a typed subtype
    /// rather than passing an opaque string.
    ///
    /// Sources:
    /// - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    ///   v2.7. Each of the ten interface-event classes below carries
    ///   an immediate sub-class named <c>&lt;Name&gt;.Request</c> and
    ///   <c>&lt;Name&gt;.Response</c> that generalize from the parent
    ///   interface DataItem (e.g. CloseChuck.Request, CloseChuck.Response
    ///   at XMI lines 48715 and 48736).
    /// - Reference implementation: cppagent v2.7.0.7 emits these
    ///   DataItems with a SubType of REQUEST or RESPONSE on the
    ///   resulting observation element.
    /// </summary>
    [TestFixture]
    [Category("InterfaceDataItemSubTypes")]
    public class InterfaceDataItemSubTypesTests
    {
        private static readonly Type[] _interfaceDataItemTypes = new[]
        {
            typeof(CloseChuckDataItem),
            typeof(CloseDoorDataItem),
            typeof(OpenChuckDataItem),
            typeof(OpenDoorDataItem),
            typeof(MaterialChangeDataItem),
            typeof(MaterialFeedDataItem),
            typeof(MaterialLoadDataItem),
            typeof(MaterialRetractDataItem),
            typeof(MaterialUnloadDataItem),
            typeof(PartChangeDataItem)
        };

        private static System.Collections.Generic.IEnumerable<Type> InterfaceDataItemTypes => _interfaceDataItemTypes;

        [Test]
        [TestCaseSource(nameof(InterfaceDataItemTypes))]
        public void Interface_DataItem_Exposes_SubTypes_Enum(Type interfaceDataItemType)
        {
            var subTypesEnum = interfaceDataItemType.GetNestedType("SubTypes");

            Assert.That(subTypesEnum, Is.Not.Null,
                $"{interfaceDataItemType.Name} must expose a nested SubTypes type");
            Assert.That(subTypesEnum!.IsEnum, Is.True,
                $"{interfaceDataItemType.Name}.SubTypes must be an enum");
        }

        [Test]
        [TestCaseSource(nameof(InterfaceDataItemTypes))]
        public void Interface_DataItem_SubTypes_Contains_Request_And_Response(Type interfaceDataItemType)
        {
            var subTypesEnum = interfaceDataItemType.GetNestedType("SubTypes");
            Assert.That(subTypesEnum, Is.Not.Null,
                $"{interfaceDataItemType.Name} must expose a nested SubTypes type");

            var names = Enum.GetNames(subTypesEnum!);

            Assert.That(names, Does.Contain("REQUEST"),
                $"{interfaceDataItemType.Name}.SubTypes must contain REQUEST");
            Assert.That(names, Does.Contain("RESPONSE"),
                $"{interfaceDataItemType.Name}.SubTypes must contain RESPONSE");
        }
    }
}
