// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Http
{
    /// <summary>
    /// Controls how aggressively the MTConnect HTTP server validates outgoing response documents
    /// against the configured MTConnect schema version before they are written to the wire.
    /// Strict validation gives the strongest conformance guarantees at the cost of per-response
    /// CPU; the looser modes trade that for throughput.
    /// </summary>
    public enum OutputValidationLevel
    {
        /// <summary>Skip schema validation entirely; the agent emits documents as built without further checks.</summary>
        Ignore,

        /// <summary>Validate each outgoing document and log any failures, but still return the response to the client.</summary>
        Warning,

        /// <summary>Validate each outgoing document and refuse to return responses that fail validation (the client receives an error).</summary>
        Strict
    }
}
