using detection;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private void Awake(){
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Setting.AuthManager.Instance.Login();
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        CameraManager.Init();
    }

    private void OnApplicationQuit()
    {
        Setting.AuthManager.Instance.Logout();
        CameraManager.Release();
    }
}
