using minimal_api.domain.DTOs;
using minimal_api.domain.entities;
using minimal_api.domain.interfaces;

namespace Test.Mocks;

public class AdministradorServiceMock : IAdministradorService
{
    private static List<Administrador> _administradores = new List<Administrador>()
    {
        new Administrador
        {
            Id = 1,
            Email = "adm@test.com",
            Password = "123456",
            Profile = "Adm"
        },        
        new Administrador
        {
            Id = 2,
            Email = "editor@test.com",
            Password = "123456",
            Profile = "Editor"
        }
    };

    public Administrador? Login(LoginDto loginDto)
    {
        return _administradores.Find(a => a.Email == loginDto.Email && a.Password == loginDto.Password);
    }

    public Administrador Incluir(Administrador administrador)
    {
        administrador.Id = _administradores.Count() + 1;
        _administradores.Add(administrador);

        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        return _administradores;
    }

    public Administrador? BuscarPorId(int id)
    {
        return _administradores.Find(a => a.Id == id);
    }
}