using UnityEngine;
using UnityEngine.Pool;

public class BestObjectPool<T> where T : MonoBehaviour
{
    private ObjectPool<T> _objectPool;

    public BestObjectPool(T prefab, int capacity = 10, int maxCapacity = 500)
    {
        _objectPool = new ObjectPool<T>
        (
            createFunc: () =>
            {
                var newObject = Object.Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                return newObject;
            },
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: Object.Destroy,
            defaultCapacity: capacity,
            maxSize: maxCapacity,
            collectionCheck: false
        );
        
        var poolObjects = new T[capacity];
        for (var i = 0; i < capacity; i++)
        {
            poolObjects[i] = _objectPool.Get();
        }
        for (var i = 0; i < capacity; i++)
        {
            _objectPool.Release(poolObjects[i]);
        }
    }

    public T Get() => _objectPool.Get();
    
    public void Release(T obj) => _objectPool.Release(obj);
}