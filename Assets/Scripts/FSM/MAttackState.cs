using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.TextCore.Text;

public class MAttackState : IMonsterState
{
    private Monster monster;
    private Player targetPlayer;    public Player TargetPlayer { get { return targetPlayer; } }
    private Coroutine attackCoroutine;
    private Coroutine attackCooldownCoroutine;
    private Coroutine skillCooldownCoroutine;

    private bool isAttackReady = true;   // 일반 공격 준비 여부

    public MAttackState(Monster monster, Player targetPlayer)
    {
        this.monster = monster;
        this.targetPlayer = targetPlayer;
    }
    public void Enter()
    {
        CoroutineHelper.RestartCor(monster, ref attackCoroutine, AttackRoutine());
    }
    public void Exit()
    {
        CoroutineHelper.StopCor(monster, ref attackCoroutine);
        CoroutineHelper.StopCor(monster, ref attackCooldownCoroutine);
        CoroutineHelper.StopCor(monster, ref skillCooldownCoroutine);
        targetPlayer = null;
        monster.ResetAnimTrigger("CastSkill");
        monster.ResetAnimTrigger("BasicAttack");
    }
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return null;
            // 공격타겟 계속 감지. 없으면 idle로 전환.
            if (targetPlayer == null 
                || !targetPlayer.gameObject.activeInHierarchy 
                || !BattleManager.Instance.isExistingPlayer(targetPlayer))
            {
                targetPlayer = null;
                monster.TransitionState(new MIdleState(monster));
                yield break;
            }
            // 공격타겟 감지, 시야와 공격사거리 따라 추적내지는 idle 전환
            float distance = Vector2.Distance(monster.transform.position, targetPlayer.transform.position);
            if (distance > monster.attackRange && distance <= monster.sightRange)
            {
                monster.TransitionState(new MMoveState(monster, targetPlayer));
                yield break;
            }
            else if (distance > monster.sightRange)
            {
                targetPlayer = null;
                monster.TransitionState(new MIdleState(monster));
                yield break;
            }
            // 공격속도에 따른 공격로직
            if (isAttackReady)
            {
                monster.SetAnimTrigger("BasicAttack");
                isAttackReady = false;
                CoroutineHelper.RestartCor(monster, ref attackCooldownCoroutine, AttackCoolDownRoutine());
            }
        }
    }
    private IEnumerator AttackCoolDownRoutine()
    {
        yield return new WaitForSeconds(monster.attackCooltime);
        isAttackReady = true;
    }
}
