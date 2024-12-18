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

    private bool isAttackReady = true;   // �Ϲ� ���� �غ� ����

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
            // ����Ÿ�� ��� ����. ������ idle�� ��ȯ.
            if (targetPlayer == null 
                || !targetPlayer.gameObject.activeInHierarchy 
                || !BattleManager.Instance.isExistingPlayer(targetPlayer))
            {
                targetPlayer = null;
                monster.TransitionState(new MIdleState(monster));
                yield break;
            }
            // ����Ÿ�� ����, �þ߿� ���ݻ�Ÿ� ���� ���������� idle ��ȯ
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
            // ���ݼӵ��� ���� ���ݷ���
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
