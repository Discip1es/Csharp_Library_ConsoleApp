using LibraryApp;
using LibraryApp.Data;
using LibraryApp.Domain;
using LibraryApp.Services;
using LibraryApp.Ui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var builder = Host.CreateApplicationBuilder(args);

var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(dataDir);

builder.Services.AddSingleton<IRepository<Book>>(_ => new JsonRepository<Book>(Path.Combine(dataDir, "books.json")));
builder.Services.AddSingleton<IRepository<Member>>(_ => new JsonRepository<Member>(Path.Combine(dataDir, "members.json")));
builder.Services.AddSingleton<IRepository<Loan>>(_ => new JsonRepository<Loan>(Path.Combine(dataDir, "loans.json")));

builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddSingleton<IMemberService, MemberService>();
builder.Services.AddSingleton<ILoanService, LoanService>();

builder.Services.AddSingleton<IConsole, SystemConsole>();
builder.Services.AddSingleton<App>();

using var host = builder.Build();

try
{
    await host.Services.GetRequiredService<App>().RunAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nОтменено пользователем");
}