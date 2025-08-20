using AngryKoala.Services;

namespace AngryKoala.Pooling
{
    public interface IPoolService : IService
    {
        public CubePool CubePool { get; }
        
        public ClassPool ClassPool { get; }
    }
}