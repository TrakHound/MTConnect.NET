// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using MTConnect.NET_DocsGen;
using NUnit.Framework;

namespace MTConnect.NET_Docs_Tests;

/// <summary>
/// Direct unit coverage FLOOR for <c>ConfigRenderer.RenderType</c> — the
/// private helper introduced by PR #219 (build/MTConnect.NET-DocsGen/Renderers.cs:402)
/// that upgrades the plain backtick-wrapped Type column to a markdown
/// link for known enum / class types with an authored docfx API page.
///
/// The existing <c>DocsReferenceGenerationTests.Configuration_Page_Is_In_Sync_With_Source</c>
/// exercises <see cref="ConfigRenderer.Render"/> against the whole
/// live inventory — but only asserts final-file equality; it does not
/// pin the specific mapping table entries. If a maintainer typos
/// <c>/api/MTConnect.Agents.DeviceValidationLevel</c> to
/// <c>/api/MTConnect.Agents.DeviceValidation</c>, the sync test would
/// still pass so long as the on-disk markdown was regenerated with the
/// typo. This fixture pins the mapping table directly at both branches
/// (mapped-type → linked backtick; unmapped-type → plain backtick) so
/// the branches survive independently of the docs regenerator.
/// </summary>
[TestFixture]
[Category("ConfigRendererTypeMapping")]
public class ConfigRendererTypeMappingTests
{
    // Mapped-type branch — DeviceValidationLevel.

    /// <summary>Pins that a property typed <c>DeviceValidationLevel</c> renders as a markdown link into the api namespace, not a plain backtick.</summary>
    [Test]
    public void RenderType_maps_DeviceValidationLevel_to_api_link()
    {
        var md = RenderOneProperty("Config1", "DeviceValidationLevel");

        Assert.That(md, Does.Contain("[`DeviceValidationLevel`](/api/MTConnect.Agents.DeviceValidationLevel)"),
            "DeviceValidationLevel must render as a docfx-linked backtick — the RenderType mapping table is the single source of truth for this href.");
    }

    // Mapped-type branch — InputValidationLevel.

    /// <summary>Pins that a property typed <c>InputValidationLevel</c> renders as a markdown link into the api namespace.</summary>
    [Test]
    public void RenderType_maps_InputValidationLevel_to_api_link()
    {
        var md = RenderOneProperty("Config1", "InputValidationLevel");

        Assert.That(md, Does.Contain("[`InputValidationLevel`](/api/MTConnect.Agents.InputValidationLevel)"),
            "InputValidationLevel must render as a docfx-linked backtick — pins the second entry in the RenderType mapping table.");
    }

    // Unmapped-type branch — fallback to plain backtick.

    /// <summary>Pins the fallback branch: an unmapped type renders as a plain backtick-fenced type, not a link.</summary>
    [Test]
    public void RenderType_unmapped_type_falls_back_to_plain_backtick()
    {
        var md = RenderOneProperty("Config1", "int");

        Assert.That(md, Does.Contain("| `int` |"),
            "An unmapped type must fall back to the plain backtick-fenced shape the renderer previously emitted for every type.");
        Assert.That(md, Does.Not.Contain("[`int`]"),
            "An unmapped type must NOT be linked into the /api/ namespace.");
    }

    /// <summary>Pins that adding an unrelated type name to the mapping table does not accidentally trigger substring matches — the map is keyed on full type name equality.</summary>
    [Test]
    public void RenderType_substring_of_mapped_type_is_not_linked()
    {
        // "Device" is a substring of "DeviceValidationLevel" but should
        // NOT be mapped — the RenderType dictionary is an exact-string
        // lookup, not a prefix match.
        var md = RenderOneProperty("Config1", "Device");

        Assert.That(md, Does.Contain("| `Device` |"),
            "A type whose name is a substring of a mapped entry must render as the plain backtick fallback — the mapping table is exact-string, not prefix-match.");
        Assert.That(md, Does.Not.Contain("[`Device`]"),
            "A substring of a mapped type must NOT be linked.");
    }

    /// <summary>Pins that the pipe character in a type name is escaped so the markdown table row does not lose columns — matches the Escape helper called by RenderType and by the surrounding Render loop.</summary>
    [Test]
    public void RenderType_escapes_pipe_in_type_name()
    {
        var md = RenderOneProperty("Config1", "Foo|Bar");

        Assert.That(md, Does.Contain("`Foo\\|Bar`"),
            "A pipe in the type name must be escaped so it does not close the markdown table cell early.");
    }

    // -----------------------------------------------------------------
    // Helper — build a minimal ConfigClassInfo with one property, run
    // the renderer, return the markdown for assertion.
    // -----------------------------------------------------------------

    private static string RenderOneProperty(string typeName, string propertyType)
    {
        var property = new ConfigPropertyInfo(
            Name: "Prop",
            Type: propertyType,
            SerialisedKey: "prop",
            Summary: "summary",
            DefaultLiteral: null);

        var cls = new ConfigClassInfo(
            TypeName: typeName,
            Namespace: "MTConnect.Test",
            FileRelativePath: "libraries/test/Config1.cs",
            Summary: "test class",
            Properties: new List<ConfigPropertyInfo> { property });

        return ConfigRenderer.Render(new List<ConfigClassInfo> { cls });
    }
}
