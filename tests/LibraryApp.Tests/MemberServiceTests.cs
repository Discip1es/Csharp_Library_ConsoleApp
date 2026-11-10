using LibraryApp.Services;
using LibraryApp.Tests.Fakes;
using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public class MemberServiceTests
{
    [Fact]
    public async Task AddAsync_ValidInput_TrimsAndAdds()
    {
        var svc = new MemberService(new InMemoryRepository<Member>());

        var m = await svc.AddAsync("  Alice  ", "  alice@x.io  ");

        Assert.Equal("Alice", m.Name);
        Assert.Equal("alice@x.io", m.Email);
        Assert.Equal(1, m.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddAsync_EmptyName_Throws(string name)
    {
        var svc = new MemberService(new InMemoryRepository<Member>());
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AddAsync(name, "e"));
    }

    [Fact]
    public async Task GetAsync_UnknownId_Throws()
    {
        var svc = new MemberService(new InMemoryRepository<Member>());
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.GetAsync(7));
    }
}