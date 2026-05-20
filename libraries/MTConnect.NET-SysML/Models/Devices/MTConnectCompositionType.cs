using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// A parsed concrete Composition subtype, derived from a member of the
    /// <c>CompositionTypeEnum</c>: its emitted class name, MTConnect
    /// <c>type</c> value, default name, and valid version range.
    /// </summary>
    public class MTConnectCompositionType : IMTConnectExportModel
    {
        /// <inheritdoc/>
        public string UmlId { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The emitted C# class name (the MTConnect type with a
        /// <c>Composition</c> suffix).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The MTConnect <c>type</c> attribute value for this composition.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The cleaned description text emitted into the doc comment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The default instance name for this composition type.
        /// </summary>
        public string DefaultName { get; set; }

        /// <summary>
        /// The MTConnect version this composition was deprecated at, or
        /// <c>null</c>.
        /// </summary>
        public Version MaximumVersion { get; set; }

        /// <summary>
        /// The MTConnect version this composition was introduced in, or
        /// <c>null</c>.
        /// </summary>
        public Version MinimumVersion { get; set; }


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectCompositionType() { }

        /// <summary>
        /// Parses a Composition subtype from an enumeration literal under
        /// <paramref name="idPrefix"/>, resolving its default name and
        /// version range.
        /// </summary>
        public MTConnectCompositionType(XmiDocument xmiDocument, string idPrefix, UmlEnumerationLiteral umlEnumerationLiteral)
        {
            if (umlEnumerationLiteral != null)
            {
                UmlId = umlEnumerationLiteral.Id;

                var name = $"{umlEnumerationLiteral.Name.ToTitleCase()}Composition";

                Id = $"{idPrefix}.{name}";
                Name = name;
                Type = umlEnumerationLiteral.Name;
                DefaultName = name.ToCamelCase();

                MaximumVersion = MTConnectVersion.LookupDeprecated(xmiDocument, umlEnumerationLiteral.Id);
                MinimumVersion = MTConnectVersion.LookupNormative(xmiDocument, umlEnumerationLiteral.Id);

                var description = umlEnumerationLiteral.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }


        /// <summary>
        /// Parses every member of <paramref name="umlEnumeration"/> into a
        /// Composition subtype under <paramref name="idPrefix"/>.
        /// </summary>
        public static IEnumerable<MTConnectCompositionType> Parse(XmiDocument xmiDocument, string idPrefix, UmlEnumeration umlEnumeration)
        {
            var subClasses = new List<MTConnectCompositionType>();

            if (umlEnumeration != null)
            {
                foreach (var item in umlEnumeration.Items)
                {
                    subClasses.Add(new MTConnectCompositionType(xmiDocument, idPrefix, item));
                }
            }

            return subClasses;
        }
    }
}
