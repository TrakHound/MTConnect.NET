// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect
{
    public static class MTConnectVersions
    {
        /// <summary>
        /// The newest MTConnect Standard version this library advertises
        /// support for. Bumped when a new Version<c>NM</c> constant is
        /// introduced after the v<c>N.M</c> SysML XMI is regenerated.
        /// </summary>
        public static Version Max => Version27;

        public static readonly Version Version10 = new Version(1, 0);
        public static readonly Version Version11 = new Version(1, 1);
        public static readonly Version Version12 = new Version(1, 2);
        public static readonly Version Version13 = new Version(1, 3);
        public static readonly Version Version14 = new Version(1, 4);
        public static readonly Version Version15 = new Version(1, 5);
        public static readonly Version Version16 = new Version(1, 6);
        public static readonly Version Version17 = new Version(1, 7);
        public static readonly Version Version18 = new Version(1, 8);
        public static readonly Version Version20 = new Version(2, 0);
        public static readonly Version Version21 = new Version(2, 1);
        public static readonly Version Version22 = new Version(2, 2);
        public static readonly Version Version23 = new Version(2, 3);
        public static readonly Version Version24 = new Version(2, 4);
        public static readonly Version Version25 = new Version(2, 5);

        /// <summary>
        /// MTConnect Standard v2.6. Adds <c>AssetAdded</c> +
        /// <c>AssociatedAssetId</c> DataItems, <c>CuttingTorch</c> +
        /// <c>Electrode</c> Components, the <c>QIF_MBD</c> media-type
        /// enum value, and narrows the <c>AssetChanged</c> description
        /// to the changed-not-added case.
        /// </summary>
        public static readonly Version Version26 = new Version(2, 6);

        /// <summary>
        /// MTConnect Standard v2.7. Adds <c>BindingState</c>,
        /// <c>Depth</c>, <c>FixtureAssetId</c>, <c>SwingAngle</c> /
        /// <c>SwingDiameter</c> / <c>SwingRadius</c>, <c>TaskAssetId</c>,
        /// <c>WaterHardness</c> DataItems; <c>PinTool</c> +
        /// <c>ToolHolder</c> Components; the <c>Axis</c> / <c>Origin</c>
        /// / <c>Rotation</c> / <c>Scale</c> / <c>Translation</c>
        /// configuration primitives plus their <c>*DataSet</c>
        /// representation siblings; the <c>BindingState</c> Event
        /// observation enum; and Pallet asset measurement description
        /// revisions.
        /// </summary>
        public static readonly Version Version27 = new Version(2, 7);
    }
}