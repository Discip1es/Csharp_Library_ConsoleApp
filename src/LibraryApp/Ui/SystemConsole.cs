// Ui/IConsole.cs + SystemConsole.cs
namespace LibraryApp.Ui;

public sealed class SystemConsole : IConsole
{
    public string? ReadLine() => Console.ReadLine();
    public void WriteLine(string text) => Console.WriteLine(text);
    public void Write(string text) => Console.Write(text);
}