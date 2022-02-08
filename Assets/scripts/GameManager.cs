using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = new GameManager();
    private static faceTracking faceTracking;
    private static string username = "1";

    private GameManager() { }

    private void Awake()
    {
        faceTracking = faceTracking.getInstance();
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
