using LibraryApp.Data;
using LibraryApp.Domain;
namespace LibraryApp.Services;

public interface IMemberService
{
    Task<IReadOnlyList<Member>> AllAsync(CancellationToken ct = default);
    Task<Member> AddAsync(string name, string email, CancellationToken ct = default);
    Task<Member> GetAsync(int id, CancellationToken ct = default);
}