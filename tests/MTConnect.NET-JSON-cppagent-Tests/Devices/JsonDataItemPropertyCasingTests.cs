// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Json;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MTConnect.NET_JSON_cppagent_Tests.Devices
{
    /// <summary>
    /// Pins the cppagent JSON wire-shape casing convention for
    /// <see cref="JsonDataItem"/>: complex object members are PascalCase,
    /// scalar attribute members are camelCase. The cppagent reference
    /// implementation distinguishes the two so consumers can tell at a
    /// glance whether a key carries a nested object or a scalar.
    /// </summary>
    [TestFixture]
    [Category("WireShape")]
    public class JsonDataItemPropertyCasingTests
    {
        // CLR property name -> expected JSON key.
        private static readonly (string Clr, string Json)[] PascalCaseObjects = new[]
        {
            ("Source", "Source"),
            ("Constraints", "Constraints"),
            ("Filters", "Filters"),
            ("Definition", "Definition"),
            ("Relationships", "Relationships"),
        };

        private static readonly (string Clr, string Json)[] CamelCaseScalars = new[]
        {
            ("DataItemCategory", "category"),
            ("Id", "id"),
            ("Type", "type"),
        };

        private static string GetJsonName(string clrPropertyName)
        {
            var prop = typeof(JsonDataItem).GetProperty(
                clrPropertyName,
                BindingFlags.Public | BindingFlags.Instance);
            Assert.That(prop, Is.Not.Null,
                $"Property {clrPropertyName} must exist on JsonDataItem.");
            var attribute = prop!.GetCustomAttribute<JsonPropertyNameAttribute>();
            Assert.That(attribute, Is.Not.Null,
                $"Property {clrPropertyName} must carry a [JsonPropertyName] attribute.");
            return attribute!.Name;
        }

        [TestCaseSource(nameof(PascalCaseObjects))]
        public void Complex_object_property_uses_PascalCase_json_key((string Clr, string Json) entry)
        {
            Assert.That(GetJsonName(entry.Clr), Is.EqualTo(entry.Json),
                "Complex object members on JsonDataItem must remain PascalCase to match the cppagent JSON wire shape.");
        }

        [TestCaseSource(nameof(CamelCaseScalars))]
        public void Scalar_attribute_property_uses_camelCase_json_key((string Clr, string Json) entry)
        {
            Assert.That(GetJsonName(entry.Clr), Is.EqualTo(entry.Json),
                "Scalar attribute members on JsonDataItem must remain camelCase to match the cppagent JSON wire shape.");
        }
    }
}
