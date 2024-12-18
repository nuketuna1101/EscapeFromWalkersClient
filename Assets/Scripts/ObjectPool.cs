using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly Queue<T> pool = new Queue<T>();
    private readonly Transform parentTransform;
    private readonly T prefab;
    private readonly int initialCount = 20;

    // 초기화
    public ObjectPool(T prefab, Transform parent = null)
    {
        this.prefab = prefab;
        parentTransform = parent;
        for (int i = 0; i < initialCount; i++)
            AddObjectToPool();
    }

    // 오브젝트 풀에 오브젝트 추가
    private void AddObjectToPool()
    {
        T newObj = Object.Instantiate(prefab, parentTransform);
        newObj.gameObject.SetActive(false);
        pool.Enqueue(newObj);
    }

    // 오브젝트 할당
    public T GetObject()
    {
        if (pool.Count == 0)
            AddObjectToPool();

        T obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    // 오브젝트 회수
    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
