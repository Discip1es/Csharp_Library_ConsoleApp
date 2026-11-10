using LibraryApp.Data;
using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public class JsonRepositoryTests : IDisposable
{
    private readonly string _dir;
    private readonly string _file;

    public JsonRepositoryTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "libtests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        _file = Path.Combine(_dir, "books.json");
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true);
    }

    [Fact]
    public async Task AddAsync_AssignsSequentialIds()
    {
        var repo = new JsonRepository<Book>(_file);
        var a = await repo.AddAsync(new Book { Title = "A", Author = "x", TotalCopies = 1, AvailableCopies = 1 });
        var b = await repo.AddAsync(new Book { Title = "B", Author = "y", TotalCopies = 1, AvailableCopies = 1 });

        Assert.Equal(1, a.Id);
        Assert.Equal(2, b.Id);
    }

    [Fact]
    public async Task AddAsync_PersistsAcrossNewInstance()
    {
        var first = new JsonRepository<Book>(_file);
        await first.AddAsync(new Book { Title = "Persisted", Author = "z", TotalCopies = 2, AvailableCopies = 2 });

        var second = new JsonRepository<Book>(_file);
        var items = await second.GetAllAsync();

        Assert.Single(items);
        Assert.Equal("Persisted", items[0].Title);
    }

    [Fact]
    public async Task GetAsync_UnknownId_ReturnsNull()
    {
        var repo = new JsonRepository<Book>(_file);
        Assert.Null(await repo.GetAsync(42));
    }

    [Fact]
    public async Task UpdateAsync_ChangesField()
    {
        var repo = new JsonRepository<Book>(_file);
        var book = await repo.AddAsync(new Book { Title = "A", Author = "x", TotalCopies = 3, AvailableCopies = 3 });

        book.AvailableCopies = 1;
        await repo.UpdateAsync(book);

        var fresh = new JsonRepository<Book>(_file);
        var loaded = await fresh.GetAsync(book.Id);
        Assert.NotNull(loaded);
        Assert.Equal(1, loaded!.AvailableCopies);
    }

    [Fact]
    public async Task UpdateAsync_Unknown_Throws()
    {
        var repo = new JsonRepository<Book>(_file);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.UpdateAsync(new Book { Id = 999, Title = "x", Author = "y" }));
    }

    [Fact]
    public async Task Concurrent_Adds_AllPersisted()
    {
        var repo = new JsonRepository<Book>(_file);

        await Task.WhenAll(Enumerable.Range(1, 20).Select(i =>
            repo.AddAsync(new Book { Title = $"B{i}", Author = "a", TotalCopies = 1, AvailableCopies = 1 })));

        var items = await repo.GetAllAsync();
        Assert.Equal(20, items.Count);
        Assert.Equal(20, items.Select(b => b.Id).Distinct().Count());
    }
}