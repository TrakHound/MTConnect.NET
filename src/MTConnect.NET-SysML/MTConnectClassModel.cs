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

                    Properties = propertyModels;
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
    }
}
