using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// A parsed DataItem subtype: the trailing segment of the XMI class name
    /// becomes the uppercase-underscore subtype constant, carried with its
    /// introduced/deprecated versions and cleaned description.
    /// </summary>
    public class MTConnectDataItemSubType : MTConnectSubclassModel
    {
        /// <summary>
        /// Parses a DataItem subtype from <paramref name="umlClass"/>, keying
        /// it under <paramref name="idPrefix"/> and resolving its version
        /// metadata and description from <paramref name="xmiDocument"/>.
        /// </summary>
        public MTConnectDataItemSubType(XmiDocument xmiDocument, string idPrefix, UmlClass umlClass) : base(null)
        {
            UmlId = umlClass.Id;

            var regex = new Regex("^.*\\.(.*)$");
            var name = regex.Match(umlClass.Name).Groups[1].Value;

            Id = $"{idPrefix}.{name}";
            Name = name.ToUnderscoreUpper();

            MaximumVersion = MTConnectVersion.LookupDeprecated(xmiDocument, umlClass.Id);
            MinimumVersion = MTConnectVersion.LookupNormative(xmiDocument, umlClass.Id);

            var description = umlClass.Comments?.FirstOrDefault().Body;
            Description = ModelHelper.ProcessDescription(description);
        }
    }
}
