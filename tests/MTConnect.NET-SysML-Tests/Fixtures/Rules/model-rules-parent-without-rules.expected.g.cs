// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = uml-r-axis

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// A TestAxis whose parent has no rules.
    /// </summary>
    public class TestAxis : AbstractTestAxis, ITestAxis
    {
        /// <summary>
        /// The description of this type as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "A TestAxis whose parent has no rules.";

        /// <summary>
        /// The OCL constraint bodies attached to this type in the source
        /// SysML model, preserved verbatim so downstream consumers can
        /// inspect the spec's raw validation rules at runtime.
        /// </summary>
        public static readonly string[] Rules = new[]
        {
            "self.value->size() > 0"
        };

    }
}