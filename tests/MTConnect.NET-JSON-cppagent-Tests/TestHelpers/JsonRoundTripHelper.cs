// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;

namespace MTConnect.NET_JSON_cppagent_Tests.TestHelpers
{
    /// <summary>
    /// JSON round-trip primitives for the cppagent v2 dialect tests. The
    /// dialect's wire-format classes carry PascalCase
    /// <c>[JsonPropertyName]</c> attributes and rely on
    /// System.Text.Json's defaults for the round-trip.
    /// </summary>
    internal static class JsonRoundTripHelper
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

        public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    }
}
