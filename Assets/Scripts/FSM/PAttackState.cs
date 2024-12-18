using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.TextCore.Text;

public class PAttackState : IPlayerState
{
    private Player player;
    private Monster targetMonster;      public Monster TargetMonster { get { return targetMonster; } }
    private Coroutine attackCoroutine;
    private Coroutine attackCooldownCoroutine;
    private Coroutine skillCooldownCoroutine;

    private bool isAttackReady = true;   // 일반 공격 준비 여부
    private bool isSkillReady = false;  // 특수 공격 준비 여부

    public PAttackState(Player player, Monster targetMonster)
    {
        this.player = player;
        this.targetMonster = targetMonster;
    }
    public void Enter()
    {
        CoroutineHelper.RestartCor(player, ref attackCoroutine, AttackRoutine());
    }
    public void Exit()
    {
        CoroutineHelper.StopCor(player, ref attackCoroutine);
        CoroutineHelper.StopCor(player, ref attackCooldownCoroutine);
        CoroutineHelper.StopCor(player, ref skillCooldownCoroutine);
        targetMonster = null;
        player.ResetAnimTrigger("CastSkill");
        player.ResetAnimTrigger("BasicAttack");
    }
    private IEnumerator AttackRoutine()
    {
        CoroutineHelper.RestartCor(player, ref skillCooldownCoroutine, SkillCoolDownRoutine());
        while (true)
        {
            yield return null;
            // 공격타겟 계속 감지. 없으면 idle로 전환.
            if (targetMonster == null 
                || !targetMonster.gameObject.activeInHierarchy 
                || !BattleManager.Instance.isExistingMonster(targetMonster))
            {
                DebugOpt.Log("EscapeRoutine called");
                player.TransitionState(new PIdleState(player));
                yield break;
            }
            // 공격타겟 감지, 시야와 공격사거리 따라 추적내지는 idle 전환
            float distance = Vector2.Distance(player.transform.position, targetMonster.transform.position);
            if (distance > player.attackRange && distance <= player.sightRange)
            {
                player.TransitionState(new PMoveState(player, targetMonster));
                yield break;
            }
            else if (distance > player.sightRange)
            {
                player.TransitionState(new PIdleState(player));
                yield break;
            }
            // 스킬과 공격속도에 따른 공격로직
            if (isSkillReady)
            {
                DebugOpt.Log("me: " + player.name + " cast skill ");
                player.SetAnimTrigger("CastSkill");
                isSkillReady = false;
                CoroutineHelper.RestartCor(player, ref skillCooldownCoroutine, SkillCoolDownRoutine());
            }
            if (isAttackReady)
            {
                player.SetAnimTrigger("BasicAttack");
                isAttackReady = false;
                CoroutineHelper.RestartCor(player, ref attackCooldownCoroutine, AttackCoolDownRoutine());
            }
        }
    }
    private IEnumerator AttackCoolDownRoutine()
    {
        yield return new WaitForSeconds(player.attackCooltime);
        isAttackReady = true;
    }
    private IEnumerator SkillCoolDownRoutine()
    {
        yield return new WaitForSeconds(player.skillCooltime);
        isSkillReady = true;
    }
}
