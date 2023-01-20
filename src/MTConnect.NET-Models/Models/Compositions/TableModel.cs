// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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