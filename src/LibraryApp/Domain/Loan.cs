namespace LibraryApp.Domain;

public class Loan : Entity
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public bool IsActive => ReturnedAt is null;
}