// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// An Observation representing an MTConnect Condition
    /// </summary>
    public class ShdrCondition
    {
        private string _changeId;


        /// <summary>
        /// Flag to set whether the Observation has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }

        /// <summary>
        /// The UUID or Name of the Device that the Observation is associated with
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        public string DataItemKey { get; set; }

        /// <summary>
        /// Gets or Sets the FaultStates associated with the Condition Observation
        /// </summary>
        public IEnumerable<ShdrFaultState> FaultStates { get; set; }

        /// <summary>
        /// A MD5 Hash of the Condition that can be used for comparison
        /// </summary>
        public string ChangeId
        {
            get
            {
                if (_changeId == null) _changeId = CreateChangeId(this);
                return _changeId;
            }
        }


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
                faultState.DataItemKey = DataItemKey;

                List<ShdrFaultState> x = null;
                if (!FaultStates.IsNullOrEmpty()) x = FaultStates.ToList();
                if (x == null) x = new List<ShdrFaultState>();
                x.Add(faultState);
                FaultStates = x;
                _changeId = null;
            }
        }

        public void ClearFaultStates()
        {
            FaultStates = null;
            _changeId = null;
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

                foreach (var faultState in FaultStates)
                {
                    lines.Add(faultState.ToString());
                }

                // Convert list of lines to single string with new line terminator
                return string.Join(ShdrLine.LineTerminator, lines);
            }

            return "";
        }


        private static string CreateChangeId(ShdrCondition condition)
        {
            if (condition != null)
            {
                if (!condition.FaultStates.IsNullOrEmpty())
                {
                    var valueString = "";
                    foreach (var faultState in condition.FaultStates) valueString += faultState.ChangeId;
                    return valueString.ToMD5Hash();
                }
            }

            return null;
        }
    }
}