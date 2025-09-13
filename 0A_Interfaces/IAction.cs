
public interface IAction
{
    public abstract bool IsDead {get; set;}
    public abstract bool IsPerformingOneShotAction { get; set; }
    public abstract void ChangeStateToDeath();
}
