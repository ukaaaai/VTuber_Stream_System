using System.Net;

namespace Setting
{
    public class NetworkSetting
    {
        private static NetworkSetting _instance;
        public IPEndPoint ServerEndPoint { get; private set; }

        private NetworkSetting()
        {
            var serverAddress = IPAddress.Any;
            const int serverPort = 0;
            ServerEndPoint = new IPEndPoint(serverAddress, serverPort);
        }
        
        public void SetServerEndPoint(IPEndPoint serverEndPoint)
        {
            ServerEndPoint = serverEndPoint;
        }
        
        public void SetServerEndPoint(string serverAddress, int serverPort)
        {
            ServerEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
        }

        public static NetworkSetting Instance
        {
            get
            {
                return _instance ??= new NetworkSetting();
            }
        }
    }
}