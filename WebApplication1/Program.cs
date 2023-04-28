using ClassLibrary1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    var class1 = new Class1();
    return class1.Method1(true);
});

app.Run();
