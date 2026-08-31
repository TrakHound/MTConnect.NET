// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579103900791_417826_16362

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
    /// </summary>
    public class Transformation : ITransformation
    {
        /// <summary>
        /// The description of this type as defined by the MTConnect Standard.
        /// </summary>
        public const string DescriptionText = "Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.";

        /// <summary>
        /// The OCL constraint bodies attached to this type in the source
        /// SysML model, preserved verbatim so downstream consumers can
        /// inspect the spec's raw validation rules at runtime.
        /// </summary>
        public static readonly string[] Rules = new[]
        {
            "val:TransformationMustHaveRotationOrTranslation\n    a sh:NodeShape ;\n    sh:message \"`Transformation` MUST have at least one of `Rotation` or `Translation` defined, and neither can be multiply defined.\" ;\n    sh:targetClass mt:Transformation ;\n\n    sh:property [\n        sh:path mt:hasRotation ;\n        sh:maxCount 1 ;\n        sh:class mt:Rotation ;\n    ] ;\n    sh:property [\n        sh:path mt:hasTranslation ;\n        sh:maxCount 1 ;\n        sh:class mt:Translation ;\n    ] ;\n\n    sh:or (\n        [ sh:property [\n            sh:path mt:hasRotation ;\n            sh:minCount 1 ;\n        ] ]\n        [ sh:property [\n            sh:path mt:hasTranslation ;\n            sh:minCount 1 ;\n        ] ]\n    ) ."
        };


        /// <summary>
        /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.
        /// </summary>
        public MTConnect.Devices.Configurations.IAbstractRotation Rotation { get; set; }
        

        /// <summary>
        /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.
        /// </summary>
        public MTConnect.Devices.Configurations.IAbstractTranslation Translation { get; set; }
    }
}