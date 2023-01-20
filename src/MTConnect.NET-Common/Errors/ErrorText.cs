// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Errors
{
    public class ErrorText
    {
        public const string InvalidRequestNegativeFrom = "If the value provided for the from parameter is a negative number or is not a, the Request MUST be determined to be invalid.";
        public const string InvalidRequestNegativeTo = "If the value provided for the to parameter is a negative number or is not a, the Request MUST be determined to be invalid.";
        public const string InvalidRequestNegativeAt = "If the value provided for the at parameter is a negative number or is not a, the Request MUST be determined to be invalid.";
        public const string InvalidRequestNegativeCount = "If the to parameter is given and the count parameter is less than zero, the Agent MUST return a 400 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an INVALID_REQUEST errorCode.";

        public const string InvalidRequestToLessThanFrom = "If the to parameter is less than the from parameter, the Agent MUST return a 400 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an INVALID_REQUEST errorCode.";
        public const string InvalidRequestAtIntervalConjunction = "The at parameter MUST NOT be used in conjunction with the interval parameter since this would cause an Agent to repeatedly return the same data.";

        public const string OutOfRangeFrom = "If the from parameter is less than the firstSequence or greater than lastSequence, the Agent MUST return a 404 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an OUT_OF_RANGE errorCode.";
        public const string OutOfRangeTo = "If the to parameter is less than the firstSequence or greater than lastSequence, the Agent MUST return a 404 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an OUT_OF_RANGE errorCode.";
        public const string OutOfRangeAt = "If the value provided for the at parameter is either lower than the value of firstSequence or greater than the value of lastSequence, the Request MUST be determined to be invalid.";
        public const string OutOfRangeCount = "If the absolute value of count is greater than the size of the buffer or equal to zero(0), the Agent MUST return a 404 HTTP Status Code and MUST publish an MTConnectErrors Response Document with an OUT_OF_RANGE errorCode.";
    }
}
