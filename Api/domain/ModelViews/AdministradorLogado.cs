namespace minimal_api.domain.ModelViews;

public record AdministradorLogado
{
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Profile { get; set; } = default!;
    public string Token { get; set; } = default!;
}