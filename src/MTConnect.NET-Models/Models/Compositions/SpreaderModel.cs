// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for flattening or spreading materials.
    /// </summary>
    public class SpreaderModel : CompositionModel, ISpreaderModel
    {
        public SpreaderModel() 
        {
            Type = SpreaderComposition.TypeId;
        }

        public SpreaderModel(string compositionId)
        {
            Id = compositionId;
            Type = SpreaderComposition.TypeId;
        }
    }
}