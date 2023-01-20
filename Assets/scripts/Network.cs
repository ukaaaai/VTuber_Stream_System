using UnityEngine;

public class Network
{
  //singleton
  private static Network _instance;
  private Network(){}
  public static Network Instance
  {
    get
    {
      return _instance ??= new Network();
    }
  }
  //end singleton

  
}
