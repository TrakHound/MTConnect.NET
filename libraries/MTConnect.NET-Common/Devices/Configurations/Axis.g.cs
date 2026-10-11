// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_3_3870182_1764951682685_285104_645

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Axis along or around which the Component moves relative to a coordinate system.
    /// </summary>
    public class Axis : AbstractAxis, IAxis
    {
        /// <summary>
        /// The description of this type as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Axis along or around which the Component moves relative to a coordinate system.";

        /// <summary>
        /// The OCL constraint bodies attached to this type in the source
        /// SysML model, preserved verbatim so downstream consumers can
        /// inspect the spec's raw validation rules at runtime.
        /// </summary>
        public static readonly string[] Rules = new[]
        {
            "val:AxisValueMustBeUnitVector\n    a sh:NodeShape ;\n    sh:message \"Axis value must be a unit vector.\" ;\n    sh:targetClass mt:Axis ;\n    sh:sparql [\n        a sh:SPARQLConstraint ;\n        sh:message \"'value' property must form a unit vector: sqrt(x^2 + y^2 + z^2) = 1.\" ;\n        sh:select \"\"\"\n            SELECT $this\n            WHERE {\n                $this mt:value ?vec .\n                ?vec mt:x ?x ; mt:y ?y ; mt:z ?z .\n                FILTER ( ABS( SQRT((?x*?x) + (?y*?y) + (?z*?z)) - 1.0 ) > 1e-6 )\n            }\n        \"\"\" ;\n    ] .\n"
        };


        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}