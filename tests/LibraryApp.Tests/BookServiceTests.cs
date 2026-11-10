using LibraryApp.Services;
using LibraryApp.Tests.Fakes;
using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public class BookServiceTests
{
    private static BookService Create(out InMemoryRepository<Book> repo)
    {
        repo = new InMemoryRepository<Book>();
        return new BookService(repo);
    }

    [Fact]
    public async Task AddAsync_ValidInput_AddsBookWithAllCopiesAvailable()
    {
        var svc = Create(out _);

        var book = await svc.AddAsync("CLR via C#", "Richter", 3);

        Assert.Equal(1, book.Id);
        Assert.Equal(3, book.TotalCopies);
        Assert.Equal(3, book.AvailableCopies);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task AddAsync_EmptyTitle_Throws(string? title)
    {
        var svc = Create(out _);
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AddAsync(title!, "A", 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task AddAsync_NonPositiveCopies_Throws(int copies)
    {
        var svc = Create(out _);
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AddAsync("T", "A", copies));
    }

    [Fact]
    public async Task SearchAsync_FindsByTitleOrAuthor_CaseInsensitive()
    {
        var svc = Create(out _);
        await svc.AddAsync("Clean Code", "Robert Martin", 2);
        await svc.AddAsync("CLR via C#", "Jeffrey Richter", 1);

        Assert.Single(await svc.SearchAsync("clean"));
        Assert.Single(await svc.SearchAsync("RICHTER"));
        Assert.Equal(2, (await svc.SearchAsync("")).Count);
    }

    [Fact]
    public async Task GetAsync_UnknownId_Throws()
    {
        var svc = Create(out _);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.GetAsync(42));
    }
}