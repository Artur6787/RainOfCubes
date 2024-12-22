using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _platform;
    [SerializeField] private Cube _prefabCube;
    [SerializeField] private float _delay = 1f;
    [SerializeField] private int _startSizePool = 5;
    [SerializeField] private int _maxSizePool = 10;

    private float _maxOffsetX;
    private float _maxOffsetY = 8f;
    private float _maxOffsetZ;
    private int _indexOffset = 2;
    private bool _isWork = true;
    private ObjectPool<Cube> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Cube>(
            createFunc: () => CreateCube(),
            actionOnGet: (obj) => ActivateCube(obj),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => DeleteCube(obj),
            collectionCheck: true,
            defaultCapacity: _startSizePool,
            maxSize: _maxSizePool);
    }

    private void Start()
    {
        _maxOffsetX = _platform.localScale.x * _indexOffset;
        _maxOffsetZ = _platform.localScale.z * _indexOffset;

        StartCoroutine(nameof(ActiveSpawn), _delay);
    }

    private void ActivateCube(Cube cube)
    {
        cube.transform.position = GetRandomPosition();
        cube.transform.rotation = Quaternion.identity;
        cube.gameObject.SetActive(true);
    }

    private Cube CreateCube()
    {
        Cube cube = Instantiate(_prefabCube);
        cube.Released += Release;

        return cube;
    }

    private void SpawnCube()
    {
        _pool.Get();
    }

    private void Release(Cube cube)
    {
        _pool.Release(cube);
    }

    private void DeleteCube(Cube cube)
    {
        cube.Released -= Release;
        Destroy(cube);
    }

    private Vector3 GetRandomPosition()
    {
        float positionOffsetX = Random.Range(-_maxOffsetX, _maxOffsetX);
        float positionOffsetZ = Random.Range(-_maxOffsetZ, _maxOffsetZ);

        float positionX = _platform.position.x + positionOffsetX;
        float positionZ = _platform.position.z + positionOffsetZ;

        return new Vector3(positionX, _maxOffsetY, positionZ);
    }

    private IEnumerator ActiveSpawn(float delay)
    {
        while (_isWork)
        {
            SpawnCube();

            yield return new WaitForSeconds(delay);
        }
    }
}
