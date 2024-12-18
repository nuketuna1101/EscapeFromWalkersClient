using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public abstract class Monster : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]
    private MonsterStatsSO monsterStatsSO;
    protected float respawnCycle;
    protected float health;
    protected float attackDamage;
    public float attackCooltime;
    public float attackRange;
    protected float maxHealth;

    [Header("Tracking")]
    public float sightRange = 8.0f;
    public float trackSpeed = 1.0f;

    [Header("Main Components")]
    public Animator anim;
    public SpriteRenderer spriter;
    public IMonsterState myState;

    [Header("Floating HP bar UI")]
    private Slider HPbar;
    private Coroutine updateHPbarCoroutine;
    private const float HPbarHeight = 1.0f;

    protected virtual void Awake()
    {
        anim = this.GetComponent<Animator>();
        spriter = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        // Ǯ���� ���� �̿��ϱ� ������ Ȱ��ȭ �κ� �ڵ�� �ʱ�ȭ �κ�.
        // �ʱ�ȭ �׸�: ���� �ɷ�ġ, ��Ʋ�Ŵ��� ���, �⺻idle ���� ����, HP Bar UI �Ҵ�
        InitStatFromSO();
        InitHPUI();
        BattleManager.Instance.RegisterMonster(this);
        TransitionState(new MIdleState(this));
    }
    protected void InitStatFromSO()
    {
        respawnCycle = monsterStatsSO.respawnCycle;
        health = monsterStatsSO.health;
        attackDamage = monsterStatsSO.attackDamage;
        attackCooltime = monsterStatsSO.attackCooltime;
        attackRange = monsterStatsSO.attackRange;
        maxHealth = monsterStatsSO.health;
    }
    protected void InitHPUI()
    {
        // ü�¹� Ǯ�κ��� ü�¹� ui�� �����ͼ� ĵ������ ���� ��, ���� ������Ʈ�� ����ٴϵ��� �ڷ�ƾ ����.
        Canvas canvas = FindObjectOfType<Canvas>();
        HPbar = HPbarPoolManager.GetFromPool().GetComponent<Slider>();
        HPbar.transform.SetParent(canvas.transform);
        HPbar.maxValue = maxHealth;
        HPbar.value = health;
        CoroutineHelper.RestartCor(this, ref updateHPbarCoroutine, UpdateHPbarRoutine());
    }
    public void TransitionState(IMonsterState nextState)
    {
        if (myState != null) myState.Exit();
        myState = nextState;
        myState.Enter();
    }
    public void SetAnimBool(string paramName, bool boolVal)
    {
        anim.SetBool(paramName, boolVal);
    }
    public void SetAnimTrigger(string paramName)
    {
        anim.SetTrigger(paramName);
    }
    public void ResetAnimTrigger(string paramName)
    {
        anim.ResetTrigger(paramName);
    }
    public abstract void BasicAttack();
    public virtual void BeAttacked(float damage)
    {
        health -= damage;
        if (health > 0)
        {
            SetAnimTrigger("BeAttacked");
        }
        else
        {
            Die();
            BattleManager.Instance.DeregisterMonster(this);
        }
    }
    protected virtual void Die()
    {
        SetAnimTrigger("Death");
    }
    public void ReturnToPool()
    {
        BattleManager.Instance.DeregisterMonster(this);
        MonsterPoolManager.ReturnToPool(this.gameObject);
    }

    private IEnumerator UpdateHPbarRoutine()
    {
        while (true)
        {
            yield return null;
            Vector3 HPbarPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + HPbarHeight, 0));
            HPbar.transform.position = HPbarPosition;
            if (HPbar != null)
            {
                HPbar.value = health;
            }
        }
    }
    private void OnDisable()
    {
        CoroutineHelper.StopCor(this, ref updateHPbarCoroutine);
        if (HPbar != null)
            HPbarPoolManager.ReturnToPool(HPbar.gameObject);
    }




    public void GetStunned(float duration)
    {
        this.TransitionState(new MStunnedState(this, myState.TargetPlayer, myState, duration));
    }

}
