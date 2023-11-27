// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Input
{
    /// <summary>
    /// An Information Model that describes Condition Streaming Data reported by a piece of equipment.
    /// </summary>
    public class ConditionObservationInput : IConditionObservationInput
    {
        private readonly object _lock = new object();

        private string _deviceKey;
        private string _dataItemKey;
        private IEnumerable<IConditionFaultStateObservationInput> _faultStates;

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
        public IEnumerable<IConditionFaultStateObservationInput> FaultStates
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


        public ConditionObservationInput() { }

        public ConditionObservationInput(string dataItemKey)
        {
            DataItemKey = dataItemKey;
        }

        public ConditionObservationInput(string dataItemKey, ConditionLevel level)
        {
            DataItemKey = dataItemKey;
            AddFaultState(new ConditionFaultStateObservationInput(dataItemKey, level));
        }

        public ConditionObservationInput(string dataItemKey, ConditionLevel level, long timestamp)
        {
            DataItemKey = dataItemKey;
            AddFaultState(new ConditionFaultStateObservationInput(dataItemKey, level, timestamp));
        }

        public ConditionObservationInput(string dataItemKey, ConditionLevel level, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            AddFaultState(new ConditionFaultStateObservationInput(dataItemKey, level, timestamp));
        }

        public ConditionObservationInput(IConditionObservationInput condition)
        {
            if (condition != null)
            {
                DeviceKey = condition.DeviceKey;
                DataItemKey = condition.DataItemKey;
                FaultStates = condition.FaultStates?.ToList();
            }
        }


        /// <summary>
        /// Set the FaultState of the Condition to UNAVAILABLE and clear any other FaultStates
        /// </summary>
        public void Unavailable()
        {
            ClearFaultStates();
            AddFaultState(new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.UNAVAILABLE));
            IsUnavailable = true;
        }

        /// <summary>
        /// Set the FaultState of the Condition to UNAVAILABLE and clear any other FaultStates
        /// </summary>
        public void Unavailable(long timestamp)
        {
            ClearFaultStates();
            AddFaultState(new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.UNAVAILABLE, timestamp));
            IsUnavailable = true;
        }

        /// <summary>
        /// Set the FaultState of the Condition to UNAVAILABLE and clear any other FaultStates
        /// </summary>
        public void Unavailable(DateTime timestamp)
        {
            ClearFaultStates();
            AddFaultState(new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.UNAVAILABLE, timestamp));
            IsUnavailable = true;
        }


        /// <summary>
        /// Set the FaultState of the Condition to NORMAL and clear all other FaultStates
        /// </summary>
        public void Normal()
        {
            ClearFaultStates();
            AddFaultState(new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.NORMAL));
        }

        /// <summary>
        /// Set the FaultState of the Condition to NORMAL and clear all other FaultStates
        /// </summary>
        public void Normal(long timestamp)
        {
            ClearFaultStates();
            AddFaultState(new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.NORMAL, timestamp));
        }

        /// <summary>
        /// Set the FaultState of the Condition to NORMAL and clear all other FaultStates
        /// </summary>
        public void Normal(DateTime timestamp)
        {
            ClearFaultStates();
            AddFaultState(new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.NORMAL, timestamp));
        }


        /// <summary>
        /// Set the FaultState of the Condition to WARNING and clear any other FaultStates
        /// </summary>
        public void Warning(string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            ClearFaultStates();

            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.WARNING);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Set the FaultState of the Condition to WARNING and clear any other FaultStates
        /// </summary>
        public void Warning(long timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            ClearFaultStates();

            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.WARNING, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Set the FaultState of the Condition to WARNING and clear any other FaultStates
        /// </summary>
        public void Warning(DateTime timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            ClearFaultStates();

            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.WARNING, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }


        /// <summary>
        /// Set the FaultState of the Condition to FAULT and clear any other FaultStates
        /// </summary>
        public void Fault(string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            ClearFaultStates();

            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.FAULT);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Set the FaultState of the Condition to FAULT and clear any other FaultStates
        /// </summary>
        public void Fault(long timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            ClearFaultStates();

            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.FAULT, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Set the FaultState of the Condition to FAULT and clear any other FaultStates
        /// </summary>
        public void Fault(DateTime timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            ClearFaultStates();

            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.FAULT, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }


        /// <summary>
        /// Add a FaultState to the Condition of NORMAL while retaining any other FaultStates that do not match the specified NativeCode
        /// </summary>
        public void AddNormal(string nativeCode, string text = null)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.NORMAL);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Add a FaultState to the Condition of NORMAL while retaining any other FaultStates that do not match the specified NativeCode
        /// </summary>
        public void AddNormal(long timestamp, string nativeCode, string text = null)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.NORMAL, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Add a FaultState to the Condition of NORMAL while retaining any other FaultStates that do not match the specified NativeCode
        /// </summary>
        public void AddNormal(DateTime timestamp, string nativeCode, string text = null)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.NORMAL, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            AddFaultState(faultState);
        }


        /// <summary>
        /// Add a FaultState to the Condition of WARNING while retaining any other FaultStates
        /// </summary>
        public void AddWarning(string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.WARNING);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Add a FaultState to the Condition of WARNING while retaining any other FaultStates
        /// </summary>
        public void AddWarning(long timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.WARNING, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Add a FaultState to the Condition of WARNING while retaining any other FaultStates
        /// </summary>
        public void AddWarning(DateTime timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.WARNING, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }


        /// <summary>
        /// Add a FaultState to the Condition of FAULT while retaining any other FaultStates
        /// </summary>
        public void AddFault(string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.FAULT);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Add a FaultState to the Condition of FAULT while retaining any other FaultStates
        /// </summary>
        public void AddFault(long timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.FAULT, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }

        /// <summary>
        /// Add a FaultState to the Condition of FAULT while retaining any other FaultStates
        /// </summary>
        public void AddFault(DateTime timestamp, string text = null, string nativeCode = null, string nativeSeverity = null, ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED)
        {
            var faultState = new ConditionFaultStateObservationInput(DataItemKey, ConditionLevel.FAULT, timestamp);
            faultState.Message = text;
            faultState.NativeCode = nativeCode;
            faultState.NativeSeverity = nativeSeverity;
            faultState.Qualifier = qualifier;
            AddFaultState(faultState);
        }


        /// <summary>
        /// Add a FaultState to the Condition while retaining any other FaultStates
        /// </summary>
        public void AddFaultState(IConditionFaultStateObservationInput faultState)
        {
            if (faultState != null)
            {
                faultState.DeviceKey = DeviceKey;
                faultState.DataItemKey = DataItemKey;

                if (!string.IsNullOrEmpty(faultState.NativeCode))
                {
                    ClearFaultStates(faultState.NativeCode);
                }
                else if (faultState.Level == ConditionLevel.WARNING || faultState.Level == ConditionLevel.FAULT)
                {
                    ClearFaultStates();
                }

                lock (_lock)
                {
                    List<IConditionFaultStateObservationInput> x = null;
                    if (!_faultStates.IsNullOrEmpty()) x = _faultStates.ToList();
                    if (x == null) x = new List<IConditionFaultStateObservationInput>();
                    if (!x.Any(o => o.ChangeId == faultState.ChangeId)) x.Add(faultState);
                    FaultStates = x;
                }

                _changeId = null;
                _changeIdWithTimestamp = null;
            }
        }


        public void ClearFaultStates(string nativeCode)
        {
            lock (_lock)
            {
                if (!_faultStates.IsNullOrEmpty())
                {
                    var x = new List<IConditionFaultStateObservationInput>();
                    foreach (var faultState in _faultStates)
                    {
                        if (faultState.NativeCode != nativeCode)
                        {
                            x.Add(faultState);
                        }
                    }
                    _faultStates = x;
                }
            }

            _changeId = null;
            _changeIdWithTimestamp = null;
        }

        public void ClearFaultStates()
        {
            lock (_lock) _faultStates = null;
            _changeId = null;
            _changeIdWithTimestamp = null;
        }


        private static byte[] CreateChangeId(ConditionObservationInput condition, bool includeTimestamp)
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