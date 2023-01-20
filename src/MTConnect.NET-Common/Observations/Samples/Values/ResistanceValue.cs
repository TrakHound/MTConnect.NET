// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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