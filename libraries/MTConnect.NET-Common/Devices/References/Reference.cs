// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.References
{
    public abstract partial class Reference
    {
        /// <summary>
        /// A human-readable description of the concrete Reference type, defaulting to the type's description text.
        /// </summary>
        public virtual string TypeDescription => DescriptionText;
    }
}