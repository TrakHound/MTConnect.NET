// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    public interface IMTConnectEntity
    {
        string Uuid { get; }

        MTConnectEntityType EntityType { get; }
    }
}
