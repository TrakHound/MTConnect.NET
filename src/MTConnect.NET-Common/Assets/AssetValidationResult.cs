// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets
{
    public struct AssetValidationResult
    {
        public bool IsValid { get; set; }

        public string Message { get; set; }


        public AssetValidationResult(bool isValid, string message = null)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}