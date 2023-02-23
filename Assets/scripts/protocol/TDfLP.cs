#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace protocol
{
  // ReSharper disable InconsistentNaming
  public class TDfLp{
    // ReSharper restore InconsistentNaming
    //flags
    public enum SocketFlag{
      Undefined = 0,
      Register = 1,
      ClientTrackingData = 2,
      MemberTrackingData = 3,
      JoinRoom = 4,
      CreateRoom = 5,
      LeaveRoom = 6,
      RemoveRoom = 7,
      Ack = 8,
      Ready = 9,
      ClientNotConnect = 10,
      Reconnect = 11,
      ClientDisconnect = 12,
      TcpSocketInfo = 13,
    }

    private const int Params = 24;
    private const int HeaderSize = 3;

    private byte[]? _sendData;
    private Dictionary<int, byte[]> _receiveData = new();
    private Dictionary<int, List<short[]>> _trackingData = new();
    public Dictionary<int, string> UserNames { get;} = new();
    private UdpClient _client = new();
    private IPAddress _ip = IPAddress.Any;
    private const int Port = 9000;
    private IPEndPoint _remoteEndPoint = new(IPAddress.Any, 0);

    //singleton
    private static TDfLp? _instance;
    private TDfLp(){}
    public static TDfLp Instance{
      get{
        return _instance ??= new TDfLp();
      }
    }

    public async void Send(byte[] data){
      this._sendData = data;
      await _client.SendAsync(_sendData, _sendData.Length, _remoteEndPoint);
    }

    public void StartReceive(){
      for(;;){
        //スレッド生成
        Thread thread = new(async ()=>{
          //受信
          var data = (await _client.ReceiveAsync()).Buffer;
          if (data == null) return;
          //ヘッダー取得
          var id = GetUserID(data);
          var roomID = GetRoomID(data);
          var flags = GetFlag(data);
          _receiveData[id] = data;
          switch ((SocketFlag)(flags[0]))
          {
            //トラッキングデータ取得
            case SocketFlag.ClientTrackingData:
            {
              //トラッキングデータ取得
              var trackingData = GetTrackingData(data);
              _trackingData[id].Add(trackingData);
              //保持データ数の制御
              if(_trackingData[id].Count > 10){
                _trackingData[id].RemoveAt(0);
              }

              break;
            }
            case SocketFlag.JoinRoom:
              UserNames[id] = GetUserName(data);
              break;
            //クライアント切断
            case SocketFlag.ClientDisconnect:
              _trackingData[id].Clear();
              _receiveData.Remove(id);
              break;
            case SocketFlag.Undefined:
              break;
            case SocketFlag.Register:
              break;
            case SocketFlag.MemberTrackingData:
              break;
            case SocketFlag.CreateRoom:
              break;
            case SocketFlag.LeaveRoom:
              break;
            case SocketFlag.RemoveRoom:
              break;
            case SocketFlag.Ack:
              break;
            case SocketFlag.Ready:
              break;
            case SocketFlag.ClientNotConnect:
              break;
            case SocketFlag.Reconnect:
              break;
            case SocketFlag.TcpSocketInfo:
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        });
      }
    }

    private static int[] GetFlag(in byte[] data){
      var flag = data[0];
      var flags = new int[2];
      const byte mask = 0x0F;
      flags[0] = flag & mask;
      flags[1] = (flag >> 4) & mask;
      return flags;
    }

    private static int GetUserID(in byte[] data){
      return data[1];
    }

    private static int GetRoomID(in byte[] data){
      return data[2];
    }

    private static string GetUserName(byte[] data){
      return System.Text.Encoding.UTF32.GetString(data[HeaderSize..]);
    }

    private static short[] GetTrackingData(byte[] data){
      var trackingData = new short[Params];
      for(var i = 0; i < Params; i++){
        var cursor = HeaderSize + i * 2;
        trackingData[i] = BitConverter.ToInt16(data, cursor);
      }
      return trackingData;
    }
  }
}
