// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Http
{
    /// <summary>
    /// An Observation that is attempting to be added to an MTConnect Agent from an Http interface
    /// </summary>
    public struct MTConnectHttpObservationInput
    {
        public string DeviceUuid { get; set; }

        public string DataItemKey { get; set; }

        public string Input { get; set; }


        public MTConnectHttpObservationInput(string deviceUuid, string dataItemKey, string input)
        {
            DeviceUuid = deviceUuid;
            DataItemKey = dataItemKey;
            Input = input;
        }
    }
}
