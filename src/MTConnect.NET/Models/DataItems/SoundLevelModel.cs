// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Streams.Samples;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of a sound level or sound pressure level relative to atmospheric pressure.
    /// </summary>
    public class SoundLevelModel
    {
        /// <summary>
        /// No weighting factor on the frequency scale
        /// </summary>
        public SoundLevelValue NoScale { get; set; }
        public IDataItemModel NoScaleDataItem { get; set; }

        /// <summary>
        /// A Scale weighting factor. This is the default weighting factor if no factor is specified
        /// </summary>
        public SoundLevelValue AScale { get; set; }
        public IDataItemModel AScaleDataItem { get; set; }

        /// <summary>
        /// B Scale weighting factor
        /// </summary>
        public SoundLevelValue BScale { get; set; }
        public IDataItemModel BScaleDataItem { get; set; }

        /// <summary>
        /// C Scale weighting factor
        /// </summary>
        public SoundLevelValue CScale { get; set; }
        public IDataItemModel CScaleDataItem { get; set; }

        /// <summary>
        /// D Scale weighting factor
        /// </summary>
        public SoundLevelValue DScale { get; set; }
        public IDataItemModel DScaleDataItem { get; set; }
    }
}
