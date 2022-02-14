using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static string username = "1";

    private GameManager() { }

    private void Awake()
    {
        modelManager.Init();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static void trackingStart()
    {

    }

    public static string getUserName() { return username; }
}
