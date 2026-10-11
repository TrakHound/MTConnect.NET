// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579100679936_1279_16310

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004
    /// </summary>
    public class CoordinateSystem : ICoordinateSystem
    {
        /// <summary>
        /// The description of this type as defined by the MTConnect Standard.
        /// </summary>
        public const string DescriptionText = "Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004";

        /// <summary>
        /// The OCL constraint bodies attached to this type in the source
        /// SysML model, preserved verbatim so downstream consumers can
        /// inspect the spec's raw validation rules at runtime.
        /// </summary>
        public static readonly string[] Rules = new[]
        {
            "val:CoordinateSystemOriginOrTransformationExclusiveOptional\n    a sh:NodeShape ;\n    sh:message \"`CoordinateSystem` may have either an `Origin` or a `Transformation` but not both.\" ;\n    sh:targetClass mt:CoordinateSystem ;\n\n    sh:property [\n        sh:path mt:hasOrigin ;\n        sh:maxCount 1 ;\n        sh:class mt:Origin ;\n    ] ;\n\n    sh:property [\n        sh:path mt:hasTransformation ;\n        sh:maxCount 1 ;\n        sh:class mt:Transformation ;\n    ] ;\n    sh:sparql [\n        a sh:SPARQLConstraint ;\n        sh:select \"\"\"\n            SELECT $this\n            WHERE {\n                OPTIONAL { $this mt:hasOrigin ?origin . }\n                OPTIONAL { $this mt:hasTransformation ?trans . }\n                FILTER (BOUND(?origin) && BOUND(?trans))\n            }\n        \"\"\" ;\n    ] ."
        };


        /// <summary>
        /// Natural language description of the CoordinateSystem.
        /// </summary>
        public string Description { get; set; }
        

        /// <summary>
        /// Unique identifier for the coordinate system.
        /// </summary>
        public string Id { get; set; }
        

        /// <summary>
        /// Name of the coordinate system.
        /// </summary>
        public string Name { get; set; }
        

        /// <summary>
        /// Manufacturer's name or users name for the coordinate system.
        /// </summary>
        public string NativeName { get; set; }
        

        /// <summary>
        /// Coordinates of the origin position of a coordinate system.
        /// </summary>
        public MTConnect.Devices.Configurations.IAbstractOrigin Origin { get; set; }
        

        /// <summary>
        /// Id.
        /// </summary>
        public string ParentIdRef { get; set; }
        

        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public MTConnect.Devices.Configurations.ITransformation Transformation { get; set; }
        

        /// <summary>
        /// Type of coordinate system.
        /// </summary>
        public MTConnect.Devices.Configurations.CoordinateSystemType Type { get; set; }
        

        /// <summary>
        /// UUID for the coordinate system.
        /// </summary>
        public string Uuid { get; set; }
    }
}