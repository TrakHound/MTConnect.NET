// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Agents
{
    /// <summary>
    /// Controls how the Agent reacts when input data fails validation against
    /// the device model.
    /// </summary>
    public enum InputValidationLevel
    {
        /// <summary>
        /// Accept invalid input unchanged; perform no validation action.
        /// </summary>
        Ignore,

        /// <summary>
        /// Accept invalid input but emit a validation warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Drop the invalid input and continue processing the remainder.
        /// </summary>
        Remove,

        /// <summary>
        /// Reject the entire input on the first validation failure.
        /// </summary>
        Strict
    }
}