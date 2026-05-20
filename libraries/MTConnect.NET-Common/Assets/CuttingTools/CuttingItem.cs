// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Assets.CuttingTools
{
    public partial class CuttingItem
    {
        /// <summary>
        /// Initializes a new CuttingItem with empty cutter-status and measurement collections.
        /// </summary>
        public CuttingItem()
        {
            CutterStatus = new List<CutterStatusType>();
            Measurements = new List<ToolingMeasurement>();
        }


        /// <summary>
        /// Returns a copy of this cutting item with its measurements rebound to their concrete tooling-measurement subtypes so they serialize with the correct element name.
        /// </summary>
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
                var measurements = new List<ToolingMeasurement>();
                foreach (var measurement in Measurements)
                {
                    var typeMeasurement = ToolingMeasurement.Create(measurement.Type, measurement);
                    if (typeMeasurement != null) measurements.Add(typeMeasurement);
                }
                cuttingItem.Measurements = measurements;
            }

            return cuttingItem;
        }

        /// <summary>
        /// Computes a SHA-1 content hash over the cutting item's scalar properties for change detection; returns null for a null item.
        /// </summary>
        /// <param name="cuttingItem">The cutting item to hash.</param>
        public static string GenerateHash(ICuttingItem cuttingItem)
        {
            if (cuttingItem != null)
            {
                var ids = new List<string>();
                ids.Add(ObjectExtensions.GetHashPropertyString(cuttingItem).ToSHA1Hash());
                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }
    }
}