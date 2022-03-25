// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// Source identifies the physical part of a device where the data represented by the DataItem is originally measured.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// The id attribute of the Component that represents the physical part of a device where teh data represented by the DataItem is actually measured.
        /// </summary>
        string ComponentId { get; }

        /// <summary>
        /// The id attribute of the DataItem that represents the originally measured value of the data referenced by this DataItem.
        /// </summary>
        string DataItemId { get; }

        /// <summary>
        /// The identifier attribute of the Composition element that represents the physical part of a piece of equipment where the data represented by the DataItem element originated.
        /// </summary>
        string CompositionId { get; }

        string CDATA { get; }
    }
}
