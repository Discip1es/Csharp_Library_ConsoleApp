using LibraryApp.Domain;
using LibraryApp.Services;
using LibraryApp.Ui;
using Microsoft.Extensions.Logging;

namespace LibraryApp;

public class App
{
    private readonly IBookService _books;
    private readonly IMemberService _members;
    private readonly ILoanService _loans;
    private readonly IConsole _console;
    private readonly ILogger<App> _log;

    public App(IBookService books, IMemberService members, ILoanService loans,
               IConsole console, ILogger<App> log)
    { _books = books; _members = members; _loans = loans; _console = console; _log = log; }

    public async Task RunAsync(CancellationToken ct = default)
    {
        await SeedAsync(ct);
        await BuildMenu().RunAsync(ct);
    }

    private async Task SeedAsync(CancellationToken ct)
    {
        if ((await _books.AllAsync(ct)).Count > 0) return;
        await _books.AddAsync("CLR via C#", "Jeffrey Richter", 3, ct);
        await _books.AddAsync("Clean Code", "Robert Martin", 2, ct);
        await _members.AddAsync("Alice", "alice@example.com", ct);
        await _members.AddAsync("Bob", "bob@example.com", ct);
        _log.LogInformation("Демо-данные загружены");
    }

    private Menu BuildMenu()
    {
        var menu = new Menu(_console);

        menu.Add(new DelegateCommand("Список книг", async ct =>
        {
            foreach (var b in await _books.AllAsync(ct)) _console.WriteLine(b.ToString());
        }));

        menu.Add(new DelegateCommand("Поиск книг", async ct =>
        {
            _console.Write("Термин: ");
            var term = _console.ReadLine() ?? "";
            foreach (var b in await _books.SearchAsync(term, ct)) _console.WriteLine(b.ToString());
        }));

        menu.Add(new DelegateCommand("Добавить книгу", async ct =>
        {
            _console.Write("Название: "); var t = _console.ReadLine() ?? "";
            _console.Write("Автор: "); var a = _console.ReadLine() ?? "";
            var c = PromptInt("Копий: ");
            var book = await _books.AddAsync(t, a, c, ct);
            _console.WriteLine($"Добавлено: {book}");
            _log.LogInformation("Добавлена книга {Id}", book.Id);
        }));

        menu.Add(new DelegateCommand("Список читателей", async ct =>
        {
            foreach (var m in await _members.AllAsync(ct)) _console.WriteLine(m.ToString());
        }));

        menu.Add(new DelegateCommand("Добавить читателя", async ct =>
        {
            _console.Write("Имя: "); var n = _console.ReadLine() ?? "";
            _console.Write("Email: "); var e = _console.ReadLine() ?? "";
            var m = await _members.AddAsync(n, e, ct);
            _console.WriteLine($"Добавлено: {m}");
        }));

        menu.Add(new DelegateCommand("Выдать книгу", async ct =>
        {
            var bid = PromptInt("ID книги: ");
            var mid = PromptInt("ID читателя: ");
            var loan = await _loans.BorrowAsync(bid, mid, ct);
            _console.WriteLine($"Создана выдача #{loan.Id}");
            _log.LogInformation("Выдана книга {BookId} читателю {MemberId}", bid, mid);
        }));

        menu.Add(new DelegateCommand("Вернуть книгу", async ct =>
        {
            var lid = PromptInt("ID выдачи: ");
            await _loans.ReturnAsync(lid, ct);
            _console.WriteLine("Возвращено");
        }));

        menu.Add(new DelegateCommand("Активные выдачи", async ct =>
        {
            foreach (var l in await _loans.ActiveAsync(ct))
                _console.WriteLine($"#{l.Id}: книга {l.BookId} → читатель {l.MemberId} ({l.BorrowedAt:u})");
        }));

        return menu;
    }

    private int PromptInt(string prompt)
    {
        _console.Write(prompt);
        return int.TryParse(_console.ReadLine(), out var v)
            ? v
            : throw new InvalidOperationException("Ожидалось целое число");
    }
}