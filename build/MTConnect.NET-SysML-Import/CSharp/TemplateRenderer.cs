using MTConnect.SysML.Models.Assets;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Models.Observations;
using System.Collections;

namespace MTConnect.SysML.CSharp
{
    public static class CSharpTemplateRenderer
    {
        public static void Render(MTConnectModel mtconnectModel, string outputPath)
        {
            if (mtconnectModel != null && !string.IsNullOrEmpty(outputPath))
            {
                var exportModels = GetExportModels(mtconnectModel);
                if (exportModels != null)
                {
                    var classModels = exportModels.Where(o => typeof(MTConnectClassModel).IsAssignableFrom(o.GetType())).Select(o => (MTConnectClassModel)o);

                    var dClassModels = new Dictionary<string, MTConnectClassModel>();
                    foreach (var classModel in classModels)
                    {
                        if (!string.IsNullOrEmpty(classModel.Name))
                        {
                            // TryAdd preserves first-wins semantics with a single hash lookup.
                            dClassModels.TryAdd(classModel.Name, classModel);
                        }
                    }


                    var enumModels = exportModels.Where(o => typeof(MTConnectEnumModel).IsAssignableFrom(o.GetType())).Select(o => (MTConnectEnumModel)o);

                    var dEnumModels = new Dictionary<string, MTConnectEnumModel>();
                    foreach (var enumModel in enumModels)
                    {
                        if (!string.IsNullOrEmpty(enumModel.Name))
                        {
                            dEnumModels.TryAdd(enumModel.Name, enumModel);
                        }
                    }


                    var templates = new List<ITemplateModel>();

                    foreach (var exportModel in exportModels)
                    {
                        var type = exportModel.GetType();

                        if (typeof(MTConnectClassModel).IsAssignableFrom(type))
                        {
                            var classModel = (MTConnectClassModel)exportModel;
                            if (classModel.Properties != null)
                            {
                                foreach (var property in classModel.Properties)
                                {
                                    switch (property.DataType)
                                    {
                                        case "UnitsEnum": property.DataType = "string"; break;
                                        case "NativeUnitsEnum": property.DataType = "string"; break;
                                        case "MeasurementCodeEnum": property.DataType = "string"; break;
                                        case "UNIT_VECTOR_3D": property.DataType = "MTConnect.UnitVector3D"; break;
                                        case "POSITION_3D": property.DataType = "MTConnect.Position3D"; break;
                                        case "DEGREE_3D": property.DataType = "MTConnect.Degree3D"; break;
                                        case "MILLIMETER_3D": property.DataType = "MTConnect.Millimeter3D"; break;
                                        case "QIFDocument": property.DataType = "string"; break;
                                        default:

                                            var classMatch = dClassModels.GetValueOrDefault(property.DataType);
                                            if (classMatch != null)
                                            {
                                                property.DataType = $"{NamespaceHelper.GetNamespace(classMatch.Id)}.I{classMatch.Name}";
                                            }
                                            else
                                            {
                                                var enumMatch = dEnumModels.GetValueOrDefault(property.DataType);
                                                if (enumMatch != null)
                                                {
                                                    property.DataType = $"{NamespaceHelper.GetNamespace(enumMatch.Id)}.{enumMatch.Name}";
                                                }
                                            }

                                            break;                               
                                    }
                                }
                            }
                        }


                        ITemplateModel template = null;

                        if (typeof(MTConnectInterfaceDataItemType).IsAssignableFrom(type)) template = InterfaceDataItemType.Create((MTConnectInterfaceDataItemType)exportModel);
                        else if (typeof(MTConnectDataItemType).IsAssignableFrom(type)) template = DataItemType.Create((MTConnectDataItemType)exportModel);
                        else if (typeof(MTConnectCompositionType).IsAssignableFrom(type)) template = CompositionType.Create((MTConnectCompositionType)exportModel);
                        else if (typeof(MTConnectComponentType).IsAssignableFrom(type))
                        {
                            // Surface the SysML-declared introduction year
                            // for every Component, including the Controllers
                            // organizer AssociationClass (introduced='2.0').
                            // The earlier override that hard-coded Controllers
                            // to v1.0 contradicted the spec — the
                            // `Profile:normative introduced='2.0'` record
                            // on the Controllers UML AssociationClass is the
                            // authoritative source.
                            template = ComponentType.Create((MTConnectComponentType)exportModel);
                        }
                        else if (typeof(MTConnectMeasurementModel).IsAssignableFrom(type))
                        {
                            if (exportModel.Id?.StartsWith("Assets.CuttingTools.") == true)
                            {
                                template = CuttingToolMeasurementModel.Create((MTConnectMeasurementModel)exportModel);
                            }
                            else if (exportModel.Id?.StartsWith("Assets.Pallet.") == true)
                            {
                                template = MeasurementModel.Create((MTConnectMeasurementModel)exportModel);
                            }
                            // No fallback: every measurement in the v2.x SysML routes
                            // through one of the two prefixes above. A future model
                            // adding a third measurement package will surface here as a
                            // null template (NullReferenceException downstream) — preferable
                            // to a silent drop with a stderr warning that nothing watches.
                        }
                        else if (typeof(MTConnectClassModel).IsAssignableFrom(type) && exportModel.Id?.EndsWith("Result") == true)
                        {
                            // Suffix-based DataSetResult selector. Type guard required because the recursive
                            // GetExportModels walk surfaces both classes AND properties; a property whose Id
                            // happens to end in "Result" (e.g. `Devices.Configurations.DataSet.Result` — the
                            // `result` field on the v2.7 DataSet base class) would otherwise crash with
                            // InvalidCastException when forced into MTConnectClassModel.
                            template = DataSetResultModel.Create((MTConnectClassModel)exportModel);
                        }
                        else if (typeof(MTConnectClassModel).IsAssignableFrom(type)) template = ClassModel.Create((MTConnectClassModel)exportModel);
                        else if (typeof(MTConnectObservationModel).IsAssignableFrom(type)) template = ObservationModel.Create((MTConnectObservationModel)exportModel);
                        else if (typeof(MTConnectEnumModel).IsAssignableFrom(type))
                        {
                            switch (exportModel.Id)
                            {
                                case "Devices.UnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Devices.NativeUnitsEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertUnitEnum); break;
                                case "Assets.CuttingTools.Measurements.MeasurementCodeEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertMeasurementCodeEnum); break;
                                //case "Assets.Pallet.Measurements.MeasurementCodeEnum": template = EnumStringModel.Create((MTConnectEnumModel)exportModel, ConvertMeasurementCodeEnum); break;
                                default: template = EnumModel.Create((MTConnectEnumModel)exportModel); break;
                            }
                        }


                        if (template != null)
                        {
                            switch (template.Id)
                            {
                                case "Devices.Device":
                                    // Devices.Device's concrete C# base is wiped here; Component
                                    // is reintroduced through the hand-authored partial-class
                                    // file. Additional generalizations are cleared too so that
                                    // none of them survive into the generated header.
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).ParentName = null;
                                    ((ClassModel)template).AdditionalParentNames = new System.Collections.Generic.List<string>();
                                    ((ClassModel)template).AdditionalParentUmlIds = new System.Collections.Generic.List<string>();
                                    break;

                                case "Devices.Component": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.Composition": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.DataItem": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.AbstractDataItemRelationship": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.References.Reference": ((ClassModel)template).IsPartial = true; break;
                                case "Devices.References.ComponentReference": ((ClassModel)template).HasDescriptions = false; break;
                                case "Devices.References.DataItemReference": ((ClassModel)template).HasDescriptions = false; break;
                                case "Devices.Units": ((EnumStringModel)template).IsPartial = true; break;

                                case "Devices.DataItems.ToolOffsetsDataItem": ((DataItemType)template).Representation = "TABLE"; break;
                                case "Devices.DataItems.WorkOffsetsDataItem": ((DataItemType)template).Representation = "TABLE"; break;

                                //case "Devices.DataItemResetTrigger": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NONE" }); break;
                                //case "Devices.DataItemStatistic": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NONE" }); break;
                                //case "Devices.Configurations.CriticalityType": ((EnumModel)template).Values.Add(new MTConnectEnumValueModel { Name = "NOT_SPECIFIED" }); break;

                                case "Assets.Asset": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.PhysicalAsset": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.ComponentConfigurationParameters.ComponentConfigurationParameters":
                                case "Assets.CuttingTools.CuttingTool":
                                case "Assets.CuttingTools.CuttingToolArchetype":
                                case "Assets.Files.File":
                                case "Assets.Files.FileArchetype":
                                case "Assets.Files.AbstractFile":
                                case "Assets.QIF.QIFDocumentWrapper":
                                case "Assets.RawMaterials.RawMaterial":
                                    ApplyAssetSuffix((ClassModel)template, alsoSuffixParent: true);
                                    break;
                                case "Assets.Fixture.Fixture":
                                case "Assets.Pallet.Pallet":
                                    ApplyAssetSuffix((ClassModel)template, alsoSuffixParent: false);
                                    break;
                                case "Assets.CuttingTools.CuttingToolLifeCycle": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.CuttingItem": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.ToolLife": ((ClassModel)template).IsPartial = true; break;
                                case "Assets.CuttingTools.Measurement":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).IsAbstract = false;
                                    break;
                                case "Assets.CuttingTools.ToolingMeasurement":
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).IsAbstract = false;
                                    break;
                                case "Assets.Pallet.Measurement":
                                    // Partial + concrete so a hand-written
                                    // partial can supply the `Type` property
                                    // and the `Measurement(IMeasurement)` ctor
                                    // that the per-subtype rich template
                                    // (`Pallets.Measurement.scriban`) chains
                                    // to via `: base(measurement)`.
                                    ((ClassModel)template).IsPartial = true;
                                    ((ClassModel)template).IsAbstract = false;
                                    break;
                            }

                            templates.Add(template);
                        }
                    }

                    if (templates != null)
                    {
                        var componentModel = templates.FirstOrDefault(o => o.Id == "Devices.Component");
                        if (componentModel != null)
                        {
                            // Add special template for the IContainer interface
                            var containerModel = new ClassModel();
                            containerModel.Id = "Devices.Container";
                            containerModel.Name = "Container";
                            containerModel.IsPartial = true;
                            containerModel.HasModel = false;
                            containerModel.HasDescriptions = false;
                            foreach (var property in (((ClassModel)componentModel).Properties ?? Enumerable.Empty<PropertyModel>()).Where(o => o.Name != "Components" && o.Name != "Compositions"))
                            {
                                containerModel.Properties.Add(PropertyModel.Create(property));
                            }
                            templates.Add(containerModel);


                            var deviceModel = templates.FirstOrDefault(o => o.Id == "Devices.Device");
                            if (deviceModel != null)
                            {
                                // Remove redundant Properties (inherits from IComponent)
                                foreach (var property in ((ClassModel)deviceModel).Properties)
                                {
                                    if (property.Name == "Hash") property.ExportToInterface = false;

                                    if (((ClassModel)componentModel).Properties.Any(o => o.Name == property.Name))
                                    {
                                        property.ExportToInterface = false;
                                    }
                                }
                            }

                            var compositionModel = templates.FirstOrDefault(o => o.Id == "Devices.Composition");
                            if (compositionModel != null)
                            {
                                // Remove redundant Properties (inherits from IContainer)
                                foreach (var property in ((ClassModel)compositionModel).Properties)
                                {
                                    if (containerModel.Properties.Any(o => o.Name == property.Name))
                                    {
                                        property.ExportToInterface = false;
                                    }
                                }
                            }

                            // Remove redundant Properties (inherits from IContainer)
                            foreach (var property in (((ClassModel)componentModel).Properties ?? Enumerable.Empty<PropertyModel>()).Where(o => o.Name != "Components" && o.Name != "Compositions"))
                            {
                                property.ExportToInterface = false;
                            }
                        }

                        // Mark each property whose Name matches an inherited member with
                        // IsInherited=true so Model.scriban / Interface.scriban /
                        // Observations.DataSetResults.scriban emit the `new` modifier on
                        // the declaration. Without `new`, the C# compiler raises CS0108
                        // ("hides inherited member; use the new keyword if hiding was
                        // intended"). The walk + hand-stitched seeds cover both SysML-
                        // declared generalizations and inheritance links the SysML model
                        // does not express (hand-written partials such as
                        // `IContainer : IMTConnectEntity`, `IComposition : IContainer`,
                        // and the `Observation` base of every DataSetResult).
                        //
                        // The full import-side classModels list is passed too so the
                        // walk can find parents that are present in the SysML graph but
                        // never reach `templates` (e.g. `Assets.CuttingTools.Measurement`,
                        // an abstract base whose .g.cs is hand-maintained / frozen and
                        // therefore not re-emitted by any current renderer flow). The
                        // child ToolingMeasurement still extends it at C# compile time,
                        // so its `Code` property hides Measurement.Code and needs `new`.
                        MarkInheritedProperties(templates, classModels);


                        foreach (var template in templates)
                        {
                            WriteModel(template, outputPath);
                            WriteInterface(template, outputPath);
                            WriteDescriptions(template, outputPath);
                        }
                    }
                }
            }
        }


        private static IEnumerable<IMTConnectExportModel> GetExportModels(object model)
        {
            // Track visited reference-type instances to break cycles. The
            // SysML model graph is generated and can contain back-references
            // (e.g. parent ⇄ child) which would otherwise drive an unbounded
            // recursion → StackOverflowException. HashSet keyed by
            // reference equality so two distinct strings or value-typed
            // boxes don't collide on Equals.
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            var exportModels = new List<IMTConnectExportModel>();
            CollectExportModels(model, exportModels, visited);
            return exportModels;
        }

        private static void CollectExportModels(object model, List<IMTConnectExportModel> exportModels, HashSet<object> visited)
        {
            if (model == null) return;

            var modelType = model.GetType();

            // Skip primitives, strings, and value types early. Strings are
            // IEnumerable<char> and would otherwise be walked character-by-
            // character, exploding the recursion; value types neither
            // participate in cycles nor implement IMTConnectExportModel.
            if (modelType.IsPrimitive || modelType.IsValueType || modelType == typeof(string)) return;

            if (!visited.Add(model)) return;

            if (typeof(IMTConnectExportModel).IsAssignableFrom(modelType))
            {
                exportModels.Add((IMTConnectExportModel)model);
            }

            var properties = modelType.GetProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyValue = property.GetValue(model);
                    if (propertyValue != null)
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                        {
                            IEnumerable childValues = (IEnumerable)propertyValue;
                            foreach (var childValue in childValues)
                            {
                                CollectExportModels(childValue, exportModels, visited);
                            }
                        }
                        else
                        {
                            CollectExportModels(propertyValue, exportModels, visited);
                        }
                    }
                }
            }
        }


        private static void WriteModel(ITemplateModel template, string outputPath)
        {
            if (template != null)
            {
                var result = template.RenderModel();
                if (result != null && template.Id != null)
                {
                    var resultPath = template.Id.Replace('.', Path.DirectorySeparatorChar);
                    resultPath = Path.Combine(outputPath, resultPath);
                    resultPath = $"{resultPath}.g.cs";

                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    if (!string.IsNullOrEmpty(resultDirectory) && !Directory.Exists(resultDirectory))
                        Directory.CreateDirectory(resultDirectory);

                    File.WriteAllText(resultPath, result);
                }
            }
        }

        private static void WriteInterface(ITemplateModel template, string outputPath)
        {
            if (template != null)
            {
                var result = template.RenderInterface();
                if (result != null && template.Id != null)
                {
                    var resultPath = template.Id.Replace('.', Path.DirectorySeparatorChar);
                    resultPath = Path.Combine(outputPath, resultPath);
                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    var resultFilename = Path.GetFileName(resultPath);
                    resultPath = Path.Combine(resultDirectory ?? string.Empty, $"I{resultFilename}.g.cs");

                    if (!string.IsNullOrEmpty(resultDirectory) && !Directory.Exists(resultDirectory))
                        Directory.CreateDirectory(resultDirectory);

                    File.WriteAllText(resultPath, result);
                }
            }
        }

        private static void WriteDescriptions(ITemplateModel template, string outputPath)
        {
            if (template != null)
            {
                var result = template.RenderDescriptions();
                if (result != null && template.Id != null)
                {
                    var resultPath = template.Id.Replace('.', Path.DirectorySeparatorChar);
                    resultPath = Path.Combine(outputPath, resultPath);
                    var resultDirectory = Path.GetDirectoryName(resultPath);
                    var resultFilename = Path.GetFileName(resultPath);
                    resultPath = Path.Combine(resultDirectory ?? string.Empty, $"{resultFilename}Descriptions.g.cs");

                    if (!string.IsNullOrEmpty(resultDirectory) && !Directory.Exists(resultDirectory))
                        Directory.CreateDirectory(resultDirectory);

                    File.WriteAllText(resultPath, result);
                }
            }
        }

        // Property names declared on the hand-written `Observation` base class
        // (libraries/MTConnect.NET-Common/Observations/Observation.cs) — kept
        // here because Observation is fully hand-written and never lands in
        // the SysML class graph, so the inheritance walk cannot discover it.
        // Every DataSetResult inherits from EventDataSetObservation ⇒ ... ⇒
        // Observation, so a generated `Name` / `Type` / `Uuid` / etc. property
        // hides the matching base member and needs `new`. Only the names
        // exposed as instance properties on Observation belong here; const /
        // static / readonly fields are not hidden by an instance property and
        // so are omitted.
        private static readonly System.Collections.Generic.HashSet<string> ObservationBasePropertyNames = new(System.StringComparer.Ordinal)
        {
            "DeviceUuid",
            "DataItem",
            "Uuid",
            "DataItemId",
            "Timestamp",
            "Name",
            "InstanceId",
            "Sequence",
            "Category",
            "Type",
            "SubType",
            "CompositionId",
            "Representation",
            "Quality",
            "Deprecated",
            "Extended",
            "IsUnavailable",
            "Values",
        };

        /// <summary>
        /// Marks each <see cref="MTConnectPropertyModel.IsInherited"/> flag on a class /
        /// interface template whose property name matches one declared on an
        /// ancestor. The C# templates use the flag to emit a <c>new</c>
        /// modifier on the redeclared member, suppressing CS0108 ("hides
        /// inherited member; use the new keyword if hiding was intended") on
        /// the generated output.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Two contributors feed the inherited-name set per class:
        /// </para>
        /// <list type="number">
        ///   <item>
        ///     The SysML-declared parent chain walked via
        ///     <see cref="MTConnectClassModel.ParentName"/> through every
        ///     ClassModel the renderer has assembled. This catches the
        ///     overwhelming majority of cases (Asset.SerialNumber ⇒
        ///     CuttingToolAsset, Measurement.Code ⇒ ToolingMeasurement, etc.).
        ///   </item>
        ///   <item>
        ///     Hand-stitched seeds for inheritance links the SysML model does
        ///     not express. Three categories ship today:
        ///     <list type="bullet">
        ///       <item>
        ///         The synthesized <c>Devices.Container</c> ClassModel —
        ///         <c>IContainer</c>'s hand-written partial extends
        ///         <c>IMTConnectEntity</c>, so its <c>Uuid</c> hides
        ///         <c>IMTConnectEntity.Uuid</c>.
        ///       </item>
        ///       <item>
        ///         <c>Devices.Composition</c> — the hand-written
        ///         <c>IComposition.cs</c> partial extends <c>IContainer</c>,
        ///         so <c>Type</c> hides <c>IContainer.Type</c> (declared on
        ///         the hand-written IContainer partial, not in the .g.cs).
        ///       </item>
        ///       <item>
        ///         Every DataSetResult — its rendered template hardcodes the
        ///         base class as <c>EventDataSetObservation</c>, which
        ///         transitively extends the hand-written <c>Observation</c>
        ///         (also not in the SysML graph). Each DataSetResult property
        ///         whose name appears in
        ///         <see cref="ObservationBasePropertyNames"/> needs <c>new</c>.
        ///       </item>
        ///     </list>
        ///   </item>
        /// </list>
        /// <para>
        /// The walk also doubles as the marker for the I-prefix interface
        /// templates — Interface.scriban renders the same Properties list, so
        /// marking once flags both the model and the interface.
        /// </para>
        /// </remarks>
        private static void MarkInheritedProperties(
            List<ITemplateModel> templates,
            IEnumerable<MTConnectClassModel> importClassModels)
        {
            if (templates == null) return;

            // Both ClassModel and DataSetResultModel derive from
            // MTConnectClassModel, and both expose Properties via the base
            // type's List<MTConnectPropertyModel>. Working through the base
            // type covers both template families with a single walk.
            var classTemplates = templates.OfType<MTConnectClassModel>().ToList();
            if (classTemplates.Count == 0) return;

            // Two lookup tables. byId is the authoritative one — multiple
            // SysML packages can share a class name (e.g. Pallet.Measurement
            // and CuttingTools.Measurement both have local name "Measurement"),
            // so a Name-only dictionary discards the second. The walk uses
            // byId where possible, resolving "<sibling namespace>.<Name>"
            // first and falling back to byName only when the sibling lookup
            // misses. byName remains for the rare case where the parent
            // genuinely lives in a non-sibling namespace.
            //
            // Both the export-side ClassModel templates and the import-side
            // raw SysML ClassModel set are folded in. Some SysML classes are
            // never rendered (e.g. Assets.CuttingTools.Measurement, whose
            // .g.cs is hand-maintained / frozen) but their declared
            // properties still hide the child's same-named members at C#
            // compile time, so the walk needs to see them.
            var byId = new Dictionary<string, MTConnectClassModel>(StringComparer.Ordinal);
            var byName = new Dictionary<string, MTConnectClassModel>(StringComparer.Ordinal);
            foreach (var ct in classTemplates)
            {
                if (!string.IsNullOrEmpty(ct.Id)) byId.TryAdd(ct.Id, ct);
                if (!string.IsNullOrEmpty(ct.Name)) byName.TryAdd(ct.Name, ct);
            }
            if (importClassModels != null)
            {
                foreach (var cm in importClassModels)
                {
                    if (cm == null) continue;
                    if (!string.IsNullOrEmpty(cm.Id)) byId.TryAdd(cm.Id, cm);
                    if (!string.IsNullOrEmpty(cm.Name)) byName.TryAdd(cm.Name, cm);
                }
            }


            foreach (var template in classTemplates)
            {
                if (!HasAnyProperties(template)) continue;

                // Walk the ancestor chain collecting every property name. Use
                // a HashSet for O(1) per-property lookup and a visited set to
                // guard against pathological cycles (a malformed XMI could
                // produce one; the resolver tries to terminate them but
                // defensive cycle-detection is cheap here).
                var inheritedNames = new HashSet<string>(StringComparer.Ordinal);
                var visited = new HashSet<string>(StringComparer.Ordinal);
                var currentId = template.Id;
                var parentName = template.ParentName;
                while (!string.IsNullOrEmpty(parentName))
                {
                    var parent = ResolveParent(currentId, parentName, byId, byName);
                    if (parent == null) break;
                    if (!visited.Add(parent.Id ?? parentName)) break;
                    foreach (var name in EnumeratePropertyNames(parent))
                    {
                        inheritedNames.Add(name);
                    }
                    currentId = parent.Id;
                    parentName = parent.ParentName;
                }

                // Names found via the SysML class chain are inherited on
                // both the C# class and the C# interface (the generator
                // emits parallel class / interface trees, so the SysML
                // generalization mirrors into both). Names contributed by
                // hand-stitched overrides may be one-sided — Composition is
                // the canonical example, where the hand-written interface
                // partial extends IContainer but the hand-written class
                // partial does not extend Container. ToolingMeasurement is
                // the symmetric case — IMeasurement.g.cs has its Code
                // declaration commented out, so the interface-side child
                // does NOT hide anything (CS0109 if `new` is emitted) while
                // the class-side still extends Measurement.Code (CS0108 if
                // `new` is missing). Track the three contributors separately.
                var interfaceOnlyNames = new HashSet<string>(StringComparer.Ordinal);
                var classOnlyNames = new HashSet<string>(StringComparer.Ordinal);

                // Hand-stitched seeds for inheritance links the SysML model
                // does not express — see XML doc remarks above.
                switch (template.Id)
                {
                    case "Devices.Container":
                        // IContainer.cs partial: `IContainer : IMTConnectEntity`,
                        // and IMTConnectEntity declares `Uuid`. Interface-side
                        // only — there is no hand-written class-side
                        // Container : IMTConnectEntity (Container is not even
                        // emitted as a class, only as an interface).
                        interfaceOnlyNames.Add("Uuid");
                        break;

                    case "Devices.Composition":
                        // IComposition.cs partial: `IComposition : IContainer`,
                        // and the hand-written IContainer.cs declares `Type`.
                        // Interface-side only — the hand-written
                        // Composition.cs partial does NOT extend Container as
                        // a class base, so the Composition.g.cs class
                        // declaration's `Type` does not hide anything.
                        // (Other shared names — Configuration, Description,
                        // Id, Name, Uuid — are already filtered out via
                        // ExportToInterface=false in the earlier pass, so
                        // they don't appear in Composition.Properties at
                        // template-render time.)
                        interfaceOnlyNames.Add("Type");
                        break;

                    case "Assets.CuttingTools.ToolingMeasurement":
                        // ToolingMeasurement extends `Measurement` (the
                        // CuttingTools abstract Measurement base, NOT
                        // Assets.Pallet.Measurement). The CuttingTools
                        // Measurement.g.cs is hand-maintained / frozen —
                        // not produced by any current renderer flow — so
                        // it never enters the export-side ClassModel
                        // graph the inheritance walk traverses, and a
                        // Name-only lookup of "Measurement" resolves to
                        // Pallet.Measurement (which lacks Code). Class
                        // side only — IMeasurement.g.cs has `Code`
                        // commented out, so the interface child does NOT
                        // hide anything and emitting `new` there would
                        // produce CS0109 instead.
                        classOnlyNames.Add("Code");
                        break;
                }

                // The DataSetResult flow is a parallel template family. Each
                // *Result class extends EventDataSetObservation ⇒ Observation
                // (Observation is hand-written, never in the SysML graph), so
                // any property matching a name on Observation needs `new` on
                // the class. DataSetResult has no rendered interface, so the
                // flag only matters on the class side. Suffix match against
                // the spec-derived Id (".Result") is the selector — same
                // approach the renderer uses to pick the DataSetResult
                // template above (CSharpTemplateRenderer.Render line ~121).
                if (template.Id != null && template.Id.EndsWith("Result", StringComparison.Ordinal))
                {
                    foreach (var name in ObservationBasePropertyNames) inheritedNames.Add(name);
                }

                if (inheritedNames.Count == 0
                    && interfaceOnlyNames.Count == 0
                    && classOnlyNames.Count == 0) continue;

                // Mark on the same list Scriban will iterate. ClassModel uses
                // the shadowed List<PropertyModel>; DataSetResultModel uses
                // the inherited List<MTConnectPropertyModel>. EnumerateProperties
                // picks the populated one per template type.
                foreach (var property in EnumerateProperties(template))
                {
                    if (property == null || string.IsNullOrEmpty(property.Name)) continue;
                    if (inheritedNames.Contains(property.Name))
                    {
                        property.IsInherited = true;
                        property.IsInheritedInInterface = true;
                    }
                    else if (interfaceOnlyNames.Contains(property.Name))
                    {
                        property.IsInheritedInInterface = true;
                    }
                    else if (classOnlyNames.Contains(property.Name))
                    {
                        property.IsInherited = true;
                    }
                }
            }
        }

        /// <summary>
        /// Resolves a parent ClassModel from <paramref name="parentName"/>
        /// (a bare <c>ClassName</c> as stored in
        /// <see cref="MTConnectClassModel.ParentName"/>). Prefers the parent
        /// living in the child's own namespace — multiple SysML packages can
        /// share a class name (e.g. <c>Pallet.Measurement</c> vs
        /// <c>CuttingTools.Measurement</c>), and a Name-only dictionary
        /// discards the second. The sibling-namespace lookup walks up the
        /// child's Id one segment at a time, so a child at
        /// <c>Assets.CuttingTools.ToolingMeasurement</c> resolves
        /// <c>"Measurement"</c> first to <c>Assets.CuttingTools.Measurement</c>
        /// (sibling), then <c>Assets.Measurement</c>, then <c>Measurement</c>,
        /// before falling back to the bare Name lookup. Single-segment Ids
        /// fall straight through to the Name table.
        /// </summary>
        private static MTConnectClassModel ResolveParent(
            string childId,
            string parentName,
            Dictionary<string, MTConnectClassModel> byId,
            Dictionary<string, MTConnectClassModel> byName)
        {
            if (string.IsNullOrEmpty(parentName)) return null;

            if (!string.IsNullOrEmpty(childId))
            {
                var lastDot = childId.LastIndexOf('.');
                while (lastDot > 0)
                {
                    var candidate = childId.Substring(0, lastDot + 1) + parentName;
                    if (byId.TryGetValue(candidate, out var hit)) return hit;
                    lastDot = childId.LastIndexOf('.', lastDot - 1);
                }
            }

            return byName.TryGetValue(parentName, out var fallback) ? fallback : null;
        }

        /// <summary>
        /// Returns true when the template exposes any property to walk —
        /// regardless of whether the property list lives on
        /// <see cref="ClassModel"/>'s shadowed <c>List&lt;PropertyModel&gt;</c>
        /// or the inherited <see cref="MTConnectClassModel.Properties"/>
        /// <c>List&lt;MTConnectPropertyModel&gt;</c> used by
        /// <see cref="DataSetResultModel"/> and any other direct subclass.
        /// </summary>
        private static bool HasAnyProperties(MTConnectClassModel template)
        {
            if (template is ClassModel cm)
                return cm.Properties != null && cm.Properties.Count > 0;
            return template.Properties != null && template.Properties.Count > 0;
        }

        /// <summary>
        /// Yields the property objects that Scriban will iterate when
        /// rendering <paramref name="template"/>. Picks the populated list
        /// per template type — see <see cref="HasAnyProperties"/> for the
        /// shadow rationale.
        /// </summary>
        private static IEnumerable<MTConnectPropertyModel> EnumerateProperties(MTConnectClassModel template)
        {
            if (template is ClassModel cm && cm.Properties != null)
            {
                foreach (var p in cm.Properties) yield return p;
                yield break;
            }
            if (template.Properties != null)
            {
                foreach (var p in template.Properties) yield return p;
            }
        }

        /// <summary>
        /// Yields the property names declared on <paramref name="template"/>,
        /// for use during the ancestor-chain walk that builds the inherited-
        /// name set. Reads through the same shadow-aware accessor as the
        /// child-side marking pass so the parent's list is not missed.
        /// </summary>
        private static IEnumerable<string> EnumeratePropertyNames(MTConnectClassModel template)
        {
            foreach (var p in EnumerateProperties(template))
            {
                if (!string.IsNullOrEmpty(p.Name)) yield return p.Name;
            }
        }


        // Apply the "Asset" suffix to a ClassModel's Id / Name (and optionally
        // ParentName) for cases where the spec collapses the namespace. Guards
        // null Id / Name explicitly — the switch arm in Render guarantees
        // template.Id is the literal spec key, but Name is copied from the
        // imported model and could be null on a malformed XMI; guarding here
        // keeps the suffix from masking a missing Name as the literal "Asset".
        private static void ApplyAssetSuffix(ClassModel template, bool alsoSuffixParent)
        {
            if (template == null) return;
            template.IsPartial = true;
            if (template.Id == null)
                throw new InvalidOperationException("ClassModel has null Id; cannot apply Asset suffix. Asset rename relies on the spec-derived id.");
            template.Id += "Asset";
            if (template.Name == null)
                throw new InvalidOperationException($"ClassModel '{template.Id}' has null Name; cannot apply Asset suffix.");
            template.Name += "Asset";
            if (alsoSuffixParent && template.ParentName != null && template.ParentName != "Asset")
                template.ParentName += "Asset";
        }


        private static string ConvertUnitEnum(string input)
        {           
            var output = input;

            if (output != null)
            {
                output = output.Replace("/", "_PER_");
                output = output.Replace("^2", "_SQUARED");
            }

            return output;
        }

        private static string ConvertMeasurementCodeEnum(string input)
        {
            var output = input;

            if (output != null)
            {
                output = input;
            }

            return output;
        }
    }
}
