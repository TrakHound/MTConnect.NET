// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect
{
    internal struct XmlValidationResponse
    {
        public bool Success { get; }

        public IEnumerable<string> Errors { get; }


        public XmlValidationResponse(bool success, IEnumerable<string> errors = null)
        {
            Success = success;
            Errors = errors;
        }
    }
}