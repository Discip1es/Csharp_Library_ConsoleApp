namespace LibraryApp.Domain;

public class Member : Entity
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public override string ToString() => $"[{Id}] {Name} <{Email}>";
}