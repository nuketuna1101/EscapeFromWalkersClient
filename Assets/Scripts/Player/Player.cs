using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public abstract class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]
    private PlayerStatsSO playerStatsSO;
    public abstract PlayerClass playerClass { get; }
    public float respawnCycle;
    public float health;
    protected float attackDamage;
    public float attackRange;
    public float attackCooltime;
    public float skillRange;
    public float skillCooltime;
    public float maxHealth;

    [Header("Tracking")]
    public float sightRange = 8.0f;
    public float trackSpeed = 1.0f;

    [Header("Main Components")]
    public Animator anim;
    public SpriteRenderer spriter;
    public IPlayerState myState;

    [Header("Floating HP bar UI")]
    private Slider HPbar;
    private Coroutine updateHPbarCoroutine;
    private const float HPbarHeight = 2.0f;

    [Header("HealEfx")]
    private Coroutine healEfxCoroutine;


    void Awake()
    {
        anim = this.GetComponent<Animator>();
        spriter = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        InitStatFromSO();
        InitHPUI();
        BattleManager.Instance.RegisterPlayer(this);
        TransitionState(new PIdleState(this));
    }
    protected void InitStatFromSO()
    {
        // SO�κ��� ���� ���� �ʱ�ȭ
        respawnCycle = playerStatsSO.respawnCycle;
        health = playerStatsSO.health;
        attackDamage = playerStatsSO.attackDamage;
        attackRange = playerStatsSO.attackRange;
        attackCooltime = playerStatsSO.attackCooltime;
        skillRange = playerStatsSO.skillRange;
        skillCooltime = playerStatsSO.skillCooltime;
        maxHealth = playerStatsSO.health;
    }
    protected void InitHPUI()
    {
        // ü�¹� Ǯ�κ��� ü�¹� ui�� �����ͼ� ĵ������ ���� ��, ���� ������Ʈ�� ����ٴϵ��� �ڷ�ƾ ����.
        Canvas canvas = FindObjectOfType<Canvas>();
        HPbar = PlayerHPbarPoolManager.GetFromPool().GetComponent<Slider>();
        HPbar.transform.SetParent(canvas.transform);
        HPbar.maxValue = maxHealth;
        HPbar.value = health;
        CoroutineHelper.RestartCor(this, ref updateHPbarCoroutine, UpdateHPbarRoutine());
    }
    public void TransitionState(IPlayerState nextState)
    {
        if(myState != null) myState.Exit();
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
    protected void BasicAttack()
    {
        Monster targetMonster = myState.TargetMonster;
        if (targetMonster == null) return;
        BattleManager.Instance.AttackFromPlayerToMonster(this, targetMonster, attackDamage);
    }
    protected abstract void CastSkill();
    public virtual void BeAttacked(float damage)
    {
        health = (health - damage > 0 ? health - damage : 0);
        if (health > 0)
        {
            SetAnimTrigger("BeAttacked");
        }
        else
        {
            Die();
            // �̹� ���� �÷��̾ ���� ���͵��� ���� ����. �ٷ� ��Ʋ���� ���ܽ����ֱ�.
            BattleManager.Instance.DeregisterPlayer(this);
        }
    }
    protected virtual void Die()
    {
        SetAnimTrigger("Death");
    }

    public void ReturnToPool()
    {
        // ��Ȱ Ÿ�̸� ������ ������Ʈ�� Ǯ�� ȸ��
        BattleManager.Instance.RespawnPlayer(this);
        PlayerPoolManager.ReturnToPool(this.gameObject);
    }
    public virtual void BeHealed(float healAmount)
    {
        health = (maxHealth > health + healAmount ? health + healAmount : maxHealth);
        StartCoroutine(HealEfxRoutine());
    }
    private IEnumerator UpdateHPbarRoutine()
    {
        // ü�¹� HP UI�� ������Ʈ ����ٴϴ� �ڷ�ƾ. �׸��� ���� ü�¿� ���� �����̴� ������Ʈ
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
        CoroutineHelper.StopCor(this, ref healEfxCoroutine);
        if (HPbar != null)
            PlayerHPbarPoolManager.ReturnToPool(HPbar.gameObject);
    }



    private IEnumerator HealEfxRoutine()
    {
        yield return null;
        GameObject healEfx = HealEfxPoolManager.GetFromPool();
        healEfx.transform.position = this.transform.position;
        StartCoroutine(EfxTrackingRoutine(healEfx));
        yield return new WaitForSeconds(1.0f);
        StopCoroutine(EfxTrackingRoutine(healEfx));
        HealEfxPoolManager.ReturnToPool(healEfx);
    }
    private IEnumerator EfxTrackingRoutine(GameObject healEfx)
    {
        while (true)
        {
            yield return null;
            Vector3 HPbarPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + HPbarHeight, 0));
            HPbar.transform.position = HPbarPosition;
        }
    }
}