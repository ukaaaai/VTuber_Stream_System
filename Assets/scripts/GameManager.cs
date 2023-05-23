using Cysharp.Threading.Tasks;
using detection;
using Live2Dmodel;
using UnityEngine;


public sealed class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private ModelManager _modelManager;
    private bool _isPause = true;
    private bool _doDetect = true;

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
        Detection.Init();
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
        _doDetect = !_doDetect;
        if (!_doDetect) return;
        var frame = CameraManager.Instance.GetFrame();
        var detectTask = UniTask.RunOnThreadPool(() =>
        {
            var param = Detection.Detect(frame);
            if (param != null) _modelManager.UpdateParam(param.Value);
        });
        await detectTask;
    }
    private void OnApplicationQuit()
    {
        Setting.AuthManager.Instance.Logout();
        CameraManager.Release();
    }
}
