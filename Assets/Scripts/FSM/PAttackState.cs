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

    private bool isAttackReady = true;   // �Ϲ� ���� �غ� ����
    private bool isSkillReady = false;  // Ư�� ���� �غ� ����

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
            // ����Ÿ�� ��� ����. ������ idle�� ��ȯ.
            if (targetMonster == null 
                || !targetMonster.gameObject.activeInHierarchy 
                || !BattleManager.Instance.isExistingMonster(targetMonster))
            {
                DebugOpt.Log("EscapeRoutine called");
                player.TransitionState(new PIdleState(player));
                yield break;
            }
            // ����Ÿ�� ����, �þ߿� ���ݻ�Ÿ� ���� ���������� idle ��ȯ
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
            // ��ų�� ���ݼӵ��� ���� ���ݷ���
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
