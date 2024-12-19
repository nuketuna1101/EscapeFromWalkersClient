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
using System.Threading;

public class SocketManager : Singleton<SocketManager>
{
    // socket vars
    [Header("Socket Manage")]
    private SocketIOClient.SocketIO socket;
    private const string serverURL = "http://localhost:3010";
    private readonly Queue<Action> actionQueue = new Queue<Action>();
    private readonly object queueLock = new object();

    [Header("Player Manage")]
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    private GameObject localPlayer;

    void Start()
    {
        // SocketIO 인스턴스를 생성하고 서버와 연결
        socket = new SocketIOClient.SocketIO(serverURL);

        // 연결이 성공하면 호출되는 이벤트 핸들러
        socket.OnConnected += async (sender, e) =>
        {
            DebugOpt.Log("[SocketIOClient] Connected to Server");
            await PingTest();
        };

        // 'pong' 이벤트 리스닝
        socket.On("pong", response =>
        {
            DebugOpt.Log("[Pong] pong received");
            lock (queueLock)
            {
                actionQueue.Enqueue(InitPlayer);
            }
        });

        // 위치 업데이트
        socket.On("locationUpdate", async response =>
        {
            lock (queueLock)
            {
                actionQueue.Enqueue(() =>
                {
                    try
                    {
                        var playerData = response.GetValue<ResponseDTO>();
                        HandleLocationUpdate(playerData);
                    }
                    catch (Exception e)
                    {
                        DebugOpt.LogError("[Error] Exception occurred: " + e.Message);
                    }
                });
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
        DebugOpt.Log("[Ping] Ping test");
    }

    private void InitPlayer()
    {
        // pong 받으면 플레이어 초기화
        DebugOpt.Log("[SocketManager] Initializing local player");

        // 로컬 플레이어 초기화
        localPlayer = PoolManager.Instance.GetObject();
        if (localPlayer != null)
        {
            localPlayer.transform.position = Vector3.zero;
            players[socket.Id] = localPlayer;

            // 로컬 플레이어 컨트롤러 추가
            var controller = localPlayer.AddComponent<PlayerController>();
            controller.InitPlayerController(localPlayer);

            DebugOpt.Log("[SocketManager] Local player initialized with controller");
        }
        else
        {
            DebugOpt.LogError("[SocketManager] Failed to initialize local player");
        }
    }


    private void HandleLocationUpdate(ResponseDTO playerData)
    {
        Vector3 position = playerData.position.ToVector3();
        DebugOpt.Log("[SM] HandleLocationUpdateAsync called");

        // 플레이어가 존재하지 않으면 새로 생성
        if (!players.ContainsKey(playerData.uuid))
        {
            DebugOpt.Log("[SM] 새로 생성");
            GameObject newPlayer = PoolManager.Instance.GetObject();
            newPlayer.transform.position = Vector3.zero;
            players.Add(playerData.uuid, newPlayer);
        }
        else
        {
            // 위치 업데이트
            DebugOpt.Log("[SM] update location");
            players[playerData.uuid].transform.position = position;
        }
    }


    void OnApplicationQuit()
    {
        foreach (var player in players.Keys)
        {
            RemovePlayer(player);  // 모든 플레이어를 제거하고 반환
        }
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
        DebugOpt.Log("[SM] send player position emitted");
        socket.EmitAsync("location", positionData);
    }

    public void RemovePlayer(string uuid)
    {
        if (players.ContainsKey(uuid))
        {
            GameObject player = players[uuid];
            PoolManager.Instance.ReturnObject(player);
            players.Remove(uuid);
        }
    }


    private void Update()
    {
        lock (queueLock) 
        {
            while (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                action.Invoke();
            }
        }
    }

}


