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

        /// <summary>
        /// Names of additional generalizations beyond the primary
        /// <see cref="ParentName"/>. C# allows a single class base, so a
        /// SysML class with multiple generalizations contributes one
        /// generalization to <see cref="ParentName"/> (the class base —
        /// abstract preferred per the heuristic in the constructor) and
        /// emits the rest as marker interfaces on both the class and
        /// interface declarations.
        /// </summary>
        public List<string> AdditionalParentNames { get; set; } = new();

        /// <summary>
        /// xmi:id list paired one-to-one with <see cref="AdditionalParentNames"/>,
        /// used by the dangling-parent resolver to graft any of the
        /// additional generalizations that live in another SysML package.
        /// </summary>
        public List<string> AdditionalParentUmlIds { get; set; } = new();

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

                // SuperClass selection across multiple generalizations.
                //
                // A SysML class may declare more than one <generalization>
                // (multi-inheritance in UML — e.g. v2.7's OriginDataSet
                // generalizes both DataSet and AbstractOrigin). C# allows a
                // single class base, so one generalization becomes
                // ParentName / ParentUmlId and the rest land in the
                // Additional* lists, where the templates render them as
                // marker interfaces.
                //
                // Heuristic for picking the primary base:
                //  1. Prefer an abstract generalization — C# convention is
                //     "abstract classes as primary inheritance, concrete /
                //     marker types as secondary interfaces". The abstract
                //     parent is also the polymorphism contract (e.g.
                //     AbstractOrigin) the consumer code reasons about.
                //  2. On a tie (multiple abstract or multiple concrete),
                //     fall back to xmi:id ascending order — a deterministic
                //     stable ordering so regen is reproducible.
                SelectGeneralizations(xmiDocument, umlClass);

                // Chain `?.` through the FirstOrDefault() result — when Comments is
                // non-null but empty, FirstOrDefault returns null and `.Body` would NRE.
                var description = umlClass.Comments?.FirstOrDefault()?.Body;
                Description = ModelHelper.ProcessDescription(description);

                // Load Properties — guard `o.Name != null` per element. The
                // outer `?.` only protects the collection; an element with null Name
                // would NRE on `o.Name.StartsWith(...)`.
                var umlProperties = umlClass.Properties?.Where(o => o.Name != null
                    && !o.Name.StartsWith("made")
                    && !o.Name.StartsWith("is")
                    && !o.Name.StartsWith("observes"));
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

        /// <summary>
        /// Resolves <see cref="ParentName"/> / <see cref="ParentUmlId"/>
        /// (the primary class base) and the parallel
        /// <see cref="AdditionalParentNames"/> /
        /// <see cref="AdditionalParentUmlIds"/> lists (rendered as marker
        /// interfaces) from <c>umlClass.Generalizations</c>. See the call
        /// site comment for the abstract-first / xmi:id-stable heuristic.
        /// </summary>
        private void SelectGeneralizations(XmiDocument xmiDocument, UmlClass umlClass)
        {
            var generalizations = umlClass.Generalizations;
            if (generalizations == null || generalizations.Length == 0) return;

            // Resolve each generalization's target once. Skip entries whose
            // `general` attribute is missing — defensive against malformed
            // XMI; a generalization without a target is unusable here.
            // Also filter out same-name targets (a sibling class living in
            // a different package that happens to share the parent's name —
            // emitting it as an additional interface would produce a
            // self-referential `IFoo : IFoo`).
            var resolved = generalizations
                .Where(g => !string.IsNullOrEmpty(g.General))
                .Select(g => new
                {
                    Generalization = g,
                    TargetClass = ModelHelper.GetClass(xmiDocument, g.General),
                    TargetName = ModelHelper.GetClassName(xmiDocument, g.General),
                })
                .Where(r => !string.IsNullOrEmpty(r.TargetName) && r.TargetName != Name)
                .ToList();

            if (resolved.Count == 0) return;

            // Stable ordering: abstract first (primary class base), then by
            // generalization xmi:id ascending — deterministic so regen is
            // reproducible regardless of XMI declaration order.
            var ordered = resolved
                .OrderByDescending(r => r.TargetClass != null && r.TargetClass.IsAbstract)
                .ThenBy(r => r.Generalization.Id, StringComparer.Ordinal)
                .ToList();

            var primary = ordered[0];
            ParentUmlId = primary.Generalization.General;
            ParentName = primary.TargetName;

            for (var i = 1; i < ordered.Count; i++)
            {
                AdditionalParentUmlIds.Add(ordered[i].Generalization.General);
                AdditionalParentNames.Add(ordered[i].TargetName);
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

            // Single-pass: each grafted parent has its ParentName / ParentUmlId
            // stripped (see pruning block below), so the dangling chain
            // terminates the moment the parent is grafted. The previous
            // `while (true)` wrapper around the same body added no behavior
            // and silently swallowed pathological cycles if a cap had been
            // present.

            // Build the local-id set once — mutate it as grafts land, so the
            // subsequent existence check is O(1) instead of O(n).
            var localUmlIds = new HashSet<string>(
                classes.Where(c => !string.IsNullOrEmpty(c.UmlId)).Select(c => c.UmlId));

            // Dedupe missing parents via HashSet.Add rather than GroupBy/First —
            // same first-wins semantics with one allocation instead of an
            // intermediate grouping. Walks both the primary
            // (ParentName / ParentUmlId) and additional generalizations
            // (AdditionalParentNames / AdditionalParentUmlIds) so a
            // multi-inheritance class with one local + one cross-package
            // generalization still grafts the foreign parent.
            var seenParents = new HashSet<string>();
            var missing = new List<string>();
            foreach (var c in classes)
            {
                if (!string.IsNullOrEmpty(c.ParentName)
                    && !string.IsNullOrEmpty(c.ParentUmlId)
                    && !localUmlIds.Contains(c.ParentUmlId)
                    && seenParents.Add(c.ParentUmlId))
                {
                    missing.Add(c.ParentUmlId);
                }

                for (var i = 0; i < c.AdditionalParentUmlIds.Count; i++)
                {
                    var umlId = c.AdditionalParentUmlIds[i];
                    if (string.IsNullOrEmpty(umlId)) continue;
                    if (localUmlIds.Contains(umlId)) continue;
                    if (seenParents.Add(umlId)) missing.Add(umlId);
                }
            }

            if (missing.Count == 0) return;

            foreach (var danglingUmlId in missing)
            {
                var parentClass = ModelHelper.GetClass(xmiDocument, danglingUmlId);
                if (parentClass == null) continue;

                // Avoid double-grafting: a different dangling sibling may
                // already have pulled the same UmlClass into the local set.
                if (!localUmlIds.Add(parentClass.Id)) continue;

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
                grafted.AdditionalParentNames = new List<string>();
                grafted.AdditionalParentUmlIds = new List<string>();
                grafted.Properties = new List<MTConnectPropertyModel>();

                classes.Add(grafted);
            }
        }
    }
}
