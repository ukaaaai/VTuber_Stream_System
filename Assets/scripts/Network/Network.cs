using System.IO;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Network
{
  public static class Network
  {
    private static readonly UdpClient UDPClient;
    private static readonly TcpClient TcpClient;

    static Network()
    {
      UDPClient = new UdpClient();
      TcpClient = new TcpClient();
      UDPClient.Connect(Setting.NetworkSetting.Instance.ServerEndPoint);
    }

    public static void SendUdp(in byte[] data)
    {
      UDPClient.Send(data, data.Length);
    }

    public static void SendTcp(in IPEndPoint remoteEp, in byte[] data)
    {
      TcpClient.Connect(remoteEp);
      var stream = TcpClient.GetStream();
      stream.Write(data, 0, data.Length);
      stream.Close();
      TcpClient.Close();
    }

    public static async void SendTcp(IPEndPoint remoteEp, string dataPath)
    {
      var fileStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
      TcpClient.Connect(remoteEp);
      var stream = TcpClient.GetStream();
      await fileStream.CopyToAsync(stream);
      stream.Close();
      TcpClient.Close();
      Debug.Log("File sent");
    }

    public static async UniTask<byte[]> ReceiveTcp()
    {
      var stream = TcpClient.GetStream();
      var buffer = new byte[1024];
      var memoryStream = new MemoryStream();
      while (true)
      {
        var readSize = await stream.ReadAsync(buffer, 0, buffer.Length);
        if (readSize == 0) break;
        memoryStream.Write(buffer, 0, readSize);
      }

      memoryStream.Close();
      stream.Close();
      TcpClient.Close();
      Debug.Log("File received");
      return memoryStream.ToArray();
    }
  }
}