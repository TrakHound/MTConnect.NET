using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML
{
    public class MTConnectClassModel : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public bool IsAbstract { get; set; }

        public string Name { get; set; }

        public string ParentName { get; set; }

        /// <summary>
        /// xmi:id of the generalization target. Captured at parse time so the
        /// dangling-parent resolver (see <see cref="ResolveDanglingParents"/>)
        /// can look up the parent UmlClass globally even when its name is
        /// ambiguous (multiple UML classes can share a name across packages).
        /// </summary>
        public string ParentUmlId { get; set; }

        public string Description { get; set; }

        public List<MTConnectPropertyModel> Properties { get; set; } = new();

        public Version MaximumVersion { get; set; }

        public Version MinimumVersion { get; set; }


        public MTConnectClassModel() { }

        public MTConnectClassModel(XmiDocument xmiDocument, string id, UmlClass umlClass)
        {
            if (umlClass != null)
            {
                UmlId = umlClass.Id;

                Id = id;
                Name = umlClass.Name;
                IsAbstract = umlClass.IsAbstract;

                // Add SuperClass (ParentType)
                if (umlClass.Generalization != null)
                {
                    ParentUmlId = umlClass.Generalization.General;
                    ParentName = ModelHelper.GetClassName(xmiDocument, umlClass.Generalization.General);
                }

                var description = umlClass.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);

                // Load Properties
                var umlProperties = umlClass.Properties?.Where(o => !o.Name.StartsWith("made") && !o.Name.StartsWith("is") && !o.Name.StartsWith("observes"));
                if (umlProperties != null)
                {
                    var propertyModels = new List<MTConnectPropertyModel>();

                    foreach (var umlProperty in umlProperties)
                    {
                        propertyModels.Add(new MTConnectPropertyModel(xmiDocument, id, umlProperty));
                    }

                    Properties = propertyModels.OrderBy(o => o.Name).ToList();
                }
            }
        }

        public void AddProperties(IEnumerable<MTConnectPropertyModel> properties)
        {
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    if (!Properties.Any(o => o.Name == property.Name))
                    {
                        Properties.Add(property);
                    }
                }
            }
        }

        public static IEnumerable<MTConnectClassModel> Parse(XmiDocument xmiDocument, string idPrefix, IEnumerable<UmlClass> umlClasses)
        {
            var models = new List<MTConnectClassModel>();

            if (umlClasses != null)
            {
                foreach (var umlClass in umlClasses)
                {
                    var id = $"{idPrefix}.{umlClass.Name.ToTitleCase()}";

                    if (!ModelHelper.IsValueClass(umlClass))
                    {
                        models.Add(new MTConnectClassModel(xmiDocument, id, umlClass));
                    }
                }
            }

            return models;
        }

        /// <summary>
        /// Resolves dangling generalization references — classes whose
        /// <see cref="ParentName"/> targets a UML class that isn't part of the
        /// supplied <paramref name="classes"/> set. For each missing parent,
        /// the resolver looks up the class in the global XMI by its xmi:id
        /// (the authoritative reference, not the name — multiple UML classes
        /// can share a name across packages) and grafts a freshly-parsed
        /// model instance into <paramref name="classes"/> under the supplied
        /// <paramref name="idPrefix"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each per-package parser in <c>MTConnect.SysML.Models.*</c> walks a
        /// fixed sub-tree of the XMI. When a class in one sub-tree extends a
        /// class living in another (e.g. <c>Devices.Configurations.AxisDataSet</c>
        /// ⇒ <c>Observation.Representations.DataSet</c> in v2.7+), the parent
        /// is invisible to the child's parser. Without this fix-up the
        /// generator emits the child referencing a non-existent C# type and
        /// the build fails with <c>CS0246: type 'DataSet' could not be found</c>.
        /// </para>
        /// <para>
        /// The resolver is version-agnostic — it fires only when there's a
        /// dangling parent name, so older XMIs (no cross-package parents) are
        /// no-ops. The grafted parent has its own
        /// <see cref="ParentName"/> + <see cref="ParentUmlId"/> stripped
        /// (see pruning block in the implementation), so the dangling chain
        /// terminates at the graft and a single pass converges.
        /// </para>
        /// </remarks>
        public static void ResolveDanglingParents(XmiDocument xmiDocument, List<MTConnectClassModel> classes, string idPrefix)
        {
            if (xmiDocument == null || classes == null || classes.Count == 0) return;

            // Single-pass is sufficient because each grafted parent has its
            // ParentName / ParentUmlId stripped (see pruning block below) — the
            // dangling chain terminates the moment the parent is grafted, so
            // we never need to iterate. The previous maxIterations cap was
            // dead defence-in-depth and silently swallowed pathological cycles
            // if the cap was ever hit; a single pass either converges or
            // there's nothing more to do.
            while (true)
            {
                // Match dangling parents by xmi:id (the authoritative reference)
                // rather than Name — multiple UML classes can share a name across
                // packages, and Name-matching produced false-positive "already
                // local" hits that prevented legitimate grafts. The docstring's
                // invariant becomes the implementation here.
                var localUmlIds = new HashSet<string>(
                    classes.Where(c => !string.IsNullOrEmpty(c.UmlId)).Select(c => c.UmlId));

                var missing = classes
                    .Where(c => !string.IsNullOrEmpty(c.ParentName)
                                && !string.IsNullOrEmpty(c.ParentUmlId)
                                && !localUmlIds.Contains(c.ParentUmlId))
                    .GroupBy(c => c.ParentUmlId)
                    .Select(g => g.First())
                    .ToList();

                if (missing.Count == 0) return;

                var graftedThisPass = 0;
                foreach (var dangling in missing)
                {
                    var parentClass = ModelHelper.GetClass(xmiDocument, dangling.ParentUmlId);
                    if (parentClass == null) continue;

                    // Avoid double-grafting: a different dangling sibling may
                    // already have pulled the same UmlClass into the local set.
                    if (classes.Any(c => c.UmlId == parentClass.Id)) continue;

                    var graftedId = $"{idPrefix}.{parentClass.Name.ToTitleCase()}";
                    var grafted = new MTConnectClassModel(xmiDocument, graftedId, parentClass);

                    // Pruning: a class living in another SysML package frequently brings along its own
                    // generalization chain (e.g. DataSet ⇒ Representation ⇒ Observation) and properties whose
                    // declared types live in yet more foreign packages (e.g. DataSet.Result : Entry). Grafting
                    // the full transitive closure across namespace boundaries is rarely what we want — the
                    // child sub-classes that triggered the graft (e.g. AxisDataSet, OriginDataSet) declare
                    // their own concrete fields and only need a structurally-minimal C# base to extend.
                    //
                    // So we strip:
                    //  - ParentName + ParentUmlId — the grafted class becomes a top-level base in the local
                    //    namespace, terminating the recursive search rather than chasing it across packages.
                    //  - Properties — their datatypes may reference yet more classes outside the local set,
                    //    causing CS0246 cascades. The original child sub-classes already define every concrete
                    //    field they need; the grafted base contributes structure (`: DataSet`, `: IDataSet`)
                    //    rather than fields.
                    //
                    // If a future XMI introduces a cross-package base that genuinely needs to carry fields
                    // (and those fields' datatypes are resolvable in the local namespace), revisit this
                    // pruning — for now it is the safe minimum.
                    grafted.ParentName = null;
                    grafted.ParentUmlId = null;
                    grafted.Properties = new List<MTConnectPropertyModel>();

                    classes.Add(grafted);
                    graftedThisPass++;
                }

                if (graftedThisPass == 0) return;
            }
        }
    }
}
