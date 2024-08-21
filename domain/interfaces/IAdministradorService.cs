using minimal_api.domain.DTOs;
using minimal_api.domain.entities;

namespace minimal_api.domain.interfaces;

public interface IAdministradorService
{
    Administrador? Login(LoginDto loginDto);
}