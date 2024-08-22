using minimal_api.domain.DTOs;
using minimal_api.domain.entities;
using minimal_api.domain.interfaces;
using minimal_api.infra.Db;

namespace minimal_api.domain.services;

public class AdministradorService : IAdministradorService
{
    private readonly DatabaseContext _context;

    public AdministradorService(DatabaseContext context)
    {
        _context = context;
    }

    public Administrador? Login(LoginDto loginDto)
    {
        var adm = _context.Administradores.Where(a => a.Email == loginDto.Email && a.Password == loginDto.Password).FirstOrDefault();
        return adm;
    }
}