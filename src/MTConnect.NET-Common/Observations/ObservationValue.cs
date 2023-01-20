// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// A Value of a Property associated with an Observation at a point in time
    /// </summary>
    public struct ObservationValue
    {
        internal readonly string _key;
        internal readonly string _value;


        /// <summary>
        /// The unique Key of the Value that is recorded
        /// </summary>
        public string Key => _key;

        /// <summary>
        /// The recorded value of the Observation's Property
        /// </summary>
        public string Value => _value;


        public ObservationValue(string key, object value)
        {
            _key = key;
            _value = value != null ? value.ToString() : string.Empty;
        }


        public bool HasValue()
        {
            return !string.IsNullOrEmpty(Key) && Value != null;
        }
    }
}
