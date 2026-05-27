// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// Universal result of a validation operation in MTConnect.
    /// Used across Devices, DataItems, Compositions, Components, Observations,
    /// and Assets so a single shape carries every validation outcome the agent
    /// emits through its Invalid*Added event family.
    /// </summary>
    /// <remarks>
    /// Replaces the parallel per-domain structs that lived in
    /// MTConnect.Devices.DataItems.ValidationResult,
    /// MTConnect.Observations.ObservationValidationResult, and
    /// MTConnect.Assets.AssetValidationResult prior to v7. Carries a stable
    /// <see cref="Code"/> (machine-readable discriminator) alongside the
    /// existing human-readable <see cref="Message"/> so subscribers can
    /// branch on the failure category without parsing the message string.
    /// </remarks>
    public readonly struct ValidationResult
    {
        /// <summary>
        /// Gets whether the validation succeeded.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the machine-readable code identifying the failure category.
        /// Null or empty when <see cref="IsValid"/> is true.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the human-readable message describing the validation outcome.
        /// May be null when the validation succeeded.
        /// </summary>
        public string Message { get; }


        /// <summary>
        /// Initializes a new <see cref="ValidationResult"/> with the given
        /// validity, code, and message.
        /// </summary>
        /// <param name="isValid">Whether the validation succeeded.</param>
        /// <param name="message">Human-readable description of the outcome.</param>
        /// <param name="code">Machine-readable code identifying the failure category.</param>
        public ValidationResult(bool isValid, string message = null, string code = null)
        {
            IsValid = isValid;
            Message = message;
            Code = code;
        }


        /// <summary>
        /// Returns a <see cref="ValidationResult"/> representing a successful validation.
        /// </summary>
        public static ValidationResult Valid()
        {
            return new ValidationResult(true);
        }

        /// <summary>
        /// Returns a <see cref="ValidationResult"/> representing a failed validation
        /// with the given code and message.
        /// </summary>
        /// <param name="code">Machine-readable code identifying the failure category.</param>
        /// <param name="message">Human-readable description of the failure.</param>
        public static ValidationResult Invalid(string code, string message)
        {
            return new ValidationResult(false, message, code);
        }
    }
}
