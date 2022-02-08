using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using System;
using System.IO;

public class modelManager : MonoBehaviour
{
    private static string[] userIds;
    private static Dictionary<string, CubismModel3Json> models = new Dictionary<string, CubismModel3Json>();
    private static Dictionary<string, CubismModel> objects = new Dictionary<string,CubismModel>();

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            var path = Application.dataPath + "/Live2D/Cubism/Samples/Models/Koharu/Koharu.model3.json";
            models[GameManager.getUserName()] = CubismModel3Json.LoadAtPath(path, BuiltinLoadAssetAtPath);
            objects[GameManager.getUserName()] = models[GameManager.getUserName()].ToModel();
        }
        catch (Exception) { }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void AddModel(string path)
    {
        try
        {
            models[GameManager.getUserName()] = CubismModel3Json.LoadAtPath(path, BuiltinLoadAssetAtPath);
            if (objects[GameManager.getUserName()])
            {
                Destroy(objects[GameManager.getUserName()].gameObject);
            }
            objects[GameManager.getUserName()] = models[GameManager.getUserName()].ToModel();
        }
        catch(Exception) { }
    }

    public static object BuiltinLoadAssetAtPath(Type assetType, string absolutePath)
    {
        if (assetType == typeof(byte[]))
        {
            return File.ReadAllBytes(absolutePath);
        }
        else if (assetType == typeof(string))
        {
            return File.ReadAllText(absolutePath);
        }
        else if (assetType == typeof(Texture2D))
        {
            var texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(absolutePath));

            return texture;
        }

        throw new NotSupportedException();
    }
}
