using minimal_api.domain.DTOs;
using minimal_api.domain.entities;

namespace minimal_api.domain.interfaces;

public interface IVeiculoService
{
    List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);
    
    Veiculo? ObterPorId(int id);
    
    void Incluir(Veiculo veiculo);
    
    void Atualizar(Veiculo veiculo);
    
    void Excluir(Veiculo veiculo);
}