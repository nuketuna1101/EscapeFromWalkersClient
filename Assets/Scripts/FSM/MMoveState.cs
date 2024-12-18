using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.TextCore.Text;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MMoveState : IMonsterState
{
    /// <summary>
    /// 몬스터의 이동 상태: 사실 정확히는 추적임. 애초에 추적 대상을 감지하고 move state에 진입함.
    /// 추적 범위를 벗어나면, idle 상태로 변화
    /// 추적 후 공격범위 안에 들어오면 공격 상태로 진입
    /// </summary>
    private Monster monster;
    private Player targetPlayer; public Player TargetPlayer { get { return targetPlayer; } }
    private Coroutine moveCoroutine;
    public MMoveState(Monster monster, Player targetPlayer)
    {
        this.monster = monster;
        this.targetPlayer = targetPlayer;
    }
    public void Enter()
    {
        monster.SetAnimBool("MoveState", true);
        CoroutineHelper.RestartCor(monster, ref moveCoroutine, MoveRoutine());
    }
    public void Exit()
    {
        monster.SetAnimBool("MoveState", false);
        CoroutineHelper.StopCor(monster, ref moveCoroutine);
    }
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            Vector2 direction = (targetPlayer.transform.position - monster.transform.position).normalized;
            monster.transform.rotation = Quaternion.Euler(0, (direction.x >= 0 ? 0 : 180), 0);
            monster.transform.position = Vector2.MoveTowards(monster.transform.position, targetPlayer.transform.position, monster.trackSpeed * Time.deltaTime);
            float distance = Vector2.Distance(monster.transform.position, targetPlayer.transform.position);
            if (distance <= monster.attackRange)
            {
                monster.TransitionState(new MAttackState(monster, targetPlayer));
                yield break;
            }
            else if (distance > monster.sightRange)
            {
                monster.TransitionState(new MIdleState(monster));
                yield break;
            }
            yield return null;
        }
    }
}
