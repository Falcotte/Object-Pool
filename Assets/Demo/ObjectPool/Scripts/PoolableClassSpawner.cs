using AngryKoala.Pooling;
using AngryKoala.Services;
using UnityEngine;

public class PoolableClassSpawner : MonoBehaviour
{
    [SerializeField] private float _spawnDelay;

    private float _spawnTimer;

    private IPoolService _poolService;
    
    private void Start()
    {
        _poolService = ServiceLocator.Get<IPoolService>();
    }

    private void Update()
    {
        if (_spawnTimer >= _spawnDelay)
        {
            _poolService.ClassPool.Get(PoolKey.PoolableClass);
            
            Debug.Log($"Spawned PoolableClass");
            
            _spawnTimer = 0f;
        }
        else
        {
            _spawnTimer += Time.deltaTime;
        }
    }
}
