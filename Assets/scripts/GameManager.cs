using detection;
using Live2Dmodel;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private ModelManager _modelManager;
    private bool _isPause = true;
    private IDetector _detector;

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

        _detector = new DlibDetector();
    }

    private void Start()
    {
        CameraManager.Init();
        _detector.Init();
        _modelManager = FindObjectOfType<ModelManager>();
    }
    
    public void Pause()
    {
        _isPause = true;
        CameraManager.Instance.Stop();
    }
    
    public void Resume()
    {
        _isPause = false;
        CameraManager.Instance.Start();
    }

    private async void Update()
    {
        if (_isPause) return;
        var frame = CameraManager.Instance.GetFrame();
        var param = await _detector.Detect(frame);
        if(param.HasValue) _modelManager.UpdateParam(param.Value);
    }
    private void OnApplicationQuit()
    {
        Setting.AuthManager.Instance.Logout();
        CameraManager.Release();
    }
}
