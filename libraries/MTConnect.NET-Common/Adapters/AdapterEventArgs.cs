// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Adapters
{
    public struct AdapterEventArgs<T>
    {
        public string ClientId { get; set; }

        public T Data { get; set; }


        public AdapterEventArgs(string clientId, T data)
        {
            ClientId = clientId;
            Data = data;
        }
    }
}