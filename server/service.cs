using MagicOnion;
using MagicOnion.Server;
using server.Definition;

namespace server.Service
{
    public class ServerService : ServiceBase<ServiceInterface>, ServiceInterface
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            return x + y;
        }
    }
}
