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
        var adm = _context.Administradores.Where(a => a.Email == loginDto.Email && a.Password == loginDto.Password)
            .FirstOrDefault();
        return adm;
    }

    public Administrador Incluir(Administrador admin)
    {
        _context.Administradores.Add(admin);
        _context.SaveChanges();

        return admin;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _context.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }

        return query.ToList();
    }

    public Administrador? BuscarPorId(int id)
    {
        return _context.Administradores.Where(a => a.Id == id).FirstOrDefault();
    }
}