namespace LibraryApp.Ui;

public interface IMenuCommand
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken ct = default);
}