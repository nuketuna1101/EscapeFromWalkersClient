//using System.Collections;
//using UnityEngine;
//using SocketIO;
//using Quobject.SocketIoClientDotNet.Client;
//using SocketIOClient;
//using System.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using SocketIOClient;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class SocketManager : MonoBehaviour
{
    public static SocketManager Instance { get; private set; }

    private SocketIOClient.SocketIO socket;
    private const string serverURL = "http://localhost:3010";


    public GameObject playerPrefab; // 플레이어 프리팹
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();


    // 싱글턴 인스턴스 설정
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // SocketIO 인스턴스를 생성하고 서버와 연결
        socket = new SocketIOClient.SocketIO(serverURL);

        // 연결이 성공하면 호출되는 이벤트 핸들러
        socket.OnConnected += async (sender, e) =>
        {
            Debug.Log("[SocketIOClient] Connected to Server");
            await PingTest();
        };

        // 'pong' 이벤트 리스닝
        socket.On("pong", response =>
        {
            Debug.Log("[Pong] pong received");
        });

        // 위치 업데이트
        socket.On("locationUpdate", response =>
        {
            Debug.Log("[Test] test");
            // response에서 uuid와 position을 꺼내서 로그로 출력
            try
            {
                var playerData = response.GetValue<ResponseDTO>();
                Debug.Log($"[LocationUpdate] UUID: {playerData.uuid}, Position: {playerData.position.ToVector3()}");
                // 플레이어가 존재하지 않으면 새로 생성
                if (!players.ContainsKey(playerData.uuid))
                {
                    GameObject newPlayer = Instantiate(playerPrefab, playerData.position.ToVector3(), Quaternion.identity);
                    players.Add(playerData.uuid, newPlayer);
                }
                else
                {
                    // 존재하면 위치 업데이트
                    players[playerData.uuid].transform.position = playerData.position.ToVector3();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Error] Exception occurred: " + e.Message);
            }
        });

        // 연결 시작
        socket.ConnectAsync();
    }

    // 핑 테스트를 위한 함수
    async private Task PingTest()
    {
        // 서버에 'ping' 이벤트를 보냄
        await socket.EmitAsync("ping", "ping data");
        Debug.Log("[Ping] Ping test");
    }

    void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 서버 연결 종료
        socket.DisconnectAsync();
    }

    public void SendPlayerPosition(Vector3 position)
    {
        var positionData = new
        {
            uuid = socket.Id,
            position = new { x = position.x, y = position.y, z = position.z }
        };
        socket.EmitAsync("location", positionData);
    }
}


