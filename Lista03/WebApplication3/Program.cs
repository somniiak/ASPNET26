using Microsoft.AspNetCore.DataProtection;
using System.Xml.Linq;
using WebApplication3;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Lista 3");

// Zadanie 1

app.MapGet("/zadanie1", () => Results.Content(
    File.ReadAllText("wwwroot/zadanie1.html", System.Text.Encoding.UTF8),
    "text/html; charset=utf-8"));

app.MapGet("/zadanie1/wynik", (HttpRequest r) =>
    Results.Text($"Żądanie {r.Method}: p1={r.Query["p1"]}, p2={r.Query["p2"]}",
                 "text/plain; charset=utf-8"));

app.MapPost("/zadanie1/wynik", (HttpRequest r) =>
    Results.Text($"Żądanie {r.Method}: p1={r.Form["p1"]}, p2={r.Form["p2"]}",
                 "text/plain; charset=utf-8"));

// Zadanie 2

app.MapGet("/getcookie", (HttpContext context) =>
    Results.Text(context.Request.Cookies["mycookie"]));

app.MapGet("/setcookie", (HttpContext context) => {
    context.Response.Cookies.Append("mycookie", "Moje ciastko!", new CookieOptions {
        Expires = DateTimeOffset.UtcNow.AddMinutes(10),
    });

    return Results.Text("Ciastko ustawione", "text/plain; charset=utf-8");
});

app.MapGet("/delcookie", (HttpContext context) => {
    context.Response.Cookies.Delete("mycookie");
    return Results.Text("Ciastko usunięte", "text/plain; charset=utf-8");
});

// Zadanie 3

var byterev = new ByteRevDataProtector();

app.MapGet("/setencryptedcookie", (HttpContext context) => {
    var protector = byterev.CreateProtector("ciastka");

    string originalValue = "sigma_essa67";
    string encryptedValue = protector.Protect(originalValue);

    context.Response.Cookies.Append("EncCookie", encryptedValue);
    return Results.Text("Dodano zaszyfrowane ciasteczko!", "text/plain; charset=utf-8");
});

app.MapGet("/getencryptedcookie", (HttpContext context) => {
    var encryptedValue = context.Request.Cookies["EncCookie"];

    if (encryptedValue is null)
        return Results.Text("Brak ciastka", "text/plain; charset=utf-8");

    var originalValue = byterev.Unprotect(encryptedValue);

    return Results.Text(
        $"Encrypted: {encryptedValue}\n" +
        $"Decrypted: {originalValue}",
        "text/plain; charset=utf-8");
});

// Zadanie 4

// Host.CreateEmptyApplicationBuilder
// WebApplication.CreateBuilder

var empty = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
Console.WriteLine($"Empty: {empty.Services.Count}");
Console.WriteLine($"Web: {builder.Services.Count}");

// Zadanie 5

// Polityki czasu życia:
// * AddSingleton - do usług o jednym wystąpieniu per cała aplikacja
// * AddScoped - do usług żyjących tyle ile pojedyncze żądanie (w każdym żądaniu mamy inną instancję obiektu)
// * AddTransient - do usług które za każdym razem tworzą nową instancję obiektu usługi

// Zadanie 6

app.MapGet("/uploadfile", () => Results.Content(
    File.ReadAllText("wwwroot/zadanie6.html", System.Text.Encoding.UTF8),
    "text/html; charset=utf-8"));

app.MapPost("/uploadfile", async (HttpContext context) => {
    if (!context.Request.HasFormContentType)
        return Results.BadRequest("Oczekiwano formularza multipart/form-data.");

    var file = context.Request.Form.Files["myfile"];

    if (file is null)
        return Results.BadRequest("Oczekiwano przesłania pliku.");

    using var fileStream = file.OpenReadStream();
    using var memoryStream = new MemoryStream();
    await fileStream.CopyToAsync(memoryStream);

    var sum = memoryStream.ToArray().Sum(b => (long) b) % 0xFFFF;

    var xml = new XDocument(
        new XElement("opis",
            new XElement("nazwa", file.FileName),
            new XElement("rozmiar", file.Length),
            new XElement("sygnatura", sum)));

    var output = new MemoryStream();
    xml.Save(output);
    output.Position = 0;

    context.Response.Headers.ContentDisposition =
        "inline; filename=\"informacja_zwrotna.txt\"";

    return Results.File(output, "application/xml; charset=utf-8");
});

app.Run();
