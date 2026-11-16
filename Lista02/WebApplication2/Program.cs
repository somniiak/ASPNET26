var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/number/{num:int}", (int num) => $"number: {num}");

app.MapGet("/message/{text}", (string text) => $"message: {text}");

app.MapGet("/short_message/{text:maxlength(10)}", (string text) => $"short_message: {text}");

app.MapGet("/match/{text:regex(^\\d\\d\\d$)}", (string text) => $"matched string: {text}");

app.MapGet("/trouble/{id:int}", (int id) => $"trouble: id = {id}").WithOrder(0);

app.MapGet("/trouble/{num:int}", (int num) => $"trouble: num = {num}").WithOrder(1);

app.Run();
