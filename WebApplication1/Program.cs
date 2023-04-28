using ClassLibrary1;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", (bool input) =>
{
    var class1 = new Class1();
    return class1.Method1(input);
});

app.Run();
