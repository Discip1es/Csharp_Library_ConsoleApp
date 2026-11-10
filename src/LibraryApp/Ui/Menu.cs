// Ui/Menu.cs
namespace LibraryApp.Ui;

public class Menu
{
    private readonly List<IMenuCommand> _commands = new();
    private readonly IConsole _console;
    public Menu(IConsole console) => _console = console;

    public void Add(IMenuCommand cmd) => _commands.Add(cmd);

    public async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            _console.WriteLine("");
            _console.WriteLine("=== Библиотека ===");
            for (int i = 0; i < _commands.Count; i++)
                _console.WriteLine($"{i + 1}. {_commands[i].Name}");
            _console.WriteLine("0. Выход");
            _console.Write("> ");

            var input = _console.ReadLine();
            if (input is null || input == "0") return;

            if (int.TryParse(input, out var n) && n >= 1 && n <= _commands.Count)
            {
                try { await _commands[n - 1].ExecuteAsync(ct); }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex) { _console.WriteLine($"Ошибка: {ex.Message}"); }
            }
            else _console.WriteLine("Неверный выбор");
        }
    }
}