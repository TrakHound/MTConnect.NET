// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// A Value of a Property associated with an Observation at a point in time
    /// </summary>
    public struct ObservationValue
    {
        /// <summary>
        /// The unique Key of the Value that is recorded
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The recorded value of the Observation's Property
        /// </summary>
        public string Value { get; }


        public ObservationValue(string key, object value)
        {
            Value = value != null ? value.ToString() : string.Empty;
            Key = key;
        }


        public bool HasValue()
        {
            return !string.IsNullOrEmpty(Key) && Value != null;
        }
    }
}
