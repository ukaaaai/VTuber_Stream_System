using SFB;
using UnityEngine;

namespace UI
{
    public sealed class SelectModel: MonoBehaviour
    {
        public string SelectFile()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel(
                "select model", 
                "", 
                new []{ new ExtensionFilter("zip", "zip")},
                false);
            return paths.Length == 0 ? string.Empty : paths[0];
        }
    }
}
