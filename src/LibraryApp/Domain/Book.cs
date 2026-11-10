namespace LibraryApp.Domain;

public class Book : Entity
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public override string ToString() => $"[{Id}] {Title} — {Author} ({AvailableCopies}/{TotalCopies})";
}