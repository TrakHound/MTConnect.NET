// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class AlarmLimitResult : EventDataSetObservation
    {

        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double LowerLimit 
        { 
            get => GetValue<double>("DataSet[LowerLimit]");
            set => AddValue("DataSet[LowerLimit]", value);
        }
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double LowerWarning 
        { 
            get => GetValue<double>("DataSet[LowerWarning]");
            set => AddValue("DataSet[LowerWarning]", value);
        }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double UpperLimit 
        { 
            get => GetValue<double>("DataSet[UpperLimit]");
            set => AddValue("DataSet[UpperLimit]", value);
        }
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double UpperWarning 
        { 
            get => GetValue<double>("DataSet[UpperWarning]");
            set => AddValue("DataSet[UpperWarning]", value);
        }
    }
}