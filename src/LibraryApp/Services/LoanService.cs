// Services/ILoanService.cs + LoanService.cs
using LibraryApp.Data;
using LibraryApp.Domain;
namespace LibraryApp.Services;

public class LoanService : ILoanService
{
    private readonly IRepository<Loan> _loans;
    private readonly IBookService _books;
    private readonly IMemberService _members;

    public LoanService(IRepository<Loan> loans, IBookService books, IMemberService members)
    {
        _loans = loans; _books = books; _members = members;
    }

    public async Task<Loan> BorrowAsync(int bookId, int memberId, CancellationToken ct = default)
    {
        var book = await _books.GetAsync(bookId, ct);
        await _members.GetAsync(memberId, ct);
        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException("Нет свободных копий");

        book.AvailableCopies--;
        await _books.UpdateAsync(book, ct);

        return await _loans.AddAsync(new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            BorrowedAt = DateTime.UtcNow
        }, ct);
    }

    public async Task ReturnAsync(int loanId, CancellationToken ct = default)
    {
        var loan = await _loans.GetAsync(loanId, ct)
                   ?? throw new InvalidOperationException($"Выдача #{loanId} не найдена");
        if (!loan.IsActive)
            throw new InvalidOperationException("Книга уже возвращена");

        loan.ReturnedAt = DateTime.UtcNow;
        await _loans.UpdateAsync(loan, ct);

        var book = await _books.GetAsync(loan.BookId, ct);
        book.AvailableCopies = Math.Min(book.TotalCopies, book.AvailableCopies + 1);
        await _books.UpdateAsync(book, ct);
    }

    public async Task<IReadOnlyList<Loan>> ActiveAsync(CancellationToken ct = default) =>
        (await _loans.GetAllAsync(ct)).Where(l => l.IsActive).ToList();
}