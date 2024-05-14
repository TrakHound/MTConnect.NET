// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class SpecificationLimitResult : EventDataSetObservation
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
        /// Ideal or desired value for a variable.
        /// </summary>
        public double Nominal 
        { 
            get => GetValue<double>("DataSet[Nominal]");
            set => AddValue("DataSet[Nominal]", value);
        }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double UpperLimit 
        { 
            get => GetValue<double>("DataSet[UpperLimit]");
            set => AddValue("DataSet[UpperLimit]", value);
        }
    }
}