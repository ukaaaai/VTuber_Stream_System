using UnityEngine;
using System;

public class filePicker : MonoBehaviour
{
    public void OnClick()
    {
        try
        {
            string path = FilePick.getInstance().filePick();
            Debug.Log(path);
        }
        catch (Exception)
        {
            Debug.Log("file is not selected or file select is faild");
        }
    }
}
