using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectDataItemSubType : MTConnectSubclassModel
    {
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
