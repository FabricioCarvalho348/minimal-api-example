using Microsoft.EntityFrameworkCore;
using minimal_api.domain.DTOs;
using minimal_api.domain.entities;
using minimal_api.domain.interfaces;
using minimal_api.infra.Db;

namespace minimal_api.domain.services;

public class VeiculoService : IVeiculoService
{
    private readonly DatabaseContext _context;

    public VeiculoService(DatabaseContext context)
    {
        _context = context;
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _context.Veiculos.AsQueryable();
        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
        }

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

        }
        
        return query.ToList();
    }

    public Veiculo? BuscarPorId(int id)
    {
        return _context.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _context.Veiculos.Add(veiculo);
        _context.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo);
        _context.SaveChanges();
    }

    public void Excluir(Veiculo veiculo)
    {
        _context.Veiculos.Remove(veiculo);
        _context.SaveChanges();
    }
}