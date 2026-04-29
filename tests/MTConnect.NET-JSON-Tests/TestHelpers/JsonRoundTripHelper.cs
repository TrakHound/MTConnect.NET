// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;

namespace MTConnect.NET_JSON_Tests.TestHelpers
{
    /// <summary>
    /// Small JSON round-trip primitives. Each leaf class exposes
    /// <c>JsonLeaf(IInterface)</c> and <c>ToLeaf()</c>; the helper just
    /// glues System.Text.Json into the boilerplate.
    /// </summary>
    internal static class JsonRoundTripHelper
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            // The wire-format classes carry [JsonPropertyName] attributes
            // for camelCase keys; default options honour those without
            // additional configuration.
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

        public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    }
}
