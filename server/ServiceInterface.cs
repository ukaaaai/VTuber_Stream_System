using MagicOnion;

namespace server.Definition
{
    public interface ServiceInterface : IService<ServiceInterface>
    {
        UnaryResult<int> SumAsync(int x, int y);
    }
}
