// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class MaintenanceListResult : EventDataSetObservation
    {

        /// <summary>
        /// Direction of the value observed.
        /// </summary>
        public MaintenanceListDirection Direction 
        { 
            get => GetValue<MaintenanceListDirection>("DataSet[Direction]");
            set => AddValue("DataSet[Direction]", value);
        }
        
        /// <summary>
        /// Interval of the value observed.
        /// </summary>
        public MaintenanceListInterval Interval 
        { 
            get => GetValue<MaintenanceListInterval>("DataSet[Interval]");
            set => AddValue("DataSet[Interval]", value);
        }
        
        /// <summary>
        /// Last date/time stamp that maintenance was performed.
        /// </summary>
        public System.DateTime LastServiceDate 
        { 
            get => GetValue<System.DateTime>("DataSet[LastServiceDate]");
            set => AddValue("DataSet[LastServiceDate]", value);
        }
        
        /// <summary>
        /// Identifier of the maintenance activity.
        /// </summary>
        public string Name 
        { 
            get => GetValue<string>("DataSet[Name]");
            set => AddValue("DataSet[Name]", value);
        }
        
        /// <summary>
        /// Next date/time stamp that maintenance should be performed.
        /// </summary>
        public System.DateTime NextServiceDate 
        { 
            get => GetValue<System.DateTime>("DataSet[NextServiceDate]");
            set => AddValue("DataSet[NextServiceDate]", value);
        }
        
        /// <summary>
        /// Last date/time stamp of the observation was reset.
        /// </summary>
        public System.DateTime Reset 
        { 
            get => GetValue<System.DateTime>("DataSet[Reset]");
            set => AddValue("DataSet[Reset]", value);
        }
        
        /// <summary>
        /// Level of severity on a scale of 1-10.
        /// </summary>
        public int Severity 
        { 
            get => GetValue<int>("DataSet[Severity]");
            set => AddValue("DataSet[Severity]", value);
        }
        
        /// <summary>
        /// Target value of the next maintenance.
        /// </summary>
        public double Target 
        { 
            get => GetValue<double>("DataSet[Target]");
            set => AddValue("DataSet[Target]", value);
        }
        
        /// <summary>
        /// Units. See Device Information Model.
        /// </summary>
        public string Units 
        { 
            get => GetValue<string>("DataSet[Units]");
            set => AddValue("DataSet[Units]", value);
        }
        
        /// <summary>
        /// Current interval value of the activity.
        /// </summary>
        public double Value 
        { 
            get => GetValue<double>("DataSet[Value]");
            set => AddValue("DataSet[Value]", value);
        }
    }
}