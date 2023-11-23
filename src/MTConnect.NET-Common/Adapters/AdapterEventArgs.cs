// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Adapters
{
    public struct AdapterEventArgs
    {
        public string ClientId { get; set; }

        public string Message { get; set; }


        public AdapterEventArgs(string clientId, string message)
        {
            ClientId = clientId;
            Message = message;
        }
    }
}