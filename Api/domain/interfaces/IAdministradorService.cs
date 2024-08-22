using minimal_api.domain.DTOs;
using minimal_api.domain.entities;

namespace minimal_api.domain.interfaces;

public interface IAdministradorService
{
    Administrador? Login(LoginDto loginDto);
    
    Administrador Incluir(Administrador admin);
    List<Administrador> Todos(int? pagina);
    
    Administrador? BuscarPorId(int id);

}