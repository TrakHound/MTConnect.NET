// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class FeatureMeasurementResult : EventDataSetObservation
    {

        /// <summary>
        /// UUID of the characteristic.
        /// </summary>
        public string CharacteristicPersistentId 
        { 
            get => GetValue<string>("DataSet[CharacteristicPersistentId]");
            set => AddValue("DataSet[CharacteristicPersistentId]", value);
        }
        
        /// <summary>
        /// Pass/fail result of the measurement.
        /// </summary>
        public CharacteristicStatus CharacteristicStatus 
        { 
            get => GetValue<CharacteristicStatus>("DataSet[CharacteristicStatus]");
            set => AddValue("DataSet[CharacteristicStatus]", value);
        }
        
        /// <summary>
        /// UUID of the feature.
        /// </summary>
        public string FeaturePersistentId 
        { 
            get => GetValue<string>("DataSet[FeaturePersistentId]");
            set => AddValue("DataSet[FeaturePersistentId]", value);
        }
        
        /// <summary>
        /// Identifier of this measurement.
        /// </summary>
        public string MeasurementId 
        { 
            get => GetValue<string>("DataSet[MeasurementId]");
            set => AddValue("DataSet[MeasurementId]", value);
        }
        
        /// <summary>
        /// Class of measurement being performed. QIF 3:2018 Section 6.3Examples: `POINT`, `RADIUS`, `ANGLE`, `LENGTH`, etc.
        /// </summary>
        public string MeasurementType 
        { 
            get => GetValue<string>("DataSet[MeasurementType]");
            set => AddValue("DataSet[MeasurementType]", value);
        }
        
        /// <summary>
        /// Engineering units of the measurement.
        /// </summary>
        public string MeasurementUnits 
        { 
            get => GetValue<string>("DataSet[MeasurementUnits]");
            set => AddValue("DataSet[MeasurementUnits]", value);
        }
        
        /// <summary>
        /// Measurement based on the measurement type.
        /// </summary>
        public double MeasurementValue 
        { 
            get => GetValue<double>("DataSet[MeasurementValue]");
            set => AddValue("DataSet[MeasurementValue]", value);
        }
        
        /// <summary>
        /// Uncertainty specified by `UNCERTAINTY_TYPE`.
        /// </summary>
        public double Uncertainty 
        { 
            get => GetValue<double>("DataSet[Uncertainty]");
            set => AddValue("DataSet[Uncertainty]", value);
        }
        
        /// <summary>
        /// Method used to compute standard uncertainty.
        /// </summary>
        public UncertaintyType UncertaintyType 
        { 
            get => GetValue<UncertaintyType>("DataSet[UncertaintyType]");
            set => AddValue("DataSet[UncertaintyType]", value);
        }
    }
}