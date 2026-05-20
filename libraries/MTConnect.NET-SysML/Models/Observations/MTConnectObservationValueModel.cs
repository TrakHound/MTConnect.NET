using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.Models.Observations
{
    /// <summary>
    /// A single parsed observation enumeration value (a controlled-vocabulary
    /// member of an observation type) with its cleaned description.
    /// </summary>
    public class MTConnectObservationValueModel : IMTConnectExportModel
    {
        /// <inheritdoc/>
        public string UmlId { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The value's member name as emitted in the generated C#.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cleaned description text emitted into the doc comment.
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectObservationValueModel() { }

        /// <summary>
        /// Parses an observation value literal under
        /// <paramref name="idPrefix"/>, taking its name and cleaned
        /// description.
        /// </summary>
        public MTConnectObservationValueModel(string idPrefix, UmlEnumerationLiteral enumerationLiteral)
        {
            if (enumerationLiteral != null)
            {
                UmlId = enumerationLiteral.Id;

                var name = enumerationLiteral.Name;

                Id = $"{idPrefix}.{name}";
                Name = name;

                var description = enumerationLiteral.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }
    }
}
