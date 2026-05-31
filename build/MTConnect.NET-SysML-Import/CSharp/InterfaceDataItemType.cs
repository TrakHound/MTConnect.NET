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
    /// Template model for an MTConnect interface-tier data-item type.
    /// Derives from <see cref="DataItemType"/> and renders against a
    /// dedicated Scriban template so interface-level data items can
    /// carry slightly different shape than core data items.
    /// </summary>
    public class InterfaceDataItemType : DataItemType
    {
        /// <summary>Parameterless constructor used by the reflection-
        /// based <see cref="Create"/> factory.</summary>
        public InterfaceDataItemType() { }

        /// <summary>
        /// Constructs a model directly from an XMI document tree by
        /// delegating to the base constructor.
        /// </summary>
        /// <param name="xmiDocument">Source XMI document.</param>
        /// <param name="category">Data-item category.</param>
        /// <param name="idPrefix">Identifier prefix.</param>
        /// <param name="umlClass">Backing UML class.</param>
        /// <param name="umlEnumerationLiteral">Backing enumeration
        /// literal.</param>
        /// <param name="subClasses">Optional sub-classes.</param>
        public InterfaceDataItemType(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, IEnumerable<UmlClass> subClasses = null)
            : base (xmiDocument, category, idPrefix, umlClass, umlEnumerationLiteral, subClasses) { }


        /// <summary>
        /// Copies every matching property off <paramref name="importModel"/>
        /// into a fresh <see cref="InterfaceDataItemType"/> and applies
        /// the same Id / Name / ParentName <c>DataItem</c>-suffix fix-up
        /// as the parent factory. Returns <c>null</c> when the input is
        /// null.
        /// </summary>
        /// <param name="importModel">Generic SysML-import model.</param>
        /// <returns>Emitter-aware model, or <c>null</c>.</returns>
        public static InterfaceDataItemType Create(MTConnectInterfaceDataItemType importModel)
        {
            if (importModel != null)
            {
                var type = typeof(InterfaceDataItemType);

                var importProperties = importModel.GetType().GetProperties();
                var exportProperties = type.GetProperties();

                if (importProperties != null && exportProperties != null)
                {
                    var exportModel = new InterfaceDataItemType();

                    foreach (var importProperty in importProperties)
                    {
                        var propertyValue = importProperty.GetValue(importModel);

                        var exportProperty = exportProperties.FirstOrDefault(o => o.Name == importProperty.Name);
                        // Require matching PropertyType so SetValue cannot throw
                        // ArgumentException when a property of the same name has
                        // a different declared type on the export model.
                        if (exportProperty != null && exportProperty.PropertyType == importProperty.PropertyType)
                        {
                            exportProperty.SetValue(exportModel, propertyValue);
                        }
                    }

                    // Guard before `+= "DataItem"` so a null Id/Name does not silently yield the literal "DataItem".
                    if (exportModel.Id == null)
                        throw new InvalidOperationException("InterfaceDataItemType has null Id, cannot append 'DataItem' suffix.");
                    if (exportModel.Name == null)
                        throw new InvalidOperationException($"InterfaceDataItemType '{exportModel.Id}' has null Name, cannot append 'DataItem' suffix.");
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
        public override string RenderModel()
        {
            var template = TemplateLoader.LoadOrThrow("CSharp", "Templates", "Interfaces.InterfaceDataItemType.scriban");
            return template.Render(this);
        }

        /// <inheritdoc cref="ITemplateModel.RenderInterface" />
        public new string RenderInterface() => null;

        /// <inheritdoc cref="ITemplateModel.RenderDescriptions" />
        public new string RenderDescriptions() => null;
    }
}
