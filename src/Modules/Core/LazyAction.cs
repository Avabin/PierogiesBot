namespace Core;

public class LazyAction
{
    private readonly Lazy<Void> _lazy;

    public LazyAction(Action action)
    {
        _lazy = new Lazy<Void>(() =>
        {
            action();

            return Void.Default;
        });
    }

    public Void Lazy => _lazy.Value;

    public void Execute()
    {
        _ = Lazy;
    }
}