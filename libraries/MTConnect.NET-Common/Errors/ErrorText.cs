// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Errors
{
    /// <summary>
    /// Canonical, spec-derived message text used when an Agent rejects a sample/current request because a sequence parameter is malformed or outside the buffer range.
    /// </summary>
    public class ErrorText
    {
        /// <summary>
        /// Message reported when the from parameter is negative or non-numeric, making the request invalid.
        /// </summary>
        public const string InvalidRequestNegativeFrom = "If the value provided for the from parameter is a negative number or is not a, the Request MUST be determined to be invalid.";

        /// <summary>
        /// Message reported when the to parameter is negative or non-numeric, making the request invalid.
        /// </summary>
        public const string InvalidRequestNegativeTo = "If the value provided for the to parameter is a negative number or is not a, the Request MUST be determined to be invalid.";

        /// <summary>
        /// Message reported when the at parameter is negative or non-numeric, making the request invalid.
        /// </summary>
        public const string InvalidRequestNegativeAt = "If the value provided for the at parameter is a negative number or is not a, the Request MUST be determined to be invalid.";

        /// <summary>
        /// Message reported (with a 400 status and INVALID_REQUEST code) when to is given together with a negative count.
        /// </summary>
        public const string InvalidRequestNegativeCount = "If the to parameter is given and the count parameter is less than zero, the Agent MUST return a 400 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an INVALID_REQUEST errorCode.";

        /// <summary>
        /// Message reported (with a 400 status and INVALID_REQUEST code) when the to parameter is less than the from parameter.
        /// </summary>
        public const string InvalidRequestToLessThanFrom = "If the to parameter is less than the from parameter, the Agent MUST return a 400 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an INVALID_REQUEST errorCode.";

        /// <summary>
        /// Message reported when the at parameter is combined with the interval parameter, a contradictory pairing that would repeatedly return identical data.
        /// </summary>
        public const string InvalidRequestAtIntervalConjunction = "The at parameter MUST NOT be used in conjunction with the interval parameter since this would cause an Agent to repeatedly return the same data.";

        /// <summary>
        /// Message reported (with a 404 status and OUT_OF_RANGE code) when from falls outside the firstSequence..lastSequence window.
        /// </summary>
        public const string OutOfRangeFrom = "If the from parameter is less than the firstSequence or greater than lastSequence, the Agent MUST return a 404 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an OUT_OF_RANGE errorCode.";

        /// <summary>
        /// Message reported (with a 404 status and OUT_OF_RANGE code) when to falls outside the firstSequence..lastSequence window.
        /// </summary>
        public const string OutOfRangeTo = "If the to parameter is less than the firstSequence or greater than lastSequence, the Agent MUST return a 404 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an OUT_OF_RANGE errorCode.";

        /// <summary>
        /// Message reported when the at parameter falls outside the firstSequence..lastSequence window, making the request invalid.
        /// </summary>
        public const string OutOfRangeAt = "If the value provided for the at parameter is either lower than the value of firstSequence or greater than the value of lastSequence, the Request MUST be determined to be invalid.";

        /// <summary>
        /// Message reported (with a 404 status and OUT_OF_RANGE code) when the magnitude of count exceeds the buffer size or is zero.
        /// </summary>
        public const string OutOfRangeCount = "If the absolute value of count is greater than the size of the buffer or equal to zero(0), the Agent MUST return a 404 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an OUT_OF_RANGE errorCode.";
    }
}
