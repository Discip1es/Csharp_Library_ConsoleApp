using LibraryApp.Data;
using LibraryApp.Domain;
namespace LibraryApp.Services;

public class MemberService : IMemberService
{
    private readonly IRepository<Member> _repo;
    public MemberService(IRepository<Member> repo) => _repo = repo;

    public Task<IReadOnlyList<Member>> AllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);

    public Task<Member> AddAsync(string name, string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя обязательно", nameof(name));

        return _repo.AddAsync(new Member { Name = name.Trim(), Email = email.Trim() }, ct);
    }

    public async Task<Member> GetAsync(int id, CancellationToken ct = default) =>
        await _repo.GetAsync(id, ct) ?? throw new InvalidOperationException($"Читатель #{id} не найден");
}