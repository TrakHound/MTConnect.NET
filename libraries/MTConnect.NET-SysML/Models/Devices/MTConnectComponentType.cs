using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// A parsed concrete Component subtype: its emitted class name, the
    /// MTConnect <c>type</c> value, default component name, organizer flag,
    /// and valid version range.
    /// </summary>
    public class MTConnectComponentType : IMTConnectExportModel
    {
        /// <inheritdoc/>
        public string UmlId { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The emitted C# class name (the MTConnect type with a
        /// <c>Component</c> suffix).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The MTConnect <c>type</c> attribute value for this component.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The name of the base component class, or <c>null</c> when the
        /// base is <c>Component</c>.
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// The cleaned description text emitted into the doc comment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The default instance name for this component type.
        /// </summary>
        public string DefaultName { get; set; }

        /// <summary>
        /// True when the component is emitted as <c>abstract</c>.
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// True when the component is an organizer (a container for other
        /// components).
        /// </summary>
        public bool IsOrganizer { get; set; }

        /// <summary>
        /// The MTConnect version this component was deprecated at, or
        /// <c>null</c>.
        /// </summary>
        public Version MaximumVersion { get; set; }

        /// <summary>
        /// The MTConnect version this component was introduced in, or
        /// <c>null</c>.
        /// </summary>
        public Version MinimumVersion { get; set; }


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectComponentType() { }

        /// <summary>
        /// Parses a Component subtype from <paramref name="umlClass"/> under
        /// <paramref name="idPrefix"/>, resolving its default name, version
        /// range, and single base class.
        /// </summary>
        public MTConnectComponentType(XmiDocument xmiDocument, string idPrefix, UmlClass umlClass, bool isOrganizer = false)
        {
            if (umlClass != null)
            {
                UmlId = umlClass.Id;

                var name = $"{umlClass.Name.ToTitleCase()}Component";

                Id = $"{idPrefix}.{name}";
                Name = name;
                Type = umlClass.Name;
                DefaultName = GetName(Type);
                IsAbstract = umlClass.IsAbstract;
                IsOrganizer = isOrganizer;

                MaximumVersion = MTConnectVersion.LookupDeprecated(xmiDocument, umlClass.Id);
                MinimumVersion = MTConnectVersion.LookupNormative(xmiDocument, umlClass.Id);

                // Add SuperClass (ParentType). Components in MTConnect are a
                // single-inheritance hierarchy — the first generalization is
                // the C# base. If the spec ever introduces a second
                // generalization on a component class, this needs to grow
                // an additional-interfaces story like MTConnectClassModel.
                var componentParent = umlClass.Generalizations?.FirstOrDefault();
                if (componentParent != null)
                {
                    ParentName = ModelHelper.GetClassName(xmiDocument, componentParent.General);
                    if (ParentName != null && ParentName != "Component") ParentName += "Component";
                }

                var description = umlClass.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }


        /// <summary>
        /// Parses every Component subtype in <paramref name="umlClasses"/>
        /// under <paramref name="idPrefix"/>.
        /// </summary>
        public static IEnumerable<MTConnectComponentType> Parse(XmiDocument xmiDocument, string idPrefix, IEnumerable<UmlClass> umlClasses, bool isOrganizer = false)
        {
            var subClasses = new List<MTConnectComponentType>();

            if (umlClasses != null)
            {
                foreach (var umlClass in umlClasses)
                {
                    subClasses.Add(new MTConnectComponentType(xmiDocument, idPrefix, umlClass, isOrganizer));
                }
            }

            return subClasses;
        }

        private static string GetName(string type)
        {
            switch (type)
            {
                case "Controller": return "cont";
                case "Environment": return "env";
            }

            return type.ToCamelCase();
        }
    }
}
