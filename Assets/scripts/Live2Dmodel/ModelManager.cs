using System;
using System.IO;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Rendering;
using UnityEngine;

namespace Live2Dmodel
{
    public class ModelManager : MonoBehaviour
    {
        private string _modelPath;
        private CubismModel _model;

        public void SetPath(string path) => _modelPath = path;
        
        public void LoadModel()
        {
            if(_model != null) Destroy(_model.gameObject);
            _model = null;
            var model3Json = CubismModel3Json.LoadAtPath(_modelPath, LoadAsset);
            
            _model = model3Json.ToModel();
            
            var parent = GameObject.Find("Canvas");
            Transform transform1;
            (transform1 = _model.transform).SetParent(parent.transform);
            transform1.localPosition = Vector3.zero;
            transform1.localScale = Vector3.one * Screen.height / 2.2f;
            
            var controller = _model.gameObject.GetComponent<CubismRenderController>();
            controller.SortingOrder = 1;
            controller.SortingMode = CubismSortingMode.BackToFrontOrder;
        }

        private static object LoadAsset(Type assetType, string absolutePath)
        {
            if (assetType == typeof(byte[]))
            {
                return File.ReadAllBytes(absolutePath);
            }

            if (assetType == typeof(string))
            {
                return File.ReadAllText(absolutePath);
            }

            if (assetType != typeof(Texture2D)) throw new NotSupportedException();
            var texture = new Texture2D(1, 1);
            
            texture.LoadImage(File.ReadAllBytes(absolutePath));
            
            return texture;
        }
    }
}
