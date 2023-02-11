using SFB;

namespace UI
{
    public class SelectModel
    {
        public static string SelectFile()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("select model", "", new [] {new ExtensionFilter("zip", "zip")},false);
            return paths.Length == 0 ? string.Empty : paths[0];
        }
    }
}
