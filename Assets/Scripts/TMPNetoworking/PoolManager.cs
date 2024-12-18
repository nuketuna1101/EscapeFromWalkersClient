using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField]
    private GameObject prefab;
    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    private readonly Transform parentTransform;
    private readonly int initPoolSize = 20;

    private void Awake()
    {
        base.Awake();
        InitPool();
    }
    public void InitPool()
    {
        for (int i = 0; i < initPoolSize; i++)
            AddObjectToPool();
    }

    // 오브젝트 풀에 오브젝트 추가
    private void AddObjectToPool()
    {
        GameObject newObj = Object.Instantiate(prefab, parentTransform);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        pool.Enqueue(newObj);
    }

    // 오브젝트 할당
    public GameObject GetObject()
    {
        if (pool.Count == 0)
            AddObjectToPool();

        GameObject obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        obj.transform.SetParent(null);

        // 임시로 식별 가능하도록 랜덤 색상
        // 오브젝트 색상 변경
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            // 색을 랜덤으로 변경 (예시)
            objRenderer.material.color = new Color(Random.value, Random.value, Random.value);
        }

        return obj;
    }

    // 오브젝트 회수
    public void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}
