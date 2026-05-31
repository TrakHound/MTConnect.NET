using MTConnect.SysML.Xmi.UML;
using System;
using System.Linq;

namespace MTConnect.SysML
{
    /// <summary>
    /// Base parsed model for a subtype/enumeration-derived element: carries
    /// its identity, display name, cleaned description, and the version range
    /// over which it is valid.
    /// </summary>
    public class MTConnectSubclassModel : IMTConnectExportModel
    {
        /// <inheritdoc/>
        public string UmlId { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The subtype's name as it appears in the generated C#.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cleaned description text emitted into the doc comment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The MTConnect version this subtype was deprecated at, or
        /// <c>null</c> when it is not deprecated.
        /// </summary>
        public Version MaximumVersion { get; set; }

        /// <summary>
        /// The MTConnect version this subtype was introduced in, or
        /// <c>null</c> when no introduction is recorded.
        /// </summary>
        public Version MinimumVersion { get; set; }


        /// <summary>
        /// Initializes the model from a UML class, taking its name and
        /// cleaned description; a null class leaves the members unset.
        /// </summary>
        public MTConnectSubclassModel(UmlClass umlClass)
        {
            if (umlClass != null)
            {
                Name = umlClass.Name;

                var description = umlClass.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }
    }
}
