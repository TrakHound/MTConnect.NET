// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Http
{
    /// <summary>
    /// String constants for the MTConnect HTTP request types as defined by Part 1 of the MTConnect
    /// Standard. These are the leading path segments that an MTConnect agent recognises (for
    /// example <c>http://agent/probe</c> or <c>http://agent/device/sample</c>) and that this
    /// library uses to dispatch incoming requests to the appropriate response builder.
    /// </summary>
    public static class MTConnectRequestType
    {
        /// <summary>The MTConnect <c>probe</c> request — returns an <c>MTConnectDevices</c> response document describing the device model.</summary>
        public const string Probe = "probe";

        /// <summary>The MTConnect <c>current</c> request — returns an <c>MTConnectStreams</c> document with the most recent observation for each data item.</summary>
        public const string Current = "current";

        /// <summary>The MTConnect <c>sample</c> request — returns an <c>MTConnectStreams</c> document with the time-ordered observations between two sequence numbers.</summary>
        public const string Sample = "sample";

        /// <summary>The MTConnect <c>assets</c> request — returns an <c>MTConnectAssets</c> document with multiple assets that match the supplied filter.</summary>
        public const string Assets = "assets";

        /// <summary>The MTConnect <c>asset</c> request — returns an <c>MTConnectAssets</c> document for a single asset identified by its <c>assetId</c>.</summary>
        public const string Asset = "asset";
    }
}
