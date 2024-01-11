using System.Net;
using Cysharp.Threading.Tasks;

namespace Live2Dmodel
{
    public static class ModelTransporter
    {
        public static async void SendModel(string path)
        {
            var task = UniTask.Create(() =>
            {
                var remoteEp = new IPEndPoint(
                    Setting.NetworkSetting.Instance.ServerEndPoint.Address,
                    Setting.NetworkSetting.Instance.ServerEndPoint.Port);
                Network.Network.SendTcp(remoteEp, path);
                return UniTask.CompletedTask;
            });
            await task;
        }
    }
}
