using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.TextCore.Text;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Threading;

public class PMoveState : IPlayerState
{
    private Player player;
    private Monster targetMonster; public Monster TargetMonster { get { return targetMonster; } }
    private Coroutine moveCoroutine;
    public PMoveState(Player player, Monster targetMonster)
    {
        this.player = player;
        this.targetMonster = targetMonster;
    }
    public void Enter()
    {
        player.SetAnimBool("MoveState", true);
        CoroutineHelper.RestartCor(player, ref moveCoroutine, MoveRoutine());
    }
    public void Exit()
    {
        player.SetAnimBool("MoveState", false);
        CoroutineHelper.StopCor(player, ref moveCoroutine);
    }
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            DebugOpt.Log("MoveState");
            Vector2 direction = (targetMonster.transform.position - player.transform.position).normalized;
            player.transform.rotation = Quaternion.Euler(0, (direction.x >= 0 ? 180 : 0), 0);
            player.transform.position = Vector2.MoveTowards(player.transform.position, targetMonster.transform.position, player.trackSpeed * Time.deltaTime);
            float distance = Vector2.Distance(player.transform.position, targetMonster.transform.position);
            if (distance <= player.attackRange)
            {
                player.TransitionState(new PAttackState(player, targetMonster));
                yield break;
            }
            else if (distance > player.sightRange)
            {
                player.TransitionState(new PIdleState(player));
                yield break;
            }
            yield return null;
        }
    }
}
