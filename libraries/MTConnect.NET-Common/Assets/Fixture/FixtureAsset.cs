// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Assets.Fixture
{
    public partial class FixtureAsset
    {
        /// <summary>
        /// The fixed Asset type identifier ("Fixture") written to the Type attribute and used to recognize this asset during deserialization.
        /// </summary>
        public const string TypeId = "Fixture";


        /// <summary>
        /// Initializes a new FixtureAsset, stamping the Asset Type with <see cref="TypeId"/>.
        /// </summary>
        public FixtureAsset()
        {
            Type = TypeId;
        }


        /// <summary>
        /// Excludes the asset from documents before MTConnect 2.5, which do not define it.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version the response document targets.</param>
        protected override IAsset OnProcess(Version mtconnectVersion)
        {
            if (mtconnectVersion < MTConnectVersions.Version25) return null;

            return base.OnProcess(mtconnectVersion);
        }
    }
}
