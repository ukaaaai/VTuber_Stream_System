using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using System;
using System.IO;

public class model_manager : MonoBehaviour
{
    private static List<string> userIds = new List<string>();
    private static Dictionary<string, CubismModel3Json> models = new Dictionary<string,CubismModel3Json>();
    private static Dictionary<string, CubismModel> objects = new Dictionary<string, CubismModel>();

    public static bool modelCheck(string path)
    {
        try
        {
            CubismModel3Json.LoadAtPath(path, BuiltinLoadAssetAtPath);
            return true;
        }
        catch
        {
            return false;
        }
    }

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

    public static void AddModel(string path) {
        try
        {
            models[GameManager.userId] = CubismModel3Json.LoadAtPath(path, BuiltinLoadAssetAtPath);
            if (objects[GameManager.userId])
            {
                try
                {
                    Destroy(objects[GameManager.userId]);
                }
                catch{ }
            }
            objects[GameManager.userId] = models[GameManager.userId].ToModel();
        }
        catch { }
    }

    public static Dictionary<string, CubismModel3Json> getmodels() => models;
    public static Dictionary<string, CubismModel> getObjects() => objects;
}
