// Services/IBookService.cs
using LibraryApp.Domain;
namespace LibraryApp.Services;

public interface IBookService
{
    Task<IReadOnlyList<Book>> AllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Book>> SearchAsync(string term, CancellationToken ct = default);
    Task<Book> AddAsync(string title, string author, int copies, CancellationToken ct = default);
    Task<Book> GetAsync(int id, CancellationToken ct = default);
    Task UpdateAsync(Book book, CancellationToken ct = default);
}