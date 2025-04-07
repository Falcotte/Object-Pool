using UnityEngine;
using AngryKoala.Pooling;
using AngryKoala.Services;

public class PoolableCube : MonoBehaviour, IPoolable
{
    public PoolKey PoolKey { get; set; }
    
    private IPoolService _poolService;
    
    public void Initialize()
    {
        _poolService = ServiceLocator.Get<IPoolService>();
        
        _poolService.CubePool.Return(this, 10f);
    }

    public void Terminate()
    {

    }
}
