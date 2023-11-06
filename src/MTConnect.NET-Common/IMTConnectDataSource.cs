// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    public interface IMTConnectDataSource
    {
        string Id { get; }

        string Description { get; }


        void Start();

        void Stop();
    }
}
