// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Devices
{
    /// <summary>
    /// Agent is a Device representing the MTConnect Agent and all its connected data sources.
    /// </summary>
    public class Agent : Device 
    {
        public new const string TypeId = "Agent";
        public new const string DescriptionText = "Agent is a Device representing the MTConnect Agent and all its connected data sources.";

        public override string TypeDescription => DescriptionText;

        public Agent()
        {
            Type = TypeId;
            DataItems = new List<DataItem>();
            Components = new List<Component>();
            Compositions = new List<Composition>();
        }
    }
}
