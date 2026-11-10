using LibraryApp.Domain;
using LibraryApp.Services;
using LibraryApp.Tests.Fakes;
using Xunit;

namespace LibraryApp.Tests;

public class LoanServiceTests
{
    private sealed record Fixture(
        BookService Books, MemberService Members, LoanService Loans);

    private static async Task<Fixture> SetupAsync()
    {
        var books = new BookService(new InMemoryRepository<Book>());
        var members = new MemberService(new InMemoryRepository<Member>());
        var loans = new LoanService(new InMemoryRepository<Loan>(), books, members);

        await books.AddAsync("CLR", "Richter", 1);
        await members.AddAsync("Alice", "a@x");
        return new Fixture(books, members, loans);
    }

    [Fact]
    public async Task BorrowAsync_DecrementsAvailableCopies()
    {
        var f = await SetupAsync();

        await f.Loans.BorrowAsync(bookId: 1, memberId: 1);

        var book = await f.Books.GetAsync(1);
        Assert.Equal(0, book.AvailableCopies);
    }

    [Fact]
    public async Task BorrowAsync_NoCopies_Throws()
    {
        var f = await SetupAsync();
        await f.Loans.BorrowAsync(1, 1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => f.Loans.BorrowAsync(1, 1));
        Assert.Contains("Нет свободных", ex.Message);
    }

    [Fact]
    public async Task BorrowAsync_UnknownBook_Throws()
    {
        var f = await SetupAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => f.Loans.BorrowAsync(99, 1));
    }

    [Fact]
    public async Task BorrowAsync_UnknownMember_Throws()
    {
        var f = await SetupAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => f.Loans.BorrowAsync(1, 99));
    }

    [Fact]
    public async Task ReturnAsync_IncrementsAvailableCopies()
    {
        var f = await SetupAsync();
        var loan = await f.Loans.BorrowAsync(1, 1);

        await f.Loans.ReturnAsync(loan.Id);

        var book = await f.Books.GetAsync(1);
        Assert.Equal(1, book.AvailableCopies);
    }

    [Fact]
    public async Task ReturnAsync_Twice_Throws()
    {
        var f = await SetupAsync();
        var loan = await f.Loans.BorrowAsync(1, 1);
        await f.Loans.ReturnAsync(loan.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => f.Loans.ReturnAsync(loan.Id));
    }

    [Fact]
    public async Task ActiveAsync_ExcludesReturned()
    {
        var f = await SetupAsync();
        await f.Books.AddAsync("Clean Code", "Martin", 5);
        var l1 = await f.Loans.BorrowAsync(1, 1);
        await f.Loans.BorrowAsync(2, 1);
        await f.Loans.ReturnAsync(l1.Id);

        var active = await f.Loans.ActiveAsync();
        Assert.Single(active);
        Assert.Equal(2, active[0].BookId);
    }
}