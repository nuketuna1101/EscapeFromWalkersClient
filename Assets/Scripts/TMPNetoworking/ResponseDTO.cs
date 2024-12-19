using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseDTO
{
    public string status { get; set; }
    public string uuid { get; set; }
    public PositionDTO position { get; set; } // Vector3 대신 PositionDTO 사용
    public MapAssetDTO maps { get; set; }
}




public class PositionDTO
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class MapAssetDTO
{
    public string name { get; set; }  // JSON의 "name"과 매핑
    public string version { get; set; } // JSON의 "version"과 매핑
    public List<MapDataDTO> data { get; set; } // JSON의 "data"와 매핑
}


public class MapDataDTO
{
    public int id { get; set; }
    public List<List<int>> map_grid { get; set; } // 2D 배열 매핑
}