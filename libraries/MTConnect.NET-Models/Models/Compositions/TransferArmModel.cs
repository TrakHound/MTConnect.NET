// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for physically moving a tool from one location to another.
    /// </summary>
    public class TransferArmModel : CompositionModel, ITransferArmModel
    {
        public TransferArmModel() 
        {
            Type = TransferArmComposition.TypeId;
        }

        public TransferArmModel(string compositionId)
        {
            Id = compositionId;
            Type = TransferArmComposition.TypeId;
        }
    }
}