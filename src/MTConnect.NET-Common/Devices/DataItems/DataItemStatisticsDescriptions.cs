// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    public static class DataItemStatisticDescriptions
    {
        /// <summary>
        /// Statistic Not Set
        /// </summary>
        public const string NONE = "Statistic Not Set";

        /// <summary>
        /// Mathematical Average value calculated for the data item during the calculation period.
        /// </summary>
        public const string AVERAGE = "Mathematical Average value calculated for the data item during the calculation period.";

        /// <summary>
        /// DEPRECATED in Version 1.6. A measure of the "peakedness" of a probability distribution; i.e., the shape of the distribution curve.
        /// </summary>
        public const string KURTOSIS = "DEPRECATED in Version 1.6. A measure of the \"peakedness\" of a probability distribution; i.e., the shape of the distribution curve.";

        /// <summary>
        /// Maximum or peak value recorded for the data item during the calculation period.
        /// </summary>
        public const string MAXIMUM = "Maximum or peak value recorded for the data item during the calculation period.";

        /// <summary>
        /// The middle number of a series of numbers.
        /// </summary>
        public const string MEDIAN = "The middle number of a series of numbers.";
        
        /// <summary>
        /// Minimum value recorded for the data item during the calculation period.
        /// </summary>
        public const string MINIMUM = "Minimum value recorded for the data item during the calculation period.";

        /// <summary>
        /// The number in a series of numbers that occurs most often.
        /// </summary>
        public const string MODE = "The number in a series of numbers that occurs most often.";

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


        public static string Get(DataItemStatistic dataItemStatistic)
        {
            switch (dataItemStatistic)
            {
                case DataItemStatistic.NONE: return NONE;
                case DataItemStatistic.AVERAGE: return AVERAGE;
                case DataItemStatistic.KURTOSIS: return KURTOSIS;
                case DataItemStatistic.MAXIMUM: return MAXIMUM;
                case DataItemStatistic.MEDIAN: return MEDIAN;
                case DataItemStatistic.MINIMUM: return MINIMUM;
                case DataItemStatistic.MODE: return MODE;
                case DataItemStatistic.RANGE: return RANGE;
                case DataItemStatistic.ROOT_MEAN_SQUARE: return ROOT_MEAN_SQUARE;
                case DataItemStatistic.STANDARD_DEVIATION: return STANDARD_DEVIATION;
            }

            return "";
        }
    }
}
