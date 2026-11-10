// Data/JsonRepository.cs
using System.Text.Json;
using LibraryApp.Domain;
namespace LibraryApp.Data;

public class JsonRepository<T> : IRepository<T> where T : Entity
{
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };
    private readonly string _file;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private List<T>? _cache;

    public JsonRepository(string file) => _file = file;

    private async Task<List<T>> EnsureLoadedAsync(CancellationToken ct)
    {
        if (_cache is not null) return _cache;
        if (!File.Exists(_file)) return _cache = new();
        await using var fs = File.OpenRead(_file);
        return _cache = await JsonSerializer.DeserializeAsync<List<T>>(fs, Opts, ct) ?? new();
    }

    private async Task PersistAsync(CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(_cache, Opts);
        var tmp = _file + ".tmp";
        await File.WriteAllTextAsync(tmp, json, ct);
        File.Move(tmp, _file, overwrite: true);   // атомарная замена
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try { return (await EnsureLoadedAsync(ct)).ToList(); }
        finally { _gate.Release(); }
    }

    public async Task<T?> GetAsync(int id, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try { return (await EnsureLoadedAsync(ct)).FirstOrDefault(x => x.Id == id); }
        finally { _gate.Release(); }
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var items = await EnsureLoadedAsync(ct);
            entity.Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1;
            items.Add(entity);
            await PersistAsync(ct);
            return entity;
        }
        finally { _gate.Release(); }
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var items = await EnsureLoadedAsync(ct);
            var idx = items.FindIndex(x => x.Id == entity.Id);
            if (idx < 0) throw new InvalidOperationException($"{typeof(T).Name} #{entity.Id} не найден");
            items[idx] = entity;
            await PersistAsync(ct);
        }
        finally { _gate.Release(); }
    }
}