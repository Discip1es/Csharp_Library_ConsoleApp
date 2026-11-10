// Services/BookService.cs
using LibraryApp.Data;
using LibraryApp.Domain;
namespace LibraryApp.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _repo;
    public BookService(IRepository<Book> repo) => _repo = repo;

    public Task<IReadOnlyList<Book>> AllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);

    public async Task<IReadOnlyList<Book>> SearchAsync(string term, CancellationToken ct = default) =>
        (await _repo.GetAllAsync(ct))
            .Where(b => b.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                     || b.Author.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public Task<Book> AddAsync(string title, string author, int copies, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название обязательно", nameof(title));
        if (copies < 1)
            throw new ArgumentException("Количество копий должно быть >= 1", nameof(copies));

        return _repo.AddAsync(new Book
        {
            Title = title.Trim(),
            Author = author.Trim(),
            TotalCopies = copies,
            AvailableCopies = copies
        }, ct);
    }

    public async Task<Book> GetAsync(int id, CancellationToken ct = default) =>
        await _repo.GetAsync(id, ct) ?? throw new InvalidOperationException($"Книга #{id} не найдена");

    public Task UpdateAsync(Book book, CancellationToken ct = default) => _repo.UpdateAsync(book, ct);
}