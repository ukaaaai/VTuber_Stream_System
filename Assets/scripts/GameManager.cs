using detection;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private void Awake(){
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 30;
    }

    // Start is called before the first frame update
    private void Start()
    {
        CameraManager.Init();
    }

    private void OnApplicationQuit()
    {
        CameraManager.Release();
    }
}
