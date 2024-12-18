using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MIdleState : IMonsterState
{
    private Monster monster;
    private Player targetPlayer; public Player TargetPlayer { get { return targetPlayer; } }

    private Coroutine idleCoroutine;

    public MIdleState(Monster monster)
    {
        this.monster = monster;
    }
    public void Enter()
    {
        CoroutineHelper.RestartCor(monster, ref idleCoroutine, IdleRoutine());
    }
    public void Exit()
    {
        CoroutineHelper.StopCor(monster, ref idleCoroutine);
    }
    private IEnumerator IdleRoutine()
    {
        while (true)
        {
            DebugOpt.Log(monster.name + "  : IdleRoutine : is on going");

            Player player = BattleManager.Instance.FindNearestPlayer(monster);
            if (player != null)
            {
                monster.TransitionState(new MMoveState(monster, player));
                yield break;
            }
            yield return null;
            //yield return new WaitForSeconds(0.1f);
        }
    }
}
