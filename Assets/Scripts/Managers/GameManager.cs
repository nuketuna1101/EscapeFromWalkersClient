using Quobject.SocketIoClientDotNet.Client;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager Instance { get; private set; }

    public const float spawnInterval = 0.5f;
    private const int level = 1;
    private int remainMonstersToSpawn;
    private Coroutine checkWinConditionCoroutine;
    private Coroutine spawnMonsterCoroutine;
    // 싱글턴 인스턴스 설정
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += CallWithSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= CallWithSceneLoaded;
    }
    private void CallWithSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "2.Ingame")
            StartBattle();
    }
    public void StartBattle()
    {
        Presetting();
        CheckWinCondition();
    }
    private void Presetting()
    {
        InitLevel();
        SpawnPlayers();
        SpawnMonster();
        SetCameraTracking();
    }
    private void InitLevel()
    {
        remainMonstersToSpawn = 8 + 4 * level;
    }
    private void SpawnPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            var playerObj = PlayerPoolManager.GetFromPool((PlayerClass)i);
            playerObj.transform.position = new Vector3((i + 1), (i % 2) * (-2), 0.0f);
        }
    }
    private void SpawnMonster()
    {
        DebugOpt.Log("SpawnMonster called");
        CoroutineHelper.RestartCor(this, ref spawnMonsterCoroutine, SpawnMonstersRoutine());
    }
    private IEnumerator SpawnMonstersRoutine()
    {
        while (remainMonstersToSpawn > 0)
        {
            yield return new WaitForSeconds(spawnInterval);
            GameObject goblinObj = MonsterPoolManager.GetFromPool();
            if (goblinObj == null) continue;
            //goblinObj.transform.position = new Vector3(-1, 0, 0.0f); 
            goblinObj.transform.position = BattleManager.Instance.SpawnOnRandomPosition(5f, 7f);
            remainMonstersToSpawn--;
        }
    }
    private void SetCameraTracking()
    {
        Camera camera = FindObjectOfType<Camera>();
        CameraController cameraController = camera.GetComponent<CameraController>();
        cameraController.InitCameraControl();
    }

    private void CheckWinCondition()
    {
        CoroutineHelper.RestartCor(this, ref checkWinConditionCoroutine, CheckWinConditionCoroutine());
    }
    private IEnumerator CheckWinConditionCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            yield return null;
            if (remainMonstersToSpawn <= 0 && BattleManager.Instance.isAllMonstersCleared())
            {
                // �¸�
                VictoryEnd();
                yield break;
            }
            if (!BattleManager.Instance.isAnyPlayerAlive())
            {
                // �й�
                DefeatEnd();
                yield break;
            }
        }
    }

    private void VictoryEnd()
    {
        DebugOpt.Log("GameManager: Victory");
        BattleManager.Instance.PlayerVictory();
        UIManager.Instance.ShowPopupWindow("Victory");
    }
    private void DefeatEnd()
    {
        DebugOpt.Log("GameManager: Defeat");
        Time.timeScale = 0;
        UIManager.Instance.ShowPopupWindow("Defeat");
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // ���ø����̼� ����
#endif
    }
}