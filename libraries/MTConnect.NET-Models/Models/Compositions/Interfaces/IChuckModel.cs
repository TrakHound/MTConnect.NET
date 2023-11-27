// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events.Values;

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