using AngryKoala.Services;
using UnityEngine;

namespace AngryKoala.Pooling
{
    public class PoolService : BaseService<IPoolService>, IPoolService
    {
        [SerializeField] private CubePool _cubePool;
        public CubePool CubePool => _cubePool;
    }
}