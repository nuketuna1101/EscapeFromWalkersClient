using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    // 다양한 프리팹을 위한 풀을 저장할 Dictionary
    private readonly Dictionary<string, ObjectPool> poolDictionary = new Dictionary<string, ObjectPool>();
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject blockPrefab;
    private const int initPoolSize = 10;
    // 예시: 게임 시작 시 풀 초기화
    void Start()
    {
        InitPool(playerPrefab, initPoolSize); 
        InitPool(blockPrefab, initPoolSize); 
    }



    // 초기화된 풀에 접근할 수 있도록 함
    public GameObject GetObject(string prefabName)
    {
        if (poolDictionary.ContainsKey(prefabName))
        {
            return poolDictionary[prefabName].GetObject();
        }
        else
        {
            Debug.LogError($"Pool for {prefabName} not found!");
            return null;
        }
    }

    // 오브젝트 풀을 반환
    public void ReturnObject(string prefabName, GameObject obj)
    {
        if (poolDictionary.ContainsKey(prefabName))
        {
            poolDictionary[prefabName].ReturnObject(obj);
        }
        else
        {
            Debug.LogError($"Pool for {prefabName} not found!");
        }
    }

    // 풀을 초기화하는 메서드
    public void InitPool(GameObject prefab, int poolSize)
    {
        string prefabName = prefab.name;
        Debug.Log("prefabName: " + prefabName);
        // 이미 풀에 등록된 프리팹이라면 초기화하지 않음
        if (!poolDictionary.ContainsKey(prefabName))
        {
            ObjectPool newPool = new ObjectPool(prefab, poolSize);
            poolDictionary.Add(prefabName, newPool);
        }
        else
        {
            Debug.LogWarning($"Pool for {prefabName} already initialized.");
        }
    }

    // 오브젝트 풀 클래스
    private class ObjectPool
    {
        private GameObject prefab;
        private Queue<GameObject> pool;
        private Transform parentTransform;

        public ObjectPool(GameObject prefab, int poolSize)
        {
            this.prefab = prefab;
            pool = new Queue<GameObject>();
            parentTransform = new GameObject(prefab.name + "_Pool").transform;  // 풀 객체를 하나의 GameObject로 묶음

            // 초기화된 풀에 오브젝트 추가
            for (int i = 0; i < poolSize; i++)
            {
                AddObjectToPool();
            }
        }

        // 오브젝트 추가
        private void AddObjectToPool()
        {
            GameObject newObj = Object.Instantiate(prefab, parentTransform);
            newObj.SetActive(false);
            pool.Enqueue(newObj);
        }

        // 오브젝트 가져오기
        public GameObject GetObject()
        {
            if (pool.Count == 0)
            {
                AddObjectToPool();
            }

            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            obj.transform.SetParent(null);  // 부모를 초기화하여 풀 밖으로 꺼내기

            // 랜덤 색상 적용 (예시)
            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                objRenderer.material.color = new Color(Random.value, Random.value, Random.value);
            }

            return obj;
        }

        // 오브젝트 반환
        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(parentTransform);  // 풀로 다시 넣기
            pool.Enqueue(obj);
        }
    }
}
