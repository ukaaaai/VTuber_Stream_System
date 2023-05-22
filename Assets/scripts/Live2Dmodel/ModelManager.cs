using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly int[] _paramIDs = new int[13];
        private Dictionary<int, CubismModel> _remoteModels;
        private Dictionary<int, int[]> _remoteModelParamIDs;
        public static float ParamBreath;
        private float _deltaBreath = 0.1f;
        private detection.Param _userParam;

        private void Update()
        {
            if(ParamBreath is > 1.0f or < 0.0f) _deltaBreath *= -1;
            ParamBreath += _deltaBreath;
        }

        private void LateUpdate()
        {
            if(_model is null) return;
            _model.Parameters[_paramIDs[0]].Value = _userParam.ParamAngleX;
            _model.Parameters[_paramIDs[1]].Value = _userParam.ParamAngleY;
            _model.Parameters[_paramIDs[2]].Value = _userParam.ParamAngleZ;
            _model.Parameters[_paramIDs[3]].Value = _userParam.ParamEyeLOpen;
            _model.Parameters[_paramIDs[4]].Value = _userParam.ParamEyeROpen;
            _model.Parameters[_paramIDs[5]].Value = _userParam.ParamEyeBallX;
            _model.Parameters[_paramIDs[6]].Value = _userParam.ParamEyeBallY;
            _model.Parameters[_paramIDs[7]].Value = _userParam.ParamBrowLY;
            _model.Parameters[_paramIDs[8]].Value = _userParam.ParamBrowRY;
            _model.Parameters[_paramIDs[9]].Value = _userParam.ParamMouthForm;
            _model.Parameters[_paramIDs[10]].Value = _userParam.ParamMouthOpenY;
            _model.Parameters[_paramIDs[11]].Value = _userParam.ParamCheek;
            _model.Parameters[_paramIDs[12]].Value = ParamBreath;
        }

        public void UpdateParam(detection.Param param)
        {
            _userParam = param;
        }

        public void SetPath(string path) => _modelPath = path;
        
        public void LoadModel()
        {
            if(_model != null) Destroy(_model.gameObject);
            _model = CubismModel3Json.LoadAtPath(_modelPath, LoadAsset).ToModel();
            
            var parent = GameObject.Find("Canvas");
            Transform transform1;
            (transform1 = _model.transform).SetParent(parent.transform);
            transform1.localPosition = Vector3.zero;
            transform1.localScale = Vector3.one * Screen.height / 2.2f;
            
            var controller = _model.gameObject.GetComponent<CubismRenderController>();
            controller.SortingOrder = 1;
            controller.SortingMode = CubismSortingMode.BackToFrontOrder;
            
            var paramList = _model.Parameters;
            var paramStr = paramList.Select(p => p.Id).ToArray();
            try
            {
                _paramIDs[0] = Array.IndexOf(paramStr, "ParamAngleX");
                _paramIDs[1] = Array.IndexOf(paramStr, "ParamAngleY");
                _paramIDs[2] = Array.IndexOf(paramStr, "ParamAngleZ");
                _paramIDs[3] = Array.IndexOf(paramStr, "ParamEyeLOpen");
                _paramIDs[4] = Array.IndexOf(paramStr, "ParamEyeROpen");
                _paramIDs[5] = Array.IndexOf(paramStr, "ParamEyeBallX");
                _paramIDs[6] = Array.IndexOf(paramStr, "ParamEyeBallY");
                _paramIDs[7] = Array.IndexOf(paramStr, "ParamBrowLY");
                _paramIDs[8] = Array.IndexOf(paramStr, "ParamBrowRY");
                _paramIDs[9] = Array.IndexOf(paramStr, "ParamMouthForm");
                _paramIDs[10] = Array.IndexOf(paramStr, "ParamMouthOpenY");
                _paramIDs[11] = Array.IndexOf(paramStr, "ParamCheek");
                _paramIDs[12] = Array.IndexOf(paramStr, "ParamBreath");
            }
            catch
            {
                _model = null;
            }
        }

        private static object LoadAsset(Type assetType, string absolutePath)
        {
            if (assetType == typeof(byte[])) return File.ReadAllBytes(absolutePath);
            if (assetType == typeof(string)) return File.ReadAllText(absolutePath);
            if (assetType != typeof(Texture2D)) throw new NotSupportedException();
            
            var texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(absolutePath));
            
            return texture;
        }
    }
}
