// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The Operating System of a component.
    /// </summary>
    public class OperatingSystemModel
    {
        public OperatingSystemType Type { get; set; }
        public IDataItemModel TypeDataItem { get; set; }

        /// <summary>
        /// The corporate identity for the maker of the hardware or software.
        /// </summary>
        public string Manufacturer { get; set; }
        public IDataItemModel ManufacturerDataItem { get; set; }

        /// <summary>
        /// The license code to validate or activate the hardware or software
        /// </summary>
        public string License { get; set; }
        public IDataItemModel LicenseDataItem { get; set; }

        /// <summary>
        /// The version of the hardware or software.
        /// </summary>
        public string Version { get; set; }
        public IDataItemModel VersionDataItem { get; set; }

        /// <summary>
        /// The date the hardware or software was released for general use.
        /// </summary>
        public string ReleaseDate { get; set; }
        public IDataItemModel ReleaseDateDataItem { get; set; }

        /// <summary>
        /// The date the hardware or software was installed.
        /// </summary>
        public string InstallDate { get; set; }
        public IDataItemModel InstallDateDataItem { get; set; }
    }
}
