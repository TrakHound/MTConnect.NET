namespace MTConnect.SysML.CSharp
{
    /// <summary>
    /// Contract every C# template model implements. Each model owns
    /// the Scriban rendering of one MTConnect SysML concept and emits
    /// three artefacts: the concrete class, the matching interface,
    /// and a descriptions companion class.
    /// </summary>
    public interface ITemplateModel
    {
        /// <summary>
        /// Identifier of the concept being rendered (matches the
        /// SysML element's <c>xmi:id</c>).
        /// </summary>
        string Id { get; }


        /// <summary>
        /// Returns the rendered C# class body.
        /// </summary>
        /// <returns>C# source text.</returns>
        string RenderModel();

        /// <summary>
        /// Returns the rendered C# interface body.
        /// </summary>
        /// <returns>C# source text.</returns>
        string RenderInterface();

        /// <summary>
        /// Returns the rendered C# <c>*Descriptions</c> companion
        /// class body.
        /// </summary>
        /// <returns>C# source text.</returns>
        string RenderDescriptions();
    }
}
