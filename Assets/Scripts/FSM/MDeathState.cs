using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDeathState : IMonsterState
{
    /// <summary>
    /// 사망 처리를 위해 만들었던 death state이나, 로직을 변경함에 따라 쓰이지 않을 legacy 코드. 하지만 임시 백업.
    /// </summary>
    private Monster monster;
    private Player targetPlayer; public Player TargetPlayer { get { return targetPlayer; } }
    private Coroutine deathCoroutine;

    public MDeathState(Monster monster)
    {
        this.monster = monster;
    }
    public void Enter()
    {
        CoroutineHelper.RestartCor(monster, ref deathCoroutine, DeathCoroutine());
    }
    public void Exit()
    {
        CoroutineHelper.StopCor(monster, ref deathCoroutine);
    }
    private IEnumerator DeathCoroutine()
    {
        yield return null;
        monster.SetAnimTrigger("Death");
        BattleManager.Instance.DeregisterMonster(monster);
        MonsterPoolManager.ReturnToPool(monster.gameObject);
    }
}
