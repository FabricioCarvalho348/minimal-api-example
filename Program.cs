using minimal_api.domain.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDto loginDto) => {
    if ((loginDto.Email == "adm@teste.com") && (loginDto.Password == "123456"))
        return Results.Ok("Login com sucesso");
    
    return Results.Unauthorized();
});

app.Run();