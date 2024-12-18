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
    private SocketIOClient.SocketIO socket;
    private const string serverURL = "http://localhost:3010";


    public GameObject playerPrefab;
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    private SynchronizationContext mainThreadContext;

    void Start()
    {
        // 메인 스레드 SynchronizationContext 저장
        mainThreadContext = SynchronizationContext.Current;
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
        });

        // 위치 업데이트
        socket.On("locationUpdate", async response =>
        {
            // EAP TAP 이벤트 / 태스크
            DebugOpt.Log("[locationUpdate] lu res received");

            // TO DO :: 쓰레드 로그 찍어서 메인인지 확인
            mainThreadContext.Post(async _ => 
            {
                // TO DO :: action은 queue로 관리
                // TO DO ::main thread에서 관리되는 queue와 lock으로 비동기에 대한 순차처리
                try
                {
                    var playerData = response.GetValue<ResponseDTO>();
                    await HandleLocationUpdateAsync(playerData);
                }
                catch (Exception e)
                {
                    DebugOpt.LogError("[Error] Exception occurred: " + e.Message);
                }

            }, null);

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

    async private Task HandleLocationUpdateAsync(ResponseDTO playerData)
    {
        Vector3 position = playerData.position.ToVector3();
        DebugOpt.Log("[SM] HandleLocationUpdateAsync called");

        // 플레이어가 존재하지 않으면 새로 생성
        if (!players.ContainsKey(playerData.uuid))
        {
            DebugOpt.Log("[SM] 새로 생성");
            GameObject newPlayer = PoolManager.Instance.GetObject();
            newPlayer.transform.position = Vector3.zero;
            //newPlayer.transform.position = position;
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

}


