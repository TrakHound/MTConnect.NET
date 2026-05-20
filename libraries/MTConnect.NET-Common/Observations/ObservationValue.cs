// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// A Value of a Property associated with an <see cref="Observation"></see> at a point in time
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


        /// <summary>
        /// Initializes a new Observation value pairing a key with a value, stringifying the value.
        /// </summary>
        /// <param name="key">The unique key identifying the value.</param>
        /// <param name="value">The value to record; stored as an empty string when null.</param>
        public ObservationValue(string key, object value)
        {
            _key = key;
            _value = value != null ? value.ToString() : string.Empty;
        }


        /// <summary>
        /// Determines whether this entry carries both a non-empty key and a value.
        /// </summary>
        /// <returns>True when the entry has a usable key and value; otherwise false.</returns>
        public bool HasValue()
        {
            return !string.IsNullOrEmpty(Key) && Value != null;
        }
    }
}