using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleManager : MonoBehaviour
{
    // singleton
    public static BattleManager Instance { get; private set; }

    private List<Monster> monsters = new List<Monster>();
    private Player[] players = new Player[4]; public Player[] Players { get { return players; } }


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
    }

    public void RegisterMonster(Monster monster)
    {
        monsters.Add(monster);
    }
    public void RegisterPlayer(Player player)
    {
        DebugOpt.Log("BattleManager : RegisterPlayer : " + (player == null) + " :: " + (int)player.playerClass + " :: " + player.name);
        if (player == null) return;
        players[(int)player.playerClass] = player;
    }
    public void DeregisterMonster(Monster monster)
    {
        DebugOpt.Log("BattleManager : DeregisterMonster : " + monster.name);
        monsters.Remove(monster);
    }
    public void DeregisterPlayer(Player player)
    {
        if (player == null) return;
        players[(int)player.playerClass] = null;
    }
    public bool isExistingPlayer(Player player)
    {
        return (players[(int)player.playerClass] != null);
    }
    public bool isExistingMonster(Monster monster)
    {
        return (monsters.Contains(monster));
    }
    public bool isAnyPlayerAlive()
    {
        // �� ���̶� �÷��̾� ĳ���� �������ִ���
        foreach (Player player in players)
        {
            if (player != null) return true;
        }
        return false;
    }
    public bool isAllMonstersCleared()
    {
        // �� ���̶� �÷��̾� ĳ���� �������ִ���
        return monsters.Count == 0;
    }
    public int test()
    {
        return monsters.Count;

    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ���� ���� ���� �Ŵ�¡
    /// </summary>
    public void AttackFromPlayerToMonster(Player player, Monster targetMonster, float damage)
    {
        if (player == null || targetMonster == null) return;
        targetMonster.BeAttacked(damage);
    }
    public void AttackFromMonsterToPlayer(Monster monster, Player targetPlayer, float damage)
    {
        if (targetPlayer == null || monster == null) return;
        targetPlayer.BeAttacked(damage);
    }
    public void AttackAreaFromPlayer(Player player, float skillRange, float damage)
    {
        // ��������: skill Range �� ��� monster �˻��ؼ�
        foreach (Monster monster in monsters)
        {
            if (monster == null) continue;
            if (isInArea(monster, player.transform.position, skillRange))
                monster.BeAttacked(damage);
        }
    }
    public void HealPlayer(Player healer, Player targetPlayer, float amount)
    {
        if (healer == null || targetPlayer == null) return;
        targetPlayer.BeHealed(amount);
    }
    public void HealAnyPlayerInRange(Player healer, float skillRange, float amount)
    {
        foreach (Player player in players)
        {
            if (player == null
                || !isInArea(player, healer.transform.position, skillRange)
                || player.health == player.maxHealth)
                continue;
            player.BeHealed(amount);
            break;
        }
    }
    public void GiveStunned(Player player, Monster targetMonster, float duration)
    {
        targetMonster.GetStunned(duration);
    }
    // ������� �ʾ����� �˹� ���� �ڵ�
    private void KnockBackFromPlayerToMonster(Player player, Monster monster)
    {
        Vector3 direction = (monster.transform.position - player.transform.position).normalized;
        monster.transform.position = monster.transform.position + direction * 0.25f;
    }
    private void KnockBackFromMonsterToPlayer(Monster monster, Player player)
    {
        Vector3 direction = (player.transform.position - monster.transform.position).normalized;
        player.transform.position = player.transform.position + direction * 0.25f;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ��� Ž�� ����
    public Player FindNearestPlayer(Monster monster)
    {
        float shortestDistance = Mathf.Infinity;
        Player nearest = null;
        foreach (Player player in players)
        {
            if (player == null) continue;

            float distanceToPlayer = Vector2.Distance(monster.transform.position, player.transform.position);
            if (distanceToPlayer > monster.sightRange) continue; // ���� ���͸�

            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearest = player;
            }
        }
        return nearest;
    }
    public Monster FindNearestMonster(Player player)
    {
        float shortestDistance = Mathf.Infinity;
        Monster nearest = null;
        foreach (Monster monster in monsters)
        {
            if (monster == null) continue;

            float distanceToPlayer = Vector2.Distance(monster.transform.position, player.transform.position);
            if (distanceToPlayer > player.sightRange) continue; // ���� ���͸�

            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearest = monster;
            }
        }
        return nearest;
    }
    private bool isInArea(Monster monster, Vector3 centerPoint, float range)
    {
        float distance = Vector2.Distance(monster.transform.position, centerPoint);
        return (distance <= range);
    }
    private bool isInArea(Player player, Vector3 centerPoint, float range)
    {
        float distance = Vector2.Distance(player.transform.position, centerPoint);
        return (distance <= range);
    }
    private Player GetPlayerLeftmost()
    {
        Player leftmostPlayer = null;
        foreach (Player player in players)
        {
            if (player == null) continue;

            if (leftmostPlayer == null) leftmostPlayer = player;
            else
            {
                Vector3 curPos = player.transform.position;
                Vector3 leftPos = leftmostPlayer.transform.position;
                if (curPos.x < leftPos.x)
                {
                    leftmostPlayer = player;
                }
            }
        }
        return leftmostPlayer;
    }
    private Player GetPlayerRightmost()
    {
        Player rightmostPlayer = null;
        foreach (Player player in players)
        {
            if (player == null) continue;

            if (rightmostPlayer == null) rightmostPlayer = player;
            else
            {
                Vector3 curPos = player.transform.position;
                Vector3 leftPos = rightmostPlayer.transform.position;
                if (curPos.x < leftPos.x)
                {
                    rightmostPlayer = player;
                }
            }
        }
        return rightmostPlayer;
    }
    public Vector3 SpawnOnRandomPosition(float minDistance, float maxDistance)
    {
        int flag = Random.Range(0, 2);
        if (flag == 0) return SpawnOnRandomLeftmost(minDistance, maxDistance);
        else return SpawnOnRandomRightmost(minDistance, maxDistance);
    }
    private Vector3 SpawnOnRandomLeftmost(float minDistance, float maxDistance)
    {
        // �÷��̾�� ������ ���� minDistance �̻� maxDistance ���� �Ÿ��� ������ ���� ��ǥ ��ȯ
        // ���� ã�� ���� ���Ѵ�Ⱑ �Ͼ�⿡ ���Ž� �ڵ�� ���.
        Vector3 randomPosition = Vector3.zero;
        bool isValid = false;
        Transform leftmostPlayerTransform = GetPlayerLeftmost().transform;
        while (!isValid)
        {
            randomPosition = new Vector3(
                Random.Range((-1) * maxDistance, 0),
                Random.Range((-1) * maxDistance, maxDistance),
                0.0f
            );
            isValid = true;

            float distance = Vector3.Distance(Vector3.zero, randomPosition);
            if (!(distance >= minDistance && distance <= maxDistance))
            {
                isValid = false;
                break;
            }
        }
        Vector3 result = new Vector3(randomPosition.x + leftmostPlayerTransform.position.x,
            randomPosition.y + leftmostPlayerTransform.position.y, 0);
        return result;
    }
    private Vector3 SpawnOnRandomRightmost(float minDistance, float maxDistance)
    {
        // �÷��̾�� ������ ���� minDistance �̻� maxDistance ���� �Ÿ��� ������ ���� ��ǥ ��ȯ
        // ���� ã�� ���� ���Ѵ�Ⱑ �Ͼ�⿡ ���Ž� �ڵ�� ���.
        Vector3 randomPosition = Vector3.zero;
        bool isValid = false;
        Transform rightmostPlayerTransform = GetPlayerRightmost().transform;
        while (!isValid)
        {
            randomPosition = new Vector3(
                Random.Range(0, maxDistance),
                Random.Range((-1) * maxDistance, maxDistance),
                0.0f
            );
            isValid = true;

            float distance = Vector3.Distance(Vector3.zero, randomPosition);
            if (!(distance >= minDistance && distance <= maxDistance))
            {
                isValid = false;
                break;
            }
        }
        Vector3 result = new Vector3(randomPosition.x + rightmostPlayerTransform.position.x,
            randomPosition.y + rightmostPlayerTransform.position.y, 0);
        return result;
    }

    public Vector3 SpawnOnRandomPosition_Legacy(float minDistance, float maxDistance)
    {
        // �÷��̾�� ������ ���� minDistance �̻� maxDistance ���� �Ÿ��� ������ ���� ��ǥ ��ȯ
        // ���� ã�� ���� ���Ѵ�Ⱑ �Ͼ�⿡ ���Ž� �ڵ�� ���.
        Vector3 randomPosition = Vector3.zero;
        bool isValid = false;
        while (!isValid)
        {
            randomPosition = new Vector3(
                Random.Range(-10, 11),
                Random.Range(-10, 11),
                0.0f
            );
            isValid = true;
            foreach (Player player in players)
            {
                if (player == null) continue;

                float distance = Vector3.Distance(player.transform.position, randomPosition);
                if (!(distance >= minDistance && distance <= maxDistance))
                {
                    isValid = false;
                    break;
                }
            }
        }
        return randomPosition;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ��Ʋ �������� �÷��̾� ĳ������ ��Ȱ ����
    /// </summary>
    private Coroutine[] respawnCoroutines = new Coroutine[4];
    public void RespawnPlayer(Player player)
    {
        DebugOpt.Log("BattleManager - RespawnPlayer called " + player.name);
        int index = (int)player.playerClass;
        CoroutineHelper.RestartCor(this, ref respawnCoroutines[index], RespawnRoutine(player));
    }
    private IEnumerator RespawnRoutine(Player player)
    {
        DebugOpt.Log("BattleManager - RespawnRoutine called " + player.name);
        yield return new WaitForSeconds(player.respawnCycle);
        var playerObj = PlayerPoolManager.GetFromPool(player.playerClass);
        int index = (int)player.playerClass;
        playerObj.transform.position = new Vector3((index + 1), (index % 2) * (-2), 0.0f);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PlayerVictory()
    {
        DebugOpt.Log("BattleManager:  PlayerVictory");
        foreach (var player in players)
        {
            player.SetAnimTrigger("Victory");
        }
    }
}
