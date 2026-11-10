namespace LibraryApp.Ui;

public sealed class DelegateCommand : IMenuCommand
{
    private readonly Func<CancellationToken, Task> _action;
    public string Name { get; }
    public DelegateCommand(string name, Func<CancellationToken, Task> action)
    { Name = name; _action = action; }
    public Task ExecuteAsync(CancellationToken ct = default) => _action(ct);
}