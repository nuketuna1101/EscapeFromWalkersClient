using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [Header("Grid Settings")]
    public int gridWidth = 16;
    public int gridHeight = 8; 
    public float cellSize = 1.0f;
    public Vector3 gridOrigin = new Vector3(-8, -4, 0);


    private void Start()
    {
        // 가져오기
    }

    public void InitGridAssets()
    {
        // 서버로부터 가져오기
        SocketManager.Instance.GetMapAssets();

    }

}
