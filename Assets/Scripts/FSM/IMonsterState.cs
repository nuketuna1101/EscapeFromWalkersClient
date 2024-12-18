public interface IMonsterState : IState
{
    public Player TargetPlayer { get; }
}