using MTConnect.SysML.Xmi.UML;
using System;
using System.Linq;

namespace MTConnect.SysML
{
    /// <summary>
    /// A single parsed enumeration literal: its C# member name, the original
    /// XMI literal value, and the cleaned description emitted as its doc
    /// comment.
    /// </summary>
    public class MTConnectEnumValueModel : IMTConnectExportModel
    {
        /// <inheritdoc/>
        public string UmlId { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The enumeration member name as emitted in the generated C# enum.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The original XMI literal value the member maps to.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The cleaned description text emitted into the doc comment.
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectEnumValueModel() { }

        /// <summary>
        /// Parses an enumeration literal under <paramref name="idPrefix"/>,
        /// optionally rewriting its name through
        /// <paramref name="convertFunction"/> while preserving the original
        /// literal as <see cref="Value"/>.
        /// </summary>
        public MTConnectEnumValueModel(string idPrefix, UmlEnumerationLiteral enumerationLiteral, Func<string, string> convertFunction = null)
        {
            if (enumerationLiteral != null)
            {
                UmlId = enumerationLiteral.Id;

                var name = enumerationLiteral.Name;
                if (convertFunction != null)
                {
                    name = convertFunction(name);
                }

                Id = $"{idPrefix}.{name}";
                Name = name;
                Value = enumerationLiteral.Name;

                var description = enumerationLiteral.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }
    }
}
