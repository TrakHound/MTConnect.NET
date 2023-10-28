// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Assets.CuttingTools
{
    public partial class CuttingItem
    {
        public CuttingItem()
        {
            CutterStatus = new List<CutterStatusType>();
            Measurements = new List<Measurement>();
        }


        public ICuttingItem Process()
        {
            var cuttingItem = new CuttingItem();
            cuttingItem.Indices = Indices;
            cuttingItem.ItemId = ItemId;
            cuttingItem.Manufacturers = Manufacturers;
            cuttingItem.Grade = Grade;
            cuttingItem.Description = Description;
            cuttingItem.Locus = Locus;
            cuttingItem.ItemLife = ItemLife;
            cuttingItem.CutterStatus = CutterStatus;
            cuttingItem.ProgramToolGroup = ProgramToolGroup;

            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<Measurement>();
                foreach (var measurement in Measurements)
                {
                    var typeMeasurement = Measurement.Create(measurement.Type, measurement);
                    if (typeMeasurement != null) measurements.Add(typeMeasurement);
                }
                cuttingItem.Measurements = measurements;
            }

            return cuttingItem;
        }
    }
}