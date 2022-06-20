using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string userId = "1";
    public static string displayName = "sample";
    public static string modelPath = null;
    public static string model_directory = null;

    public bool modelDetect(string path)
    {
        if (model_manager.modelCheck(path))
        {
            modelPath = path;
            DirectoryInfo directory = new DirectoryInfo(modelPath);
            model_directory = directory.FullName;
            return true;
        }
        else
        {
            modelPath = null;
            model_directory = null;
            return false;
        }
    }
}
