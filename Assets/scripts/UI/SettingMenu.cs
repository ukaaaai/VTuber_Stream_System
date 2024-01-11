using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject menuCanvas;
        [SerializeField]
        private Button menuOpenButton;
        [SerializeField]
        private Button menuCloseButton;
        
        [SerializeField]
        private InputField serverAddressInputField;
        [SerializeField]
        private InputField serverPortInputField;
        [SerializeField]
        private Button applyButton;
        [SerializeField]
        private Button connectButton;
        
        private void Start()
        {
            menuCanvas.SetActive(true);
            menuOpenButton.onClick.AddListener(OpenMenu);
            menuCloseButton.onClick.AddListener(CloseMenu);
            applyButton.onClick.AddListener(ApplySetting);
            connectButton.onClick.AddListener(Connect);
            menuCanvas.SetActive(false);
        }
        
        private void OpenMenu()
        {
            menuCanvas.SetActive(true);
        }
        
        private void CloseMenu()
        {
            menuCanvas.SetActive(false);
        }
        
        private void ApplySetting()
        {
            var serverAddress = serverAddressInputField.text;
            var serverPort = int.Parse(serverPortInputField.text);

            if (serverAddress is "localhost") serverAddress = "127.0.0.1";
            
            try
            {
                IPAddress.Parse(serverAddress);
            }
            catch
            {
                Debug.Log("invalid server address");
                return;
            }
            if(serverPort is < 0 or > 65535)
            {
                Debug.Log("invalid server port");
                return;
            }
            Setting.NetworkSetting.Instance.SetServerEndPoint(serverAddress, serverPort);
        }
        
        private void Connect()
        {
            // TODO: connect to server
        }
    }
}
