using minimal_api.domain.Enums;

namespace minimal_api.domain.DTOs;

public class AdministradorDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Profile? Profile { get; set; } = default!;
}