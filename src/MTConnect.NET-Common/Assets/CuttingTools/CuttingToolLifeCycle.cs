﻿// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Assets.CuttingTools
{
    public partial class CuttingToolLifeCycle
    {
        public CuttingToolLifeCycle()
        {
            CutterStatus = new List<CutterStatusType>();
            Measurements = new List<Measurement>();
            CuttingItems = new List<ICuttingItem>();
        }


        public ICuttingToolLifeCycle Process()
        {
            var lifeCycle = new CuttingToolLifeCycle();
            lifeCycle.CutterStatus = CutterStatus;
            lifeCycle.ReconditionCount = ReconditionCount;
            lifeCycle.ToolLife = ToolLife;
            lifeCycle.Location = Location;
            lifeCycle.ProgramToolGroup = ProgramToolGroup;
            lifeCycle.ProgramToolNumber = ProgramToolNumber;
            lifeCycle.ProcessSpindleSpeed = ProcessSpindleSpeed;
            lifeCycle.ProcessFeedRate = ProcessFeedRate;
            lifeCycle.ConnectionCodeMachineSide = ConnectionCodeMachineSide;
            lifeCycle.CuttingItems = CuttingItems;

            // Process Cutting Items
            if (CuttingItems != null && !CuttingItems.IsNullOrEmpty() && CuttingItems.Count() > 0)
            {
                var cuttingItems = new List<ICuttingItem>();
                foreach (var cuttingItem in CuttingItems)
                {
                    var processedCuttingItem = cuttingItem.Process();
                    if (processedCuttingItem != null) cuttingItems.Add(processedCuttingItem);
                }
                lifeCycle.CuttingItems = cuttingItems;
            }

            // Process Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<Measurement>();
                foreach (var measurement in Measurements)
                {
                    var typeMeasurement = Measurement.Create(measurement.Type, measurement);
                    if (typeMeasurement != null) measurements.Add(typeMeasurement);
                }
                lifeCycle.Measurements = measurements;
            }

            return lifeCycle;
        }

        public static string GenerateHash(ICuttingToolLifeCycle cuttingToolLifeCycle)
        {
            if (cuttingToolLifeCycle != null)
            {
                var ids = new List<string>();
                ids.Add(ObjectExtensions.GetHashPropertyString(cuttingToolLifeCycle).ToSHA1Hash());

                if (!cuttingToolLifeCycle.CutterStatus.IsNullOrEmpty())
                {
                    foreach (var cutterStatus in cuttingToolLifeCycle.CutterStatus)
                    {
                        ids.Add(cutterStatus.ToString());
                    }
                }

                if (!cuttingToolLifeCycle.CuttingItems.IsNullOrEmpty())
                {
                    foreach (var cuttingItem in cuttingToolLifeCycle.CuttingItems)
                    {
                        ids.Add(CuttingItem.GenerateHash(cuttingItem));
                    }
                }

                if (!cuttingToolLifeCycle.ToolLife.IsNullOrEmpty())
                {
                    foreach (var toolLife in cuttingToolLifeCycle.ToolLife)
                    {
                        ids.Add(CuttingTools.ToolLife.GenerateHash(toolLife));
                    }
                }

                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }
    }
}
