// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Data;
using System.Xml;
using System.Collections.Generic;

namespace MTConnect.Components
{
    // <summary>
    // Object class to return all data associated with Probe command results
    // </summary>
    public class ReturnData : IDisposable
    {
         //Device object with heirarchy of values and xml structure
        public List<Device> Devices { get; set; }

         //Header Information
        public Headers.Devices Header;

        public void Dispose()
        {
            Devices.Clear();
            Devices = null;
        }
    }
}
