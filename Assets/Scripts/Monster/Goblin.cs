using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    /// <summary>
    /// ���� �߻� Ŭ������ �޾� ��� Ŭ����.
    /// </summary>
    protected override void Awake()
    {
        anim = this.transform.GetChild(0).GetComponent<Animator>();
        spriter = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public override void BasicAttack()
    {
        Player targetPlayer = myState.TargetPlayer;
        if (targetPlayer == null) return;

        BattleManager.Instance.AttackFromMonsterToPlayer(this, targetPlayer, attackDamage);
    }
    private void Update()
    {
        DebugOpt.Log(this.name + " Goblin : my state : " + this.myState);
    }
}
