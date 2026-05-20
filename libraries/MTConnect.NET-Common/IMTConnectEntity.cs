// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// Common contract for any addressable entity in the MTConnect model, exposing its unique identity and its kind.
    /// </summary>
    public interface IMTConnectEntity
    {
        /// <summary>
        /// The globally unique identifier of this entity.
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// The category of this entity within the MTConnect model.
        /// </summary>
        MTConnectEntityType EntityType { get; }
    }
}
