using UnityEngine;
using AngryKoala.Pooling;
using AngryKoala.Services;

public class CubeSpawner : MonoBehaviour
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
        if(_spawnTimer >= _spawnDelay)
        {
            _poolService.CubePool.Get(Vector3.up * 4f, Quaternion.identity);
            _spawnTimer = 0f;
        }
        else
        {
            _spawnTimer += Time.deltaTime;
        }
    }
}
