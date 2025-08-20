using AngryKoala.Pooling;
using AngryKoala.Services;
using UnityEngine;

[System.Serializable]
public class PoolableClass: IPoolable
{
    [SerializeField] private int _value;
    public int Value => _value;
    
    public PoolKey PoolKey { get; set; }
    
    private IPoolService _poolService;
    
    public void Initialize()
    {
        _poolService = ServiceLocator.Get<IPoolService>();
        
        _poolService.ClassPool.Return(this, 0.25f);
    }

    public void Terminate()
    {

    }
}