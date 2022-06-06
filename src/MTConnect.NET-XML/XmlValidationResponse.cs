// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect
{
    public struct XmlValidationResponse
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
