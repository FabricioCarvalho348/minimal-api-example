using minimal_api.domain.Enums;

namespace minimal_api.domain.ModelViews;

public record AdministardorModelView
{
   public int Id { get; set; } = default!;
   public string Email { get; set; } = default!;
   public string Profile { get; set; } = default!;

}