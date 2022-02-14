using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tracking_debug : MonoBehaviour
{
    [SerializeField] static RawImage rawImage;
    private tracking_debug() { }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void getTexture(Texture2D texture)
    {
        if (rawImage == null)
        {
            return;
        }
        rawImage.texture = texture;
    }
}
