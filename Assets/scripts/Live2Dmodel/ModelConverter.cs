using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Rendering;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Live2Dmodel
{
    public class ModelConverter: MonoBehaviour
    {
        public void SavePrefab(string modelPath)
        {
            var model = CubismModel3Json.LoadAtPath(modelPath, ModelManager.LoadAsset).ToModel();
            var childrenCount = model.transform.childCount;
            var childrenObjects = new GameObject[childrenCount];
            for (var i = 0; i < childrenCount; i++)
            {
                var child = model.transform.GetChild(i);
                if (child.name is "Drawable")
                {
                    foreach (Transform childTransform in child)
                    {
                        Destroy(childTransform.GetComponent<CubismRenderer>());
                    }
                }
                childrenObjects[i] = model.transform.GetChild(i).gameObject;
            }

            var baseObject = new GameObject();
            foreach (var obj in childrenObjects)
            {
                Instantiate(obj, baseObject.transform);
            }

            PrefabUtility.SaveAsPrefabAsset(baseObject, "Assets/Prefab/model.prefab");
            Destroy(baseObject);
        }
    }
}
