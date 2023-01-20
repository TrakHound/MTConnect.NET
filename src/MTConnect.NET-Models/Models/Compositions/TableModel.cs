// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A surface for holding an object or material
    /// </summary>
    public class TableModel : CompositionModel, ITableModel
    {
        public TableModel() 
        {
            Type = TableComposition.TypeId;
        }

        public TableModel(string compositionId)
        {
            Id = compositionId;
            Type = TableComposition.TypeId;
        }
    }
}
