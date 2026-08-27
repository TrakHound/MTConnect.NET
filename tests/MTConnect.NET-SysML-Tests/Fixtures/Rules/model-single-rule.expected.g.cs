// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = uml-r-1

namespace MTConnect.Devices
{
    /// <summary>
    /// A Bar with a single rule.
    /// </summary>
    public class Bar : IBar
    {
        /// <summary>
        /// The description of this type as defined by the MTConnect Standard.
        /// </summary>
        public const string DescriptionText = "A Bar with a single rule.";

        /// <summary>
        /// The OCL constraint bodies attached to this type in the source
        /// SysML model, preserved verbatim so downstream consumers can
        /// inspect the spec's raw validation rules at runtime.
        /// </summary>
        public static readonly string[] Rules = new[]
        {
            "self.foo->size() > 0"
        };

    }
}
