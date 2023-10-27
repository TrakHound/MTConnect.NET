// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public enum DataItemStatistic
    {
        /// <summary>
        /// Mathematical average value calculated for the data item during the calculation period.
        /// </summary>
        AVERAGE,
        
        /// <summary>
        /// **deprecated** in *Version 1.6*. ~~A measure of the 'peakedness' of a probability distribution; i.e., the shape of the distribution curve.~~
        /// </summary>
        KURTOSIS,
        
        /// <summary>
        /// Maximum or peak value recorded for the data item during the calculation period.
        /// </summary>
        MAXIMUM,
        
        /// <summary>
        /// Middle number of a series of numbers.
        /// </summary>
        MEDIAN,
        
        /// <summary>
        /// Minimum value recorded for the data item during the calculation period.
        /// </summary>
        MINIMUM,
        
        /// <summary>
        /// Number in a series of numbers that occurs most often.
        /// </summary>
        MODE,
        
        /// <summary>
        /// Difference between the maximum and minimum value of a data item during the calculation period. Also represents Peak-to-Peak measurement in a waveform.
        /// </summary>
        RANGE,
        
        /// <summary>
        /// Mathematical Root Mean Square (RMS) value calculated for the data item during the calculation period.
        /// </summary>
        ROOT_MEAN_SQUARE,
        
        /// <summary>
        /// Statistical Standard Deviation value calculated for the data item during the calculation period.
        /// </summary>
        STANDARD_DEVIATION,
        
        /// <summary>
        /// 
        /// </summary>
        NONE
    }
}