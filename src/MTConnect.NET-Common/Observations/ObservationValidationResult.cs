// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// The result of an Observation Validation
    /// </summary>
    public struct ObservationValidationResult
    {
        public bool IsValid { get; set; }

        public string Message { get; set; }


        public ObservationValidationResult(bool isValid, string message = null)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}