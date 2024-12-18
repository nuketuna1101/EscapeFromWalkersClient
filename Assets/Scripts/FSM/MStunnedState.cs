using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MStunnedState : IMonsterState
{
    private Monster monster;
    private Player targetPlayer; public Player TargetPlayer { get { return targetPlayer; } }
    private Coroutine stunnedCoroutine;
    private IMonsterState prevState;
    private float duration;
    public MStunnedState(Monster monster, Player targetPlayer, IMonsterState prevState, float duration)
    {
        this.monster = monster;
        this.targetPlayer = targetPlayer;
        this.prevState = prevState;
        this.duration = duration;
    }
    public void Enter()
    {
        CoroutineHelper.RestartCor(monster, ref stunnedCoroutine, StunnedRoutine());
    }
    public void Exit()
    {
        CoroutineHelper.StopCor(monster, ref stunnedCoroutine);
    }
    private IEnumerator StunnedRoutine()
    {
        yield return new WaitForSeconds(duration);
        monster.TransitionState(prevState);
    }
}
