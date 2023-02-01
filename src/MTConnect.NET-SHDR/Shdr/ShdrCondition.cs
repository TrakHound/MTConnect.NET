// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Shdr
{
    /// <summary>
    /// An Observation representing an MTConnect Condition
    /// </summary>
    public class ShdrCondition
    {
        private readonly object _lock = new object();

        private string _deviceKey;
        private string _dataItemKey;
        private IEnumerable<ShdrFaultState> _faultStates;

        private byte[] _changeId;
        private byte[] _changeIdWithTimestamp;


        /// <summary>
        /// Flag to set whether the Observation has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }

        /// <summary>
        /// The UUID or Name of the Device that the Observation is associated with
        /// </summary>
        public string DeviceKey
        {
            get => _deviceKey;
            set
            {
                _deviceKey = value;

                lock (_lock)
                {
                    if (!_faultStates.IsNullOrEmpty())
                    {
                        foreach (var faultState in _faultStates) faultState.DeviceKey = _deviceKey;
                    }
                }
            }
        }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        public string DataItemKey
        {
            get => _dataItemKey;
            set
            {
                _dataItemKey = value;

                lock (_lock)
                {
                    if (!_faultStates.IsNullOrEmpty())
                    {
                        foreach (var faultState in _faultStates) faultState.DataItemKey = _dataItemKey;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the FaultStates associated with the Condition Observation
        /// </summary>
        public IEnumerable<ShdrFaultState> FaultStates
        {
            get => _faultStates;
            set
            {
                lock (_lock) _faultStates = value;
            }
        }

        /// <summary>
        /// A MD5 Hash of the Condition that can be used for comparison
        /// </summary>
        public byte[] ChangeId
        {
            get
            {
                if (_changeId == null) _changeId = CreateChangeId(this, false);
                return _changeId;
            }
        }

        /// <summary>
        /// A MD5 Hash of the Condition including the Timestamp that can be used for comparison
        /// </summary>
        public byte[] ChangeIdWithTimestamp
        {
            get
            {
                if (_changeIdWithTimestamp == null) _changeIdWithTimestamp = CreateChangeId(this, true);
                return _changeIdWithTimestamp;
            }
        }

        /// <summary>
        /// Gets or Sets whether the Observation is Unavailable
        /// </summary>
        public bool IsUnavailable { get; set; }


        private ShdrCondition() { }

        public ShdrCondition(string dataItemKey)
        {
            DataItemKey = dataItemKey;
        }

        public ShdrCondition(string dataItemKey, ConditionLevel level, long timestamp = 0)
        {
            DataItemKey = dataItemKey;

            AddFaultState(new ShdrFaultState(level, timestamp: timestamp));
        }


        /// <summary>
        /// Set the FaultState of the Condition to UNAVAILABLE and clear any other FaultStates
        /// </summary>
        public void Unavailable(long timestamp = 0)
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.UNAVAILABLE, timestamp: timestamp));
            IsUnavailable = true;
        }

        /// <summary>
        /// Set the FaultState of the Condition to NORMAL and clear any other FaultStates
        /// </summary>
        public void Normal(long timestamp = 0)
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.NORMAL, timestamp: timestamp));
        }

        /// <summary>
        /// Set the FaultState of the Condition to WARNING and clear any other FaultStates
        /// </summary>
        public void Warning(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            long timestamp = 0
            )
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.WARNING, text, nativeCode, nativeSeverity, qualifier, timestamp));
        }

        /// <summary>
        /// Set the FaultState of the Condition to FAULT and clear any other FaultStates
        /// </summary>
        public void Fault(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            long timestamp = 0
            )
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.FAULT, text, nativeCode, nativeSeverity, qualifier, timestamp));
        }

        /// <summary>
        /// Add a FaultState to the Condition of WARNING while retaning any other FaultStates
        /// </summary>
        public void AddWarning(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            long timestamp = 0
            )
        {
            AddFaultState(new ShdrFaultState(ConditionLevel.WARNING, text, nativeCode, nativeSeverity, qualifier, timestamp));
        }

        /// <summary>
        /// Add a FaultState to the Condition of FAULT while retaning any other FaultStates
        /// </summary>
        public void AddFault(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            long timestamp = 0
            )
        {
            AddFaultState(new ShdrFaultState(ConditionLevel.FAULT, text, nativeCode, nativeSeverity, qualifier, timestamp));
        }

        /// <summary>
        /// Add a FaultState to the Condition while retaning any other FaultStates
        /// </summary>
        public void AddFaultState(ShdrFaultState faultState)
        {
            if (faultState != null)
            {
                faultState.DeviceKey = DeviceKey;
                faultState.DataItemKey = DataItemKey;

                lock (_lock)
                {
                    List<ShdrFaultState> x = null;
                    if (!_faultStates.IsNullOrEmpty()) x = _faultStates.ToList();
                    if (x == null) x = new List<ShdrFaultState>();
                    x.Add(faultState);
                    FaultStates = x;
                }

                _changeId = null;
                _changeIdWithTimestamp = null;
            }
        }

        public void ClearFaultStates()
        {
            lock (_lock) _faultStates = null;
            _changeId = null;
            _changeIdWithTimestamp = null;
        }

        /// <summary>
        /// Convert ShdrCondition to an SHDR string
        /// </summary>
        /// <returns>SHDR string</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DataItemKey) && !FaultStates.IsNullOrEmpty())
            {
                var lines = new List<string>();

                lock (_lock)
                {
                    foreach (var faultState in _faultStates)
                    {
                        lines.Add(faultState.ToString());
                    }
                }

                // Convert list of lines to single string with new line terminator
                return string.Join(ShdrLine.LineTerminator, lines);
            }

            return "";
        }


        private static byte[] CreateChangeId(ShdrCondition condition, bool includeTimestamp)
        {
            if (condition != null)
            {
                if (!condition.FaultStates.IsNullOrEmpty())
                {
                    var values = new byte[condition.FaultStates.Count()][];
                    var i = 0;
                    foreach (var faultState in condition.FaultStates)
                    {
                        if (includeTimestamp) values[i] = faultState.ChangeIdWithTimestamp;
                        else values[i] = faultState.ChangeId;
                        i++;
                    }
                    return StringFunctions.ToMD5HashBytes(values);
                }
            }

            return null;
        }
    }
}