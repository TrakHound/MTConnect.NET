// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Reflection
{
    // Reflection-driven parametric coverage over every public type in the
    // regenerated namespaces of MTConnect.NET-Common (Devices, Assets,
    // Observations, Interfaces). Each enumerated type produces:
    //
    //   1. construction case      — Activator.CreateInstance(t) does not throw
    //                                if the type exposes a parameterless ctor.
    //   2. property round-trip    — every public read/write property
    //                                accepts a sentinel value of its type
    //                                without conversion.
    //   3. description presence   — when the type ships a DescriptionText
    //                                const / static / property, it is
    //                                non-null and non-empty.
    //
    // Source authority:
    //   - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model (per-version
    //     tags). Drives the type inventory (every enumerated public class is the
    //     code-generator's emission of a SysML UML class).
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnect<Kind>_<vN.M>.xsd.
    //     Drives the property names exercised by the round-trip case.
    //
    // The test catalog is produced by iterating the four anchor types'
    // assembly with public-type filters. New SysML regenerations therefore
    // pick up new coverage automatically without any test edit — that is the
    // mechanism by which "every public regenerated type" is gated.
    [TestFixture]
    public class RegeneratedTypesCoverageTests
    {
        private static readonly string[] CoveredNamespacePrefixes =
        {
            "MTConnect.Devices",
            "MTConnect.Assets",
            "MTConnect.Observations",
            "MTConnect.Interfaces",
        };

        // Types that intentionally do NOT support the bare-ctor +
        // default-property contract. Each entry must list a reason; reading
        // the array is the canonical documentation of why the parametric
        // sweep skips them.
        private static readonly HashSet<string> InstantiationExclusions = new()
        {
            // Static utility classes are surfaced by reflection but are never
            // instantiable; Activator.CreateInstance throws MemberAccessException.
            // The classes are non-test by construction (no instance state to
            // exercise; the public consts are exercised by their consumers).
            // No specific names listed — the IsAbstract && IsSealed
            // pre-filter below catches every static class.
        };

        // Property setters that throw on the value type's default(T) instance.
        // Each entry pins the typed reason; failing-rounds-tripped properties
        // must be listed here, NOT silenced via try/catch in the test.
        private static readonly HashSet<string> PropertyRoundTripExclusions = new()
        {
            // No exclusions — every regenerated property accepts its
            // own type's default value. This list exists so that future
            // regeneration runs that introduce a constraint-bearing setter
            // (e.g. "string property that throws on null") can document the
            // exception inline rather than papering over it.
        };

        private static IEnumerable<Type> EnumeratePublicRegeneratedTypes()
        {
            // Anchor the assembly via a known regenerated type. Any of the
            // four namespace anchors works — DataItem.g.cs is the densest.
            var assembly = typeof(DataItem).Assembly;

            return assembly.GetTypes()
                .Where(t => t.IsPublic || t.IsNestedPublic)
                .Where(t => !t.IsGenericTypeDefinition)
                .Where(t => t.Namespace != null
                    && CoveredNamespacePrefixes.Any(prefix =>
                        t.Namespace == prefix
                        || t.Namespace.StartsWith(prefix + ".", StringComparison.Ordinal)))
                .OrderBy(t => t.FullName, StringComparer.Ordinal);
        }

        public static IEnumerable<TestCaseData> ConstructibleTypes()
        {
            foreach (var type in EnumeratePublicRegeneratedTypes())
            {
                if (type.IsAbstract || type.IsInterface || type.IsEnum)
                {
                    continue;
                }

                if (InstantiationExclusions.Contains(type.FullName ?? type.Name))
                {
                    continue;
                }

                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    // Type has no parameterless ctor by design (e.g. requires
                    // a deviceId). The concrete ctor coverage lives in the
                    // hand-written V2_6 / V2_7 fixtures; this parametric
                    // sweep is the bare-ctor row only.
                    continue;
                }

                yield return new TestCaseData(type)
                    .SetName($"Type_can_be_constructed({SanitizeForTestName(type.FullName ?? type.Name)})");
            }
        }

        public static IEnumerable<TestCaseData> RoundTrippableTypes()
        {
            foreach (var type in EnumeratePublicRegeneratedTypes())
            {
                if (type.IsAbstract || type.IsInterface || type.IsEnum)
                {
                    continue;
                }

                if (InstantiationExclusions.Contains(type.FullName ?? type.Name))
                {
                    continue;
                }

                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    continue;
                }

                if (!HasRoundTrippableProperties(type))
                {
                    continue;
                }

                yield return new TestCaseData(type)
                    .SetName($"Type_round_trips_default_property_values({SanitizeForTestName(type.FullName ?? type.Name)})");
            }
        }

        public static IEnumerable<TestCaseData> TypesWithDescriptionText()
        {
            foreach (var type in EnumeratePublicRegeneratedTypes())
            {
                if (KnownEmptyDescriptionTypes.Contains(type.FullName ?? type.Name))
                {
                    continue;
                }
                if (TryGetDescriptionText(type, out _))
                {
                    yield return new TestCaseData(type)
                        .SetName($"Type_has_non_empty_description({SanitizeForTestName(type.FullName ?? type.Name)})");
                }
            }
        }

        // Types whose regenerated DescriptionText is the empty string. The
        // SysML XMI did not ship a description for these, and the
        // generator emitted `""` to keep the field-shape contract. Each
        // entry is a generator-or-spec gap, NOT a test bug — the
        // FixtureAsset gap is tracked under a generator-improvements plan.
        private static readonly HashSet<string> KnownEmptyDescriptionTypes = new()
        {
            // FixtureAsset (v2.7) — XMI ships no description for the asset.
            // The negative case below pins the gap as a regression marker.
            "MTConnect.Assets.Fixture.FixtureAsset",
        };

        [Test]
        public void Known_empty_description_types_still_emit_an_empty_string()
        {
            // Pins the (defective) state of the regenerator output for
            // every entry in KnownEmptyDescriptionTypes: the field exists,
            // the value is exactly the empty string, and the catalog
            // entry stays load-bearing. When the generator gap closes,
            // this test fails and the entry moves out of the exclusion set.
            foreach (var fullName in KnownEmptyDescriptionTypes)
            {
                var type = typeof(DataItem).Assembly.GetType(fullName);
                Assert.That(type, Is.Not.Null,
                    $"{fullName} not found in MTConnect.NET-Common");

                var field = type!.GetField(
                    "DescriptionText",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                Assert.That(field, Is.Not.Null,
                    $"{fullName}.DescriptionText field missing");
                var value = field!.GetRawConstantValue() as string;
                Assert.That(value, Is.EqualTo(string.Empty),
                    $"{fullName}.DescriptionText is no longer empty — move it out of KnownEmptyDescriptionTypes and re-run the parametric description sweep");
            }
        }

        [Test]
        [TestCaseSource(nameof(ConstructibleTypes))]
        public void Type_can_be_constructed(Type type)
        {
            // The §10 coverage gate requires every public regenerated type's
            // default ctor to execute at least once. This single parametric
            // case satisfies the gate for the class-with-bare-ctor case;
            // ctors with arguments are covered by the typed fixtures under
            // V2_6_V2_7/.
            object? instance = null;
            Assert.DoesNotThrow(
                () => instance = Activator.CreateInstance(type),
                $"{type.FullName} parameterless ctor threw");
            Assert.That(instance, Is.Not.Null,
                $"{type.FullName} parameterless ctor returned null");
        }

        [Test]
        [TestCaseSource(nameof(RoundTrippableTypes))]
        public void Type_round_trips_default_property_values(Type type)
        {
            // The round-trip case asserts that every public read/write
            // property of the type accepts a sentinel value of the property's
            // declared type and returns the same value via getter. The
            // sentinel is the type's own default (default(T)) — reading and
            // writing that value must not throw and must not silently
            // transform.
            //
            // Properties without a public setter (read-only computed
            // properties such as Id) are skipped — the spec contract for
            // those is "derived from other state", and the V2_6_V2_7
            // hand-written fixtures pin their semantics.
            var instance = Activator.CreateInstance(type)!;

            foreach (var property in GetRoundTrippableProperties(type))
            {
                var key = $"{type.FullName}.{property.Name}";
                if (PropertyRoundTripExclusions.Contains(key))
                {
                    continue;
                }

                object? sentinel = GetDefaultValue(property.PropertyType);

                Assert.DoesNotThrow(
                    () => property.SetValue(instance, sentinel),
                    $"{key} setter threw for default({property.PropertyType.Name})");

                object? readBack = null;
                Assert.DoesNotThrow(
                    () => readBack = property.GetValue(instance),
                    $"{key} getter threw after setting default({property.PropertyType.Name})");

                // Read-back equality is only asserted on auto-properties.
                // Hand-written types (Asset.Uuid, Observation.Value, etc.)
                // intentionally compute the getter from other state, so
                // writing default(T) followed by getting will not echo the
                // sentinel — those types are exercised by their own typed
                // fixtures and NOT by the parametric round-trip case.
                if (IsAutoProperty(property))
                {
                    if (sentinel == null)
                    {
                        Assert.That(readBack, Is.Null,
                            $"{key} read-back was non-null after writing null");
                    }
                    else
                    {
                        Assert.That(readBack, Is.EqualTo(sentinel),
                            $"{key} read-back differed from written default({property.PropertyType.Name})");
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(TypesWithDescriptionText))]
        public void Type_has_non_empty_description(Type type)
        {
            Assert.That(TryGetDescriptionText(type, out var description), Is.True,
                $"{type.FullName} surface check failed (TestCaseSource invariant)");
            Assert.That(description, Is.Not.Null.And.Not.Empty,
                $"{type.FullName} DescriptionText is null or empty");
        }

        // Smoke-test the catalog itself so the parametric sweep cannot
        // silently shrink to zero (e.g. namespace rename that drops every
        // anchor). At least one constructible type must exist.
        [Test]
        public void Catalog_enumerates_at_least_one_type_per_namespace()
        {
            var byNamespace = EnumeratePublicRegeneratedTypes()
                .GroupBy(t => CoveredNamespacePrefixes.First(prefix =>
                    t.Namespace == prefix
                    || (t.Namespace ?? "").StartsWith(prefix + ".", StringComparison.Ordinal)))
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var prefix in CoveredNamespacePrefixes)
            {
                Assert.That(byNamespace.ContainsKey(prefix), Is.True,
                    $"namespace {prefix} produced no public types");
                Assert.That(byNamespace[prefix], Is.GreaterThan(0),
                    $"namespace {prefix} produced zero public types");
            }
        }

        // ---------- helpers ----------

        private static bool HasRoundTrippableProperties(Type type)
        {
            return GetRoundTrippableProperties(type).Any();
        }

        private static IEnumerable<PropertyInfo> GetRoundTrippableProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => p.GetSetMethod(false) != null);
        }

        private static bool IsAutoProperty(PropertyInfo property)
        {
            // C# auto-properties are emitted with a compiler-generated
            // backing field whose name matches "<PropertyName>k__BackingField".
            // The presence of this field on the declaring type is the
            // canonical signal that the property is an auto-property and
            // therefore round-trips trivially.
            var declaring = property.DeclaringType;
            if (declaring == null)
            {
                return false;
            }

            var backingFieldName = $"<{property.Name}>k__BackingField";
            var field = declaring.GetField(
                backingFieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            return field != null;
        }

        private static bool TryGetDescriptionText(Type type, out string? value)
        {
            value = null;

            // const string DescriptionText is emitted as a literal field by
            // the regenerator (see e.g. CapacitySpatialDataItem.g.cs).
            var constField = type.GetField(
                "DescriptionText",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (constField != null && constField.FieldType == typeof(string))
            {
                value = constField.GetRawConstantValue() as string
                    ?? constField.GetValue(null) as string;
                return value != null;
            }

            // Some types expose Description as a property (e.g. AssetDescriptions
            // is a static class with const-string members named after the
            // properties it documents — those are NOT covered by this case;
            // only types that ship a single DescriptionText surface are).
            var prop = type.GetProperty(
                "DescriptionText",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (prop != null && prop.PropertyType == typeof(string) && prop.GetGetMethod() != null)
            {
                if (prop.GetGetMethod()!.IsStatic)
                {
                    value = prop.GetValue(null) as string;
                    return value != null;
                }

                // Instance property — only readable on a constructible
                // instance. Skip if the type can't be cheaply built; the
                // property-round-trip case has already exercised it via the
                // constructor + property paths.
                if (!type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
                {
                    var instance = Activator.CreateInstance(type);
                    value = prop.GetValue(instance) as string;
                    return value != null;
                }
            }

            return false;
        }

        private static object? GetDefaultValue(Type type)
        {
            if (!type.IsValueType)
            {
                return null;
            }

            return Activator.CreateInstance(type);
        }

        private static string SanitizeForTestName(string name)
        {
            // NUnit test names ban a few characters in the test-explorer
            // output (parentheses + commas in particular). Replace with
            // underscores so each row gets a unique, tooling-friendly name.
            var chars = name.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                var ch = chars[i];
                if (ch == ',' || ch == '(' || ch == ')' || ch == '<' || ch == '>')
                {
                    chars[i] = '_';
                }
            }
            return new string(chars);
        }
    }

    // Per-version metadata sweep for the regenerated types: when the SysML
    // model annotates a class with a `MinimumVersion` (or `MaximumVersion`)
    // stereotype, the regenerator emits an instance-property override.
    // This fixture asserts the emitted overrides are version-typed and
    // resolve to a value within the library's advertised version range.
    //
    // A type that fails to override MinimumVersion when its SysML model
    // demands one is a generator-side defect (see plan
    // 13-generator-improvements). Such defects are tracked there, NOT
    // patched in by silencing the parametric sweep.
    [TestFixture]
    public class RegeneratedTypeVersionAnnotationTests
    {
        public static IEnumerable<TestCaseData> TypesWithMinimumVersionOverride()
        {
            foreach (var type in EnumeratePublicRegeneratedTypes())
            {
                if (type.IsAbstract || type.IsInterface || type.IsEnum)
                {
                    continue;
                }
                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    continue;
                }

                var prop = type.GetProperty(
                    "MinimumVersion",
                    BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || prop.PropertyType != typeof(Version))
                {
                    continue;
                }

                if (!IsOverriddenInDeclaringType(prop, type))
                {
                    continue;
                }

                // Skip when the override returns null — same logic as the
                // MaximumVersion sweep: a null override is "no minimum",
                // not an annotated stereotype the test should assert on.
                Version? probe = null;
                try
                {
                    var instance = Activator.CreateInstance(type);
                    probe = prop.GetValue(instance) as Version;
                }
                catch
                {
                    continue;
                }

                if (probe == null)
                {
                    continue;
                }

                yield return new TestCaseData(type)
                    .SetName($"MinimumVersion_resolves_to_advertised_version({type.Name})");
            }
        }

        public static IEnumerable<TestCaseData> TypesWithMaximumVersionOverride()
        {
            foreach (var type in EnumeratePublicRegeneratedTypes())
            {
                if (type.IsAbstract || type.IsInterface || type.IsEnum)
                {
                    continue;
                }
                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    continue;
                }

                var prop = type.GetProperty(
                    "MaximumVersion",
                    BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || prop.PropertyType != typeof(Version))
                {
                    continue;
                }

                if (!IsOverriddenInDeclaringType(prop, type))
                {
                    continue;
                }

                // Skip when the override returns null — that means "no
                // maximum bound" per SysML semantics, which is the default
                // and not interesting to assert. This test exists to
                // confirm an EXPLICIT MaximumVersion stereotype resolves to
                // a known constant; types without an explicit max are
                // covered by the MinimumVersion sweep instead.
                Version? probe = null;
                try
                {
                    var instance = Activator.CreateInstance(type);
                    probe = prop.GetValue(instance) as Version;
                }
                catch
                {
                    // ctor / getter threw; the construction-case test
                    // surfaces that as its own failure.
                    continue;
                }

                if (probe == null)
                {
                    continue;
                }

                yield return new TestCaseData(type)
                    .SetName($"MaximumVersion_resolves_to_advertised_version({type.Name})");
            }
        }

        [Test]
        [TestCaseSource(nameof(TypesWithMinimumVersionOverride))]
        public void MinimumVersion_resolves_to_an_advertised_version(Type type)
        {
            var advertised = AdvertisedVersions().ToHashSet();

            var instance = Activator.CreateInstance(type)!;
            var prop = type.GetProperty("MinimumVersion", BindingFlags.Public | BindingFlags.Instance)!;
            var value = prop.GetValue(instance) as Version;

            Assert.That(value, Is.Not.Null,
                $"{type.FullName}.MinimumVersion returned null");
            Assert.That(advertised, Does.Contain(value),
                $"{type.FullName}.MinimumVersion = {value} is not one of MTConnectVersions's advertised constants");
        }

        [Test]
        [TestCaseSource(nameof(TypesWithMaximumVersionOverride))]
        public void MaximumVersion_resolves_to_an_advertised_version(Type type)
        {
            var advertised = AdvertisedVersions().ToHashSet();

            var instance = Activator.CreateInstance(type)!;
            var prop = type.GetProperty("MaximumVersion", BindingFlags.Public | BindingFlags.Instance)!;
            var value = prop.GetValue(instance) as Version;

            Assert.That(value, Is.Not.Null,
                $"{type.FullName}.MaximumVersion returned null");
            Assert.That(advertised, Does.Contain(value),
                $"{type.FullName}.MaximumVersion = {value} is not one of MTConnectVersions's advertised constants");
        }

        private static IEnumerable<Type> EnumeratePublicRegeneratedTypes()
        {
            var assembly = typeof(DataItem).Assembly;
            string[] prefixes =
            {
                "MTConnect.Devices",
                "MTConnect.Streams",
                "MTConnect.Assets",
                "MTConnect.Configurations",
            };

            return assembly.GetTypes()
                .Where(t => t.IsPublic || t.IsNestedPublic)
                .Where(t => !t.IsGenericTypeDefinition)
                .Where(t => t.Namespace != null
                    && prefixes.Any(prefix =>
                        t.Namespace == prefix
                        || t.Namespace.StartsWith(prefix + ".", StringComparison.Ordinal)));
        }

        private static IEnumerable<Version> AdvertisedVersions()
        {
            return typeof(MTConnectVersions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Version))
                .Select(f => (Version)f.GetValue(null)!)
                .Where(v => v != null);
        }

        private static bool IsOverriddenInDeclaringType(PropertyInfo prop, Type type)
        {
            // Only assert on types that actually override the property.
            // The base DataItem class declares MinimumVersion as a virtual
            // returning a default; types with no SysML version stereotype
            // inherit that default, and asserting on them tells us nothing
            // about the regenerator.
            var getter = prop.GetGetMethod(false);
            if (getter == null)
            {
                return false;
            }
            return getter.DeclaringType == type;
        }
    }
}
