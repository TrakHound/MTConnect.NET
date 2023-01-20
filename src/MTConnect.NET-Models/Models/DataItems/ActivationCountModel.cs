// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// Accumulation of the number of times a function has attempted to, or is planned to attempt to, activate or be performed.
    /// </summary>
    public class ActivationCountModel
    {
        /// <summary>
        /// An accumulation representing all actions, items, or activities being counted independent of the outcome.ALL is the default subType.
        /// </summary>
        public int All { get; set; }
        public IDataItemModel AllDataItem { get; set; }

        /// <summary>
        /// An accumulation representing actions, items, or activities being counted that do not conform to specification or expectation.
        /// </summary>
        public int Bad { get; set; }
        public IDataItemModel BadDataItem { get; set; }

        /// <summary>
        /// An accumulation representing actions, items, or activities being counted that conform to specification or expectation.
        /// </summary>
        public int Good { get; set; }
        public IDataItemModel GoodDataItem { get; set; }

        /// <summary>
        /// The goal of the operation or process.
        /// </summary>
        public int Target { get; set; }
        public IDataItemModel TargetDataItem { get; set; }

        /// <summary>
        /// An accumulation representing actions, items, or activities yet to be counted.
        /// </summary>
        public int Remaining { get; set; }
        public IDataItemModel RemainingDataItem { get; set; }

        /// <summary>
        /// An accumulation representing actions, items, or activities that have been completed, independent of the outcome.
        /// </summary>
        public int Complete { get; set; }
        public IDataItemModel CompleteDataItem { get; set; }

        /// <summary>
        /// An accumulation representing actions or activities that were attempted, but failed to complete or resulted in an unexpected or unacceptable outcome.
        /// </summary>
        public int Failed { get; set; }
        public IDataItemModel FailedDataItem { get; set; }

        /// <summary>
        /// An accumulation representing actions or activities that were attempted, but terminated before they could be completed.
        /// </summary>
        public int Aborted { get; set; }
        public IDataItemModel AbortedDataItem { get; set; }
    }
}
