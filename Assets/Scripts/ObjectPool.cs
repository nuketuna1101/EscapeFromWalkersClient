using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    private readonly Transform parentTransform;
    private readonly GameObject prefab;
    private readonly int initialCount = 20;

    // 초기화
    public ObjectPool(GameObject prefab, Transform parent = null)
    {
        this.prefab = prefab;
        parentTransform = parent;
        for (int i = 0; i < initialCount; i++)
            AddObjectToPool();
    }

    // 오브젝트 풀에 오브젝트 추가
    private void AddObjectToPool()
    {
        GameObject newObj = Object.Instantiate(prefab, parentTransform);
        newObj.gameObject.SetActive(false);
        pool.Enqueue(newObj);
    }

    // 오브젝트 할당
    public GameObject GetObject()
    {
        if (pool.Count == 0)
            AddObjectToPool();

        GameObject obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    // 오브젝트 회수
    public void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
