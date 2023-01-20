// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An object or material on which a form of work is performed.
    /// </summary>
    public class WorkpieceModel : CompositionModel, IWorkpieceModel
    {
        public WorkpieceModel() 
        {
            Type = WorkpieceComposition.TypeId;
        }

        public WorkpieceModel(string compositionId)
        {
            Id = compositionId;
            Type = WorkpieceComposition.TypeId;
        }
    }
}