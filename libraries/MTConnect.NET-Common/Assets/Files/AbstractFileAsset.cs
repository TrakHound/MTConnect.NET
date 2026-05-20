// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    public partial class AbstractFileAsset
    {
        /// <summary>
        /// The shared Asset type identifier ("File") used by the file asset family written to the Type attribute and used to recognize these assets during deserialization.
        /// </summary>
        public const string TypeId = "File";


        /// <summary>
        /// Initializes a new AbstractFileAsset, stamping the Asset Type with <see cref="TypeId"/>.
        /// </summary>
        public AbstractFileAsset()
        {
            Type = TypeId;
        }
    }
}
