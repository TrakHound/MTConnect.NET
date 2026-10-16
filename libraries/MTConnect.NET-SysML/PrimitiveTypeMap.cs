using System.Collections.Generic;

namespace MTConnect.SysML
{
    /// <summary>
    /// Test-visible seam over the SysML primitive-to-C# type mappings that
    /// used to live inline in two separate switch statements:
    /// <list type="bullet">
    ///   <item>the XMI-id switch in
    ///     <see cref="MTConnectPropertyModel.ParseType"/>, which resolves
    ///     UML built-in primitive types (identified by their Magic Draw
    ///     xmi:id) to their C# equivalents; and</item>
    ///   <item>the named-type switch inside the C# template renderer in
    ///     <c>MTConnect.SysML.CSharp.CSharpTemplateRenderer.Render</c>,
    ///     which remaps SysML primitive names (post-parse) to their
    ///     emitted C# type names.</item>
    /// </list>
    /// The helper lives in the <c>MTConnect.NET-SysML</c> library rather
    /// than in the <c>MTConnect.NET-SysML-Import</c> executable so the
    /// test project can reference it directly. The vendored
    /// MtconnectTranspiler v2.8 primitive tables — <c>int32</c>,
    /// <c>int64</c>, <c>uint32</c>, <c>uint64</c>, <c>double</c>,
    /// <c>version</c>, plus the <c>binary → bool</c>, <c>UUID → string</c>,
    /// and <c>float3d → float[]</c> transforms — are folded in here so
    /// consumers pick them up through the same seam.
    /// </summary>
    public static class PrimitiveTypeMap
    {
        // Per-property overrides. These correspond to specific property
        // instances in the MTConnect SysML model whose declared datatype is
        // wrong or too weakly-typed to render cleanly; the override forces
        // the emitted C# type without altering the source XMI. Vendored
        // one-to-one from the pre-swap `switch (propertyId)` at
        // MTConnectPropertyModel.ParseType so the diff is a pure move.
        private static readonly Dictionary<string, string> _byPropertyId = new()
        {
            // PhysicalAsset.Measurements: the model's single Measurement class is emitted twice, as
            // CuttingTools.Measurement and as Pallet.Measurement (the physical-asset measurements); the
            // name lookup picks the CuttingTools one
            ["_2024x_68e0225_1727795745690_23400_24086"] = "MTConnect.Assets.Pallet.IMeasurement",

            // RawMaterials.RawMateral.CurrentVolume
            ["_19_0_3_68e0225_1618831247227_54016_392"] = "double",

            // RawMaterials.RawMateral.CurrentDimension
            ["_19_0_3_68e0225_1622116618964_666287_1642"] = "MILLIMETER_3D",

            // RawMaterials.RawMateral.InitialVolume
            ["_19_0_3_68e0225_1618831175692_489264_387"] = "double",

            // RawMaterials.RawMateral.InitialDimension
            ["_19_0_3_68e0225_1622116618960_627070_1641"] = "MILLIMETER_3D",

            // CuttingTools.ProcessFeedRate.Value (incorrect in MTConnect Model 2.2)
            ["_19_0_3_68e0225_1636117526335_679126_67"] = "double",
        };

        // UML built-in primitive-type XMI ids. These are the stable Magic
        // Draw ids the source SysML model uses for the shipped UML
        // primitives (string, integer, boolean, real, DateTime, ID). The
        // v2.8 vendored transpiler extends this table with the additional
        // ids listed below.
        private static readonly Dictionary<string, string> _byTypeId = new()
        {
            // string
            ["_19_0_3_91b028d_1579272360416_763325_681"] = "string",

            // integer
            ["_19_0_3_91b028d_1579272271512_537408_674"] = "int",

            // boolean
            ["_19_0_3_91b028d_1579278876899_683310_3821"] = "bool",

            // float
            ["_19_0_3_91b028d_1579272506322_914606_702"] = "double",

            // double
            ["_19_0_3_68e0225_1678197512818_76309_18111"] = "double",

            // DateTime
            ["_19_0_3_91b028d_1579272233011_597138_670"] = "System.DateTime",

            // Description
            ["EAID_64352755_7251_46af_846D_937E5A1E3949"] = "Description",

            // ID
            ["_19_0_3_91b028d_1579272245466_691733_672"] = "string",

            // DataItemTypeEnum
            ["_19_0_3_45f01b9_1579563576485_587701_22033"] = "string",

            // DataItemSubTypeEnum
            ["_19_0_3_45f01b9_1579563592155_977172_22064"] = "string",
        };

        // Named-type transforms. The historical MTConnect-specific coordinate
        // structs (UNIT_VECTOR_3D, POSITION_3D, DEGREE_3D, MILLIMETER_3D)
        // and the *Enum → string coercions live here alongside the vendored
        // v2.8 primitive names — one lookup path for every downstream
        // template renderer.
        //
        // NOTE ordering: the SysML model declares some primitives under a
        // UML PrimitiveType element AND under a name that appears here.
        // MapByTypeName is called AFTER the type-id lookup, so a hit here
        // wins only when the id-lookup produced no match. That is the
        // desired precedence — the id lookup carries per-model semantics
        // (integer → int) while the name lookup covers the v2.8 primitive
        // names not yet keyed by a stable xmi:id.
        private static readonly Dictionary<string, string> _byTypeName = new()
        {
            // Historical MTConnect renderer coercions (pre-vendor).
            ["UnitsEnum"] = "string",
            ["NativeUnitsEnum"] = "string",
            ["MeasurementCodeEnum"] = "string",
            ["UNIT_VECTOR_3D"] = "MTConnect.UnitVector3D",
            ["POSITION_3D"] = "MTConnect.Position3D",
            ["DEGREE_3D"] = "MTConnect.Degree3D",
            ["MILLIMETER_3D"] = "MTConnect.Millimeter3D",
            ["QIFDocument"] = "string",

            // Vendored MtconnectTranspiler v2.8 primitives — folded in so
            // any future SysML model that lands one of these names as a
            // property type is emitted as a first-class C# type rather
            // than falling through to `string`.
            ["int32"] = "int",
            ["int64"] = "long",
            ["uint32"] = "uint",
            ["uint64"] = "ulong",
            ["double"] = "double",
            ["version"] = "System.Version",

            // Vendored MtconnectTranspiler v2.8 transforms — the name on
            // the SysML side has a canonical C# equivalent the transpiler
            // has always emitted; folding them in here means the local
            // renderer does not have to case-split the same way.
            ["binary"] = "bool",
            ["UUID"] = "string",
            ["float3d"] = "float[]",
        };

        // Default-value literals per emitted C# type. Only the types this
        // helper is authoritative over are keyed — a caller that hands in
        // a bespoke class or an untracked type gets a null result and
        // must decide its own fallback (typically the C# default(T)
        // expression at the call site). Values are the source-code
        // literal (e.g. `0`, `0L`, `false`, `null`) that a template can
        // splice into a generated field/property initializer without
        // further quoting.
        private static readonly Dictionary<string, string> _defaultValueLiteral = new()
        {
            ["string"] = "null",
            ["int"] = "0",
            ["long"] = "0L",
            ["uint"] = "0u",
            ["ulong"] = "0uL",
            ["bool"] = "false",
            ["double"] = "0.0",
            ["float"] = "0.0f",
            ["float[]"] = "null",
            ["System.DateTime"] = "default(System.DateTime)",
            ["System.Version"] = "null",
            ["MTConnect.UnitVector3D"] = "default(MTConnect.UnitVector3D)",
            ["MTConnect.Position3D"] = "default(MTConnect.Position3D)",
            ["MTConnect.Degree3D"] = "default(MTConnect.Degree3D)",
            ["MTConnect.Millimeter3D"] = "default(MTConnect.Millimeter3D)",
            ["Description"] = "null",
        };

        /// <summary>
        /// Looks up a per-property XMI-id override — the case where a
        /// specific property in the SysML model has an incorrect declared
        /// datatype and the fork forces the C# emission to a corrected
        /// primitive. Returns <c>null</c> when no override applies (the
        /// vast majority of properties).
        /// </summary>
        /// <param name="propertyId">The XMI id of the property.</param>
        /// <returns>The forced C# type, or <c>null</c> when the property
        /// has no override.</returns>
        public static string? MapByPropertyId(string? propertyId)
        {
            if (string.IsNullOrEmpty(propertyId)) return null;
            return _byPropertyId.TryGetValue(propertyId!, out var mapped) ? mapped : null;
        }

        /// <summary>
        /// Looks up a UML built-in primitive by its XMI id and returns
        /// the corresponding C# type name. Returns <c>null</c> for any
        /// id outside the built-in primitive set — the caller is
        /// expected to fall through to a class / enum lookup.
        /// </summary>
        /// <param name="typeId">The XMI id of the property's declared
        /// type (a UML PrimitiveType id).</param>
        /// <returns>The mapped C# type, or <c>null</c> when the id is
        /// not a known primitive.</returns>
        public static string? MapByTypeId(string? typeId)
        {
            if (string.IsNullOrEmpty(typeId)) return null;
            return _byTypeId.TryGetValue(typeId!, out var mapped) ? mapped : null;
        }

        /// <summary>
        /// Looks up a SysML type by its name and returns the emitted C#
        /// type. Covers both the historical MTConnect renderer coercions
        /// (<c>UnitsEnum → string</c>, <c>UNIT_VECTOR_3D →
        /// MTConnect.UnitVector3D</c>, …) and the vendored
        /// MtconnectTranspiler v2.8 primitives (<c>int32 → int</c>,
        /// <c>version → System.Version</c>, <c>binary → bool</c>, …).
        /// Returns <c>null</c> for any name the map does not recognise
        /// so the caller can fall through to a class / enum lookup and
        /// finally to the <c>string</c> default.
        /// </summary>
        /// <param name="typeName">The SysML type name as it appears on
        /// the property after id-based parsing.</param>
        /// <returns>The mapped C# type, or <c>null</c> when the name is
        /// not a known primitive or coerced enum.</returns>
        public static string? MapByTypeName(string? typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            return _byTypeName.TryGetValue(typeName!, out var mapped) ? mapped : null;
        }

        /// <summary>
        /// Returns the C# source-code literal that represents the
        /// default value for the given emitted C# type — for use by
        /// template renderers that need to splice a compile-time
        /// initializer. Returns <c>null</c> for any type outside the
        /// primitive / coordinate-struct set the map is authoritative
        /// over.
        /// </summary>
        /// <param name="csharpType">The emitted C# type name (as
        /// returned by <see cref="MapByTypeId"/> or
        /// <see cref="MapByTypeName"/>).</param>
        /// <returns>A source-code literal (e.g. <c>"0"</c>,
        /// <c>"null"</c>, <c>"false"</c>), or <c>null</c> when the
        /// type is not in the primitive set.</returns>
        public static string? DefaultValueLiteral(string? csharpType)
        {
            if (string.IsNullOrEmpty(csharpType)) return null;
            return _defaultValueLiteral.TryGetValue(csharpType!, out var literal) ? literal : null;
        }
    }
}
