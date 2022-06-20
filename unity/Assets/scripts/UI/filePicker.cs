using UnityEngine;
using System;
using UnityEngine.Events;

public class filePicker : MonoBehaviour
{
    [Serializable] class myEvent : UnityEvent<string> { }

    [SerializeField] myEvent getPath = new myEvent();
    
    public void OnClick()
    {
        try
        {
            string path = FilePick.getInstance().filePick();
            Debug.Log(path);
            getPath.Invoke(path);
        }
        catch (Exception)
        {
            Debug.Log("file is not selected or file select is faild");
        }
    }
}
