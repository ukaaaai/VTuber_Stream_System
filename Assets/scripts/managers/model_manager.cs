using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using System;
using System.IO;

public class model_manager
{
    private static List<string> userIds = new List<string>();
    private static Dictionary<string, CubismModel3Json> models = new Dictionary<string,CubismModel3Json>();
    private static Dictionary<string, CubismModel> objects = new Dictionary<string, CubismModel>();

    //Singleton
    private model_manager() { }
    public static model_manager _instance = new model_manager();

    public static model_manager getInstance() => _instance;

    public static object BuiltinLoadAssetAtPath(Type assetType, string path)
    {
        if(assetType == typeof(byte[]))
        {
            return File.ReadAllBytes(path);
        }
        else if(assetType == typeof(string))
        {
            return File.ReadAllText(path);
        }
        else if(assetType == typeof(Texture2D))
        {
            var texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(path));

            return texture;
        }

        throw new NotSupportedException();
    }

    public void AddModel(string path) {
        try
        {
            models[GameManager.getUsername()] = CubismModel3Json.LoadAtPath(path);
            if (objects[GameManager.getUsername()])
            {
                try
                {
                    UnityEngine.Object.Destroy(objects[GameManager.getUsername()]);
                }
                catch{ }
            }
            objects[GameManager.getUsername()] = models[GameManager.getUsername()].ToModel();
        }
        catch { }
    }

    public Dictionary<string, CubismModel3Json> getmodels() => models;
    public Dictionary<string, CubismModel> getObjects() => objects;
}
