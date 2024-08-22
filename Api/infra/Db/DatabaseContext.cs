using Microsoft.EntityFrameworkCore;
using minimal_api.domain.entities;

namespace minimal_api.infra.Db;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _connectionAppSettings;

    public DatabaseContext(IConfiguration connectionAppSettings) 
    {
        _connectionAppSettings = connectionAppSettings;
    }
    
    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "admin@gmail.com",
                Password = "123456",
                Profile = "Adm"
            }
        );
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _connectionAppSettings.GetConnectionString("MySql");
            if (!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(stringConexao,
                    ServerVersion.AutoDetect(stringConexao)
                );
            }
        }
    }
}