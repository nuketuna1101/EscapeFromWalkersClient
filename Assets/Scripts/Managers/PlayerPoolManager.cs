using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public enum PlayerClass
{
    Knight = 0,
    Thief = 1,
    Archer = 2,
    Priest = 3,
}

public class PlayerPoolManager : Singleton<PlayerPoolManager>
{
    [SerializeField]
    private GameObject[] prefabs;                                  // ������Ʈ ������
    [SerializeField]
    private const int initPoolSize = 4;                           // �ʱ� Ǯ ������ ����
    //Queue<GameObject> pool = new Queue<GameObject>();              // ������ Ǯ�� �̿��� ť
    GameObject[] pool = new GameObject[initPoolSize];

    private void Awake()
    {
        InitPool();
    }
    public void InitPool()
    {
        for (int i = 0; i < initPoolSize; i++)
            pool[i] = CreateObj((PlayerClass)i);
    }
    private GameObject CreateObj(PlayerClass playerClass)
    {
        var newObj = Instantiate(prefabs[(int)playerClass]);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }
    public static GameObject GetFromPool(PlayerClass playerClass)
    {
        // ��û �� Ǯ�� �ִ� ������Ʈ�� �Ҵ����ش�.
        if (Instance.pool[(int)playerClass] != null)
        {
            var obj = Instance.pool[(int)playerClass];
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
            return null;
    }
    public static void ReturnToPool(GameObject obj)
    {
        // ������Ʈ ��Ȱ��ȭ��Ű�� �ٽ� Ǯ�� ���ͽ�Ű��
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.pool[(int)obj.GetComponent<Player>().playerClass] = obj;
    }
}
