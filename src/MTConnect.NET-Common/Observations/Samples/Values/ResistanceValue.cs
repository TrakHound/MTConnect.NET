// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the degree to which a substance opposes the passage of an electric current.
    /// </summary>
    public class ResistanceValue : SampleValue
    {
        public ResistanceValue(double nativeValue, string nativeUnits = ResistanceDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = ResistanceDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
