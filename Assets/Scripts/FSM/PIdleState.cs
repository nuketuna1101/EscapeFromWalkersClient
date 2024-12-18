using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PIdleState : IPlayerState
{
    private Player player;
    private Monster targetMonster; public Monster TargetMonster { get { return targetMonster; } }
    private Coroutine idleCoroutine;

    public PIdleState(Player player)
    {
        this.player = player;
    }

    public void Enter()
    {
        CoroutineHelper.RestartCor(player, ref idleCoroutine, IdleRoutine());
    }

    public void Exit()
    {
        CoroutineHelper.StopCor(player, ref idleCoroutine);
    }

    private IEnumerator IdleRoutine()
    {
        while (true)
        {
            DebugOpt.Log("IdleState");
            Monster monster = BattleManager.Instance.FindNearestMonster(player);
            if (monster != null)
            {
                player.TransitionState(new PMoveState(player, monster));
                yield break;
            }
            yield return null;
        }
    }
}
