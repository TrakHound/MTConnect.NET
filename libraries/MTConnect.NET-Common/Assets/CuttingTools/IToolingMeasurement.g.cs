// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained scalar value associated with a cutting tool.
    /// </summary>
    public interface IToolingMeasurement : IMeasurement
    {
        /// <summary>
        /// Shop specific code for the measurement. ISO 13399 codes **MAY** be used for these codes as well. code values.
        /// </summary>
        string Code { get; }
    }
}