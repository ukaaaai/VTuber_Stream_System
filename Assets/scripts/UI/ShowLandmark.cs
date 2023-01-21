using Unity.Mathematics;
using UnityEngine;

namespace UI
{
    public class ShowLandmark : MonoBehaviour
    {
        [SerializeField] private GameObject landmark;
        private GameObject _landmark;
        private const int LandmarkNum = 68;

        // Start is called before the first frame update
        void Start()
        {
            _landmark = Instantiate(landmark, new Vector3(0, 0, -10), Quaternion.identity);
            var canvas = GameObject.Find("Canvas");
            _landmark.transform.SetParent(canvas.transform, false);
            _landmark.transform.SetAsLastSibling();
        }

        public void ShowLandmarks(int x, int y)
        {
            Debug.Log($"x: {x}, y: {y}");
            _landmark.transform.position = new Vector3(x, y, 0);
            _landmark.SetActive(true);
        }
    }
}
