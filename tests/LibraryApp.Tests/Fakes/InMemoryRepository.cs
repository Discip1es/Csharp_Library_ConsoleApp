using LibraryApp.Data;
using LibraryApp.Domain;

namespace LibraryApp.Tests.Fakes;

public class InMemoryRepository<T> : IRepository<T> where T : Entity
{
    private readonly List<T> _items = new();
    private int _nextId = 1;

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<T>>(_items.ToList());

    public Task<T?> GetAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        entity.Id = _nextId++;
        _items.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        var idx = _items.FindIndex(x => x.Id == entity.Id);
        if (idx < 0) throw new InvalidOperationException($"{typeof(T).Name} #{entity.Id} не найден");
        _items[idx] = entity;
        return Task.CompletedTask;
    }
}