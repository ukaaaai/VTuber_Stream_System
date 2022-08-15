using UnityEngine;

public class Network
{
  //singleton
  private static Network _instance;
  private Network(){}
  public static Network instance
  {
    get
    {
      if (_instance == null)
      {
        _instance = new Network();
      }
      return _instance;
    }
  }
  //end singleton

  
}
