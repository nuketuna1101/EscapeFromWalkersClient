using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    /// <summary>
    /// 몬스터 추상 클래스를 받아 고블린 클래스.
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
