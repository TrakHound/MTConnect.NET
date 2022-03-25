// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Samples.Values
{
    /// <summary>
    /// The measurement of the degree to which a substance opposes the passage of an electric current.
    /// </summary>
    public class ResistanceValue : SampleValue
    {
        public ResistanceValue(double nativeValue, string nativeUnits = Devices.Samples.ResistanceDataItem.DefaultUnits)
        {
            Value = nativeValue;
            _units = Devices.Samples.ResistanceDataItem.DefaultUnits;
            _nativeUnits = nativeUnits;
        }
    }
}
