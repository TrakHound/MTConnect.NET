// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Adapters.Shdr
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
