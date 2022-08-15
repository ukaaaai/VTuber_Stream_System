#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TDfLP{
  //flags
  public enum socket_flag{
    undefined = 0,
    register = 1,
    client_tracking_data = 2,
    member_tracking_data = 3,
    join_room = 4,
    create_room = 5,
    leave_room = 6,
    remove_room = 7,
    ack = 8,
    ready = 9,
    client_not_connect = 10,
    reconnect = 11,
    client_disconnect = 12,
    TCP_socket_info = 13,
  }

  const int Params = 0;
  const int HeaderSize = 3;

  private byte[]? sendData;
  private Dictionary<int, byte[]> receiveData = new Dictionary<int, byte[]>();
  private Dictionary<int, List<short[]>> trackingData = new Dictionary<int, List<short[]>>();
  public Dictionary<int, string> userNames { get; private set; } = new Dictionary<int, string>();
  private UdpClient client = new UdpClient();
  private IPAddress ip = IPAddress.Any;
  private int port = 9000;
  private IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

  //singleton
  private static TDfLP _instance = new TDfLP();
  private TDfLP(){}
  public static TDfLP instance{
    get{
      if (_instance == null){
        _instance = new TDfLP();
      }
      return _instance;
    }
  }

  public async void Send(byte[] data){
    this.sendData = data;
    await client.SendAsync(sendData, sendData.Length, remoteEndPoint);
  }

  public void startReceive(){
    for(;;){
      //スレッド生成
      Thread thread = new Thread(new ThreadStart(async ()=>{
        //受信
        var data = (await client.ReceiveAsync()).Buffer;
        if(data != null){
          //ヘッダー取得
          int id = getUserID(data);
          int roomID = getRoomID(data);
          int[] flags = getFlag(data);
          this.receiveData[id] = data;
          //トラッキングデータ取得
          if((socket_flag)(flags[0]) == socket_flag.client_tracking_data){
            //トラッキングデータ取得
            short[] trackingData = getTrackingData(data);
            this.trackingData[id].Add(trackingData);
            //保持データ数の制御
            if(this.trackingData[id].Count > 10){
              this.trackingData[id].RemoveAt(0);
            }
          }
          else if((socket_flag)(flags[0]) == socket_flag.join_room){
            userNames[id] = getUserName(data);
          }
          //クライアント切断
          else if((socket_flag)(flags[0]) == socket_flag.client_disconnect){
            this.trackingData[id].Clear();
            this.receiveData.Remove(id);
          }
        }
      }));
    }
  }

  public int[] getFlag(byte[] data){
    byte flag;
    flag = data[0];
    int[] flags = new int[2];
    byte mask = 0x0F;
    flags[0] = flag & mask;
    flags[1] = (flag >> 4) & mask;
    return flags;
  }

  public int getUserID(byte[] data){
    return data[1];
  }

  public int getRoomID(byte[] data){
    return data[2];
  }

  public string getUserName(byte[] data){
    return System.Text.Encoding.UTF32.GetString(data[HeaderSize..]);
  }

  public short[] getTrackingData(byte[] data){
    short[] trackingData = new short[Params];
    for(int i = 0; i < Params; i++){
      int cursor = HeaderSize + i * 2;
      trackingData[i] = BitConverter.ToInt16(data, cursor);
    }
    return trackingData;
  }
}
