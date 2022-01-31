// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
