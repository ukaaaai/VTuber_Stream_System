using detection;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private void Awake(){
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        Application.targetFrameRate = 30;
    }

    // Start is called before the first frame update
    private void Start()
    {
        CameraManager.Init();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        CameraManager.Release();
    }
}
