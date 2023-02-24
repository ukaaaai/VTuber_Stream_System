using System;
using SFB;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public sealed class SelectModel: MonoBehaviour
    {
        [Serializable] private class SelectEvent: UnityEvent<string> {}
        [SerializeField] private SelectEvent onSelect;
        public void SelectFile()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel(
                "select model", 
                "", 
                new []{ new ExtensionFilter("json", "json")},
                false);
            onSelect.Invoke(paths.Length == 0 ? string.Empty : paths[0]);
        }
    }
}
