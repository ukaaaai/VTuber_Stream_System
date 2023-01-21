using System.Net;
using System.Net.Sockets;

public class Network
{
  //singleton
  private static Network _instance;
  private readonly UdpClient _udpClient;

  private Network()
  {
    _udpClient = new UdpClient();
    _udpClient.Connect(Setting.NetworkSetting.Instance.ServerEndPoint);
  }

  public static Network Instance
  {
    get { return _instance ??= new Network(); }
  }

  public void Send(byte[] data)
  {
    _udpClient.Send(data, data.Length);
  }
}