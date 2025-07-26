using UnityEngine;
using AngryKoala.Pooling;
using AngryKoala.Services;

public class PoolableCube : MonoBehaviour, IPoolable
{
    [SerializeField] private Rigidbody _rigidbody;
    
    public PoolKey PoolKey { get; set; }
    
    private IPoolService _poolService;
    
    public void Initialize()
    {
        _poolService = ServiceLocator.Get<IPoolService>();
        
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        
        _poolService.CubePool.Return(this, 10f);
    }

    public void Terminate()
    {

    }
}