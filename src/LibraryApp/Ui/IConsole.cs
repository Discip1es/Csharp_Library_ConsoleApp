// Ui/IConsole.cs + SystemConsole.cs
namespace LibraryApp.Ui;

public interface IConsole
{
    string? ReadLine();
    void WriteLine(string text);
    void Write(string text);
}