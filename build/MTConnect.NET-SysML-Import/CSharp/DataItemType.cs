using MTConnect.NET_SysML_Import.CSharp;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Template model for an MTConnect <c>DataItemType</c>. Carries
    /// the C#-emitter-only properties (namespace, units enum, version
    /// enums, default short name) Scriban needs to render the
    /// <c>*.g.cs</c> data-item type and its descriptions companion.
    /// </summary>
    public class DataItemType : MTConnectDataItemType, ITemplateModel
    {
        /// <summary>C# namespace the generated type belongs to.</summary>
        public string Namespace => NamespaceHelper.GetNamespace(Id);

        /// <summary>XML-formatted description (XML doc-comment shape).</summary>
        public string XmlDescription { get; set; }

        /// <summary>
        /// Short-form name used for camel-case Property output; maps a
        /// handful of well-known types to their canonical short names
        /// (e.g. <c>EMERGENCY_STOP</c> → <c>estop</c>).
        /// </summary>
        public string DefaultName => GetName(Type);

        /// <summary>Fully-qualified Units enum value emitted into the
        /// Scriban template, e.g. <c>Devices.CELSIUS</c>.</summary>
        public string UnitsEnum => Units != null ? $"Devices.{Units}" : null;

        /// <summary>SysML <c>MaximumVersion</c> mapped to a C# enum
        /// value.</summary>
        public string MaximumVersionEnum => MTConnectVersion.GetVersionEnum(MaximumVersion);

        /// <summary>SysML <c>MinimumVersion</c> mapped to a C# enum
        /// value.</summary>
        public string MinimumVersionEnum => MTConnectVersion.GetVersionEnum(MinimumVersion);

        /// <summary>
        /// Result type for Sample / TimeSeries / DataSet / Table /
        /// Event data items, with the SysML <c>Enum</c> suffix
        /// stripped (e.g. <c>EVENT_VALUE</c> → <c>EVENT_VALUE</c>).
        /// </summary>
        public string ResultType { get; set; }


        /// <summary>Parameterless constructor used by the reflection-
        /// based <see cref="Create"/> factory.</summary>
        public DataItemType() { }

        /// <summary>
        /// Constructs a model directly from an XMI document tree.
        /// </summary>
        /// <param name="xmiDocument">Source XMI document.</param>
        /// <param name="category">Data-item category (Sample /
        /// Event / Condition).</param>
        /// <param name="idPrefix">Identifier prefix applied to the
        /// rendered type.</param>
        /// <param name="umlClass">Backing UML class.</param>
        /// <param name="umlEnumerationLiteral">Backing UML enumeration
        /// literal.</param>
        /// <param name="subClasses">Optional sub-classes that derive
        /// from <paramref name="umlClass"/>.</param>
        public DataItemType(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, IEnumerable<UmlClass> subClasses = null)
            : base (xmiDocument, category, idPrefix, umlClass, umlEnumerationLiteral, subClasses) { }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="DataItemType"/> and applies a few
        /// data-item-specific fix-ups: normalises the Units enum
        /// reference, suffixes the Id / Name / ParentName with
        /// <c>DataItem</c>, and lifts the Result type via
        /// <see cref="ModelHelper.RemoveEnumSuffix"/>. Returns
        /// <c>null</c> when the input is null.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static DataItemType Create(MTConnectDataItemType importModel)
        {
            if (importModel != null)
            {
                var type = typeof(DataItemType);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new DataItemType();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        // Require matching PropertyType so SetValue cannot throw
                        // ArgumentException — a future divergence in property
                        // types between the import and export hierarchies would
                        // otherwise blow up at runtime instead of silently
                        // skipping the mismatched property.
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    if (exportModel.Units != null)
                    {
                        exportModel.Units = exportModel.Units.Replace("NativeUnitsEnum", "NativeUnits");
                        exportModel.Units = exportModel.Units.Replace("UnitsEnum", "Units");
                        exportModel.Units = UnitsHelper.Get(exportModel.Units);
                    }

                    if (importModel.Result != null)
                    {
                        exportModel.ResultType = ModelHelper.RemoveEnumSuffix(importModel.Result);
                    }

                    // Guard before `+= "DataItem"` so a null Id/Name does not silently yield the literal "DataItem".
                    if (exportModel.Id == null)
                        throw new InvalidOperationException("DataItemType has null Id, cannot append 'DataItem' suffix.");
                    if (exportModel.Name == null)
                        throw new InvalidOperationException($"DataItemType '{exportModel.Id}' has null Name, cannot append 'DataItem' suffix.");
                    exportModel.Id += "DataItem";
                    exportModel.Name += "DataItem";
                    exportModel.Description = DescriptionHelper.GetTextDescription(importModel.Description);
                    exportModel.XmlDescription = DescriptionHelper.GetXmlDescription(importModel.Description);
                    if (exportModel.ParentName != null && exportModel.ParentName != "DataItem") exportModel.ParentName += "DataItem";

                    return exportModel;
                }
            }

            return null;
        }


        /// <inheritdoc />
        public virtual string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Devices.DataItemType.scriban");
            return template.Render(this);
        }

        /// <inheritdoc />
        public string RenderInterface() => null;

        /// <inheritdoc />
        public string RenderDescriptions() => null;


		private static string GetName(string type)
		{
			switch (type)
			{
				case "EMERGENCY_STOP": return "estop";
				case "CONTROLLER_MODE": return "mode";
				case "EXECUTION": return "exec";
				case "LOAD": return "load";
				case "POSITION": return "pos";
				case "TEMPERATURE": return "temp";
			}

			return type.ToCamelCase();
		}
	}
}
