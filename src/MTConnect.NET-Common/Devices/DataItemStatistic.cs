// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public enum DataItemStatistic
    {
        /// <summary>
        /// Statistic Not Set
        /// </summary>
        NONE,

        /// <summary>
        /// Mathematical Average value calculated for the data item during the calculation period.
        /// </summary>
        AVERAGE,

        /// <summary>
        /// DEPRECATED in Version 1.6. A measure of the "peakedness" of a probability distribution; i.e., the shape of the distribution curve.
        /// </summary>
        KURTOSIS,

        /// <summary>
        /// Maximum or peak value recorded for the data item during the calculation period.
        /// </summary>
        MAXIMUM,

        /// <summary>
        /// The middle number of a series of numbers.
        /// </summary>
        MEDIAN,

        /// <summary>
        /// Minimum value recorded for the data item during the calculation period.
        /// </summary>
        MINIMUM,

        /// <summary>
        /// The number in a series of numbers that occurs most often.
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
        STANDARD_DEVIATION
    }
}
