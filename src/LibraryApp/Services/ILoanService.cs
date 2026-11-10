using LibraryApp.Data;
using LibraryApp.Domain;
namespace LibraryApp.Services;

public interface ILoanService
{
    Task<Loan> BorrowAsync(int bookId, int memberId, CancellationToken ct = default);
    Task ReturnAsync(int loanId, CancellationToken ct = default);
    Task<IReadOnlyList<Loan>> ActiveAsync(CancellationToken ct = default);
}