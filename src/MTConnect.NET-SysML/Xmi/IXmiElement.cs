namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// Generic reference to a <c>xmi</c> element
    /// </summary>
    public interface IXmiElement
    {
        /// <summary>
        /// <c>xmi:id</c> attribute
        /// </summary>
        string? Id { get; set; }

        /// <summary>
        /// <c>name</c> attribute
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// <c>xmi:type</c> attribute
        /// </summary>
        string? Type { get; set; }
    }
}