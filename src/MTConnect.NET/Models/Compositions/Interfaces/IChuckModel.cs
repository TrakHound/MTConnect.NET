// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Streams.Events;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that holds a part, stock material, or any other item in place.
    /// </summary>
    public interface IChuckModel : ICompositionModel
    {
        /// <summary>
        /// An indication of the operating state of a mechanism that holds a part or stock material during a manufacturing process. 
        /// It may also represent a mechanism that holds any other mechanism in place within a piece of equipment.
        /// </summary>
        ChuckState ChuckState { get; set; }

        /// <summary>
        /// An indication of the state of an interlock function or control logic state intended to prevent the associated CHUCK component from being operated.
        /// </summary>
        ChuckInterlock ChuckInterlock { get; set; }
    }
}
