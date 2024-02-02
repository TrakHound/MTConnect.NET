// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class DataItemStatisticDescriptions
    {
        /// <summary>
        /// Mathematical average value calculated for the data item during the calculation period.
        /// </summary>
        public const string AVERAGE = "Mathematical average value calculated for the data item during the calculation period.";
        
        /// <summary>
        /// **DEPRECATED** in *Version 1.6*. ~~A measure of the 'peakedness' of a probability distribution; i.e., the shape of the distribution curve.~~
        /// </summary>
        public const string KURTOSIS = "**DEPRECATED** in *Version 1.6*. ~~A measure of the 'peakedness' of a probability distribution; i.e., the shape of the distribution curve.~~";
        
        /// <summary>
        /// Maximum or peak value recorded for the data item during the calculation period.
        /// </summary>
        public const string MAXIMUM = "Maximum or peak value recorded for the data item during the calculation period.";
        
        /// <summary>
        /// Middle number of a series of numbers.
        /// </summary>
        public const string MEDIAN = "Middle number of a series of numbers.";
        
        /// <summary>
        /// Minimum value recorded for the data item during the calculation period.
        /// </summary>
        public const string MINIMUM = "Minimum value recorded for the data item during the calculation period.";
        
        /// <summary>
        /// Number in a series of numbers that occurs most often.
        /// </summary>
        public const string MODE = "Number in a series of numbers that occurs most often.";
        
        /// <summary>
        /// Difference between the maximum and minimum value of a data item during the calculation period. Also represents Peak-to-Peak measurement in a waveform.
        /// </summary>
        public const string RANGE = "Difference between the maximum and minimum value of a data item during the calculation period. Also represents Peak-to-Peak measurement in a waveform.";
        
        /// <summary>
        /// Mathematical Root Mean Square (RMS) value calculated for the data item during the calculation period.
        /// </summary>
        public const string ROOT_MEAN_SQUARE = "Mathematical Root Mean Square (RMS) value calculated for the data item during the calculation period.";
        
        /// <summary>
        /// Statistical Standard Deviation value calculated for the data item during the calculation period.
        /// </summary>
        public const string STANDARD_DEVIATION = "Statistical Standard Deviation value calculated for the data item during the calculation period.";


        public static string Get(DataItemStatistic value)
        {
            switch (value)
            {
                case DataItemStatistic.AVERAGE: return "Mathematical average value calculated for the data item during the calculation period.";
                case DataItemStatistic.KURTOSIS: return "**DEPRECATED** in *Version 1.6*. ~~A measure of the 'peakedness' of a probability distribution; i.e., the shape of the distribution curve.~~";
                case DataItemStatistic.MAXIMUM: return "Maximum or peak value recorded for the data item during the calculation period.";
                case DataItemStatistic.MEDIAN: return "Middle number of a series of numbers.";
                case DataItemStatistic.MINIMUM: return "Minimum value recorded for the data item during the calculation period.";
                case DataItemStatistic.MODE: return "Number in a series of numbers that occurs most often.";
                case DataItemStatistic.RANGE: return "Difference between the maximum and minimum value of a data item during the calculation period. Also represents Peak-to-Peak measurement in a waveform.";
                case DataItemStatistic.ROOT_MEAN_SQUARE: return "Mathematical Root Mean Square (RMS) value calculated for the data item during the calculation period.";
                case DataItemStatistic.STANDARD_DEVIATION: return "Statistical Standard Deviation value calculated for the data item during the calculation period.";
            }

            return null;
        }
    }
}