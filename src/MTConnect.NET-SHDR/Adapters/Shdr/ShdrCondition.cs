// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrCondition
    {
        public string DataItemKey { get; set; }

        public IEnumerable<ShdrFaultState> FaultStates { get; set; }

        public bool IsSent { get; set; }


        /// <summary>
        /// A MD5 Hash of the Condition that can be used for comparison
        /// </summary>
        public string ChangeId
        {
            get
            {
                if (!FaultStates.IsNullOrEmpty())
                {
                    var valueString = "";
                    foreach (var faultState in FaultStates) valueString += faultState.ChangeId;
                    return valueString.ToMD5Hash();
                }

                return null;
            }
        }


        private ShdrCondition() { }

        public ShdrCondition(string dataItemKey)
        {
            DataItemKey = dataItemKey;
        }

        public ShdrCondition(string dataItemKey, ConditionLevel level)
        {
            DataItemKey = dataItemKey;

            AddFaultState(new ShdrFaultState(level));
        }


        /// <summary>
        /// Set the FaultState of the Condition to UNAVAILABLE and clear any other FaultStates
        /// </summary>
        public void Unavailable()
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.UNAVAILABLE));
        }

        /// <summary>
        /// Set the FaultState of the Condition to NORMAL and clear any other FaultStates
        /// </summary>
        public void Normal()
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.NORMAL));
        }

        /// <summary>
        /// Set the FaultState of the Condition to WARNING and clear any other FaultStates
        /// </summary>
        public void Warning(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED
            )
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.WARNING, text, nativeCode, nativeSeverity, qualifier));
        }

        /// <summary>
        /// Set the FaultState of the Condition to FAULT and clear any other FaultStates
        /// </summary>
        public void Fault(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED
            )
        {
            ClearFaultStates();
            AddFaultState(new ShdrFaultState(ConditionLevel.FAULT, text, nativeCode, nativeSeverity, qualifier));
        }

        /// <summary>
        /// Add a FaultState to the Condition of WARNING while retaning any other FaultStates
        /// </summary>
        public void AddWarning(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED
            )
        {
            AddFaultState(new ShdrFaultState(ConditionLevel.WARNING, text, nativeCode, nativeSeverity, qualifier));
        }

        /// <summary>
        /// Add a FaultState to the Condition of FAULT while retaning any other FaultStates
        /// </summary>
        public void AddFault(
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED
            )
        {
            AddFaultState(new ShdrFaultState(ConditionLevel.FAULT, text, nativeCode, nativeSeverity, qualifier));
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
            }
        }

        public void ClearFaultStates()
        {
            FaultStates = null;
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


        ///// <summary>
        ///// Convert ShdrCondition to an SHDR string
        ///// </summary>
        ///// <returns>SHDR string</returns>
        //public override string ToString()
        //{
        //    if (!string.IsNullOrEmpty(DataItemKey) && !FaultStates.IsNullOrEmpty())
        //    {
        //        var lines = new List<string>();

        //        foreach (var faultState in FaultStates)
        //        {
        //            var text = !string.IsNullOrEmpty(faultState.Text) ? faultState.Text.Replace("|", @"\|") : "";
        //            var qualifier = faultState.Qualifier != ConditionQualifier.NOT_SPECIFIED ? faultState.Qualifier.ToString() : "";

        //            string line;

        //            if (faultState.Timestamp > 0)
        //            {
        //                if (faultState.Level != ConditionLevel.UNAVAILABLE)
        //                {
        //                    line = $"{faultState.Timestamp.ToDateTime().ToString("o")}|{DataItemKey}|{faultState.Level}|{faultState.NativeCode}|{faultState.NativeSeverity}|{qualifier}|{text}";
        //                }
        //                else
        //                {
        //                    line = $"{faultState.Timestamp.ToDateTime().ToString("o")}|{DataItemKey}|{faultState.Level}||||";
        //                }
        //            }
        //            else
        //            {
        //                if (faultState.Level != ConditionLevel.UNAVAILABLE)
        //                {
        //                    line = $"{DataItemKey}|{faultState.Level}|{faultState.NativeCode}|{faultState.NativeSeverity}|{qualifier}|{text}";
        //                }
        //                else
        //                {
        //                    line = $"{DataItemKey}|{faultState.Level}||||";
        //                }
        //            }

        //            lines.Add(line);
        //        }

        //        // Convert list of lines to single string with new line terminator
        //        return string.Join(ShdrLine.LineTerminator, lines);
        //    }

        //    return "";
        //}
    }
}