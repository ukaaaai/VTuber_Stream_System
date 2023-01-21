using detection;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeviceSelect : MonoBehaviour
    {
        [SerializeField] private Dropdown dropdown;

        public void Awake()
        {
            dropdown.options.Clear();
            foreach(var device in WebCamTexture.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device.name));
            }
        }
        public void Start()
        {
            CameraManager.Instance.CameraOpen(dropdown.options[0].text);
            dropdown.value = 0;
        }
        
        public void OnButtonClicked()
        {
            CameraManager.Instance.CameraOpen(dropdown.options[dropdown.value].text);
        }
    }
}
