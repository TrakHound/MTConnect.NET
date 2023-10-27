namespace MTConnect.SysML.CSharp
{
    public interface ITemplateModel
    {
        string Id { get; }


        string RenderModel();

        string RenderInterface();

        string RenderDescriptions();
    }
}
