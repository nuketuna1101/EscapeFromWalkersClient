using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class ResponseDTO
//{
//    public string status { get; set; }
//    public string uuid { get; set; }
//    public Vector3 position { get; set; }
//}

public class ResponseDTO
{
    public string status { get; set; }
    public string uuid { get; set; }
    public PositionDTO position { get; set; } // Vector3 대신 PositionDTO 사용
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
