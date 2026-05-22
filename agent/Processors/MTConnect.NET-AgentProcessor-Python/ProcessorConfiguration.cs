// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Processors
{
    /// <summary>
    /// Configuration shape for the Python agent processor. Bound from
    /// the <c>processors.python</c> section of <c>agent.config.yaml</c>
    /// and consumed by <see cref="Processor"/>.
    /// </summary>
    public class ProcessorConfiguration
    {
        /// <summary>
        /// Filesystem directory containing the Python scripts the
        /// processor should execute. Each <c>*.py</c> file in the
        /// directory is loaded at startup.
        /// </summary>
        [JsonPropertyName("directory")]
        public string Directory { get; set; }
    }
}
