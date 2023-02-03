using MTConnect.Assets.Files;

namespace MTConnect.Applications
{
    internal static partial class Examples
    {
        public static FileAsset FileAsset()
        {
            var programFile = new FileAsset();
            programFile.AssetId = "114905-105-30";
            programFile.Timestamp = UnixDateTime.Now;
            programFile.Size = 1234514;
            programFile.FileLocation = new FileLocation(@"\\server\114905-105-30.NC");

            return programFile;
        }
    }
}
