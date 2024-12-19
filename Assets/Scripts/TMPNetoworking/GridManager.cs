using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [Header("Grid Settings")]
    private const int gridWidth = 16;
    private const int gridHeight = 8;
    private const float cellSize = 1.0f;
    private Vector3 gridOrigin = new Vector3(-8, -4, 0);
    private List<MapDataDTO> mapAssets { get; set; }


    public void SetMapAsset(List<MapDataDTO> mapAssets)
    {
        this.mapAssets = mapAssets;
        // 임시 테스트
        InitGridByMapAsset(101);
    }

    public void InitGridByMapAsset(int id)
    {
        // 지정된 id의 그리드 맵 초기화
        // 추후엔 scriptable object 이용도 가능?    
        MapDataDTO mapData = mapAssets.Find(map => map.id == id);
        if (mapData == null)
        {
            Debug.LogError("Map data with given ID not found.");
            return;
        }

        // 해당 id의 map_grid 출력
        //foreach (var i in mapData.map_grid)
        //{
        //    string tmp = "";
        //    foreach (var j in i)
        //    {
        //        tmp += $"{j}, ";
        //        //Debug.Log("mapData: " + j);
        //    }
        //    tmp += "\n";
        //    Debug.Log(tmp);
        //}

        // map_grid에서 각 값에 따라 프리팹 생성
        for (int y = 0; y < mapData.map_grid.Count; y++)
        {
            for (int x = 0; x < mapData.map_grid[y].Count; x++)
            {
                int cellValue = mapData.map_grid[y][x];

                // cellValue가 1이면 해당 위치에 프리팹을 생성
                if (cellValue == 1)
                {
                    Vector3 spawnPosition = gridOrigin + new Vector3(x * cellSize, y * cellSize, 0);
                    GameObject tmp = PoolManager.Instance.GetObject();
                    tmp.transform.position = spawnPosition;
                }
            }
        }
    }
}
