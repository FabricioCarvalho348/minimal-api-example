namespace minimal_api.domain.DTOs;

public record VeiculoDto
{
    public string Nome { get; set; } = default!;
    public string Marca { get; set; } = default!;
    public string Ano { get; set; } = default!;
}