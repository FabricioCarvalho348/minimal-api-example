using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.domain.entities;
using minimal_api.domain.services;
using minimal_api.infra.Db;

namespace Test.Domain.Services;

[TestClass]
public class AdministradorServiceTest
{
    private DatabaseContext CreateContextTest()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DatabaseContext(configuration);
    }

    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        // Arrange (todas as variaveis que devemos criar para fazer a validação)
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        
        var adm = new Administrador();
        adm.Email = "test@test.com";
        adm.Password = "123456";
        adm.Profile = "Adm";

        var administradorService = new AdministradorService(context);
        // Act (Ação que vamos executar)
        administradorService.Incluir(adm);
        
        
        // Assert (validação dos dados)
        Assert.AreEqual(1, administradorService.Todos(1).Count());
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("123456", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
    
    [TestMethod]
    public void TestandoBuscaPorIdAdministrador()
    {
        // Arrange (todas as variaveis que devemos criar para fazer a validação)
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        
        var adm = new Administrador();
        adm.Email = "test@test.com";
        adm.Password = "123456";
        adm.Profile = "Adm";

        var administradorService = new AdministradorService(context);
        // Act (Ação que vamos executar)
        administradorService.Incluir(adm);
        var admDatabase = administradorService.BuscarPorId(adm.Id);
        
        
        // Assert (validação dos dados)
        Assert.AreEqual(1, admDatabase.Id);
    }

    [TestMethod]
    public void TestarGetSetProperties()
    {
        // Arrange (todas as variaveis que devemos criar para fazer a validação)
        var adm = new Administrador();

        // Act (Ação que vamos executar)
        adm.Id = 1;
        adm.Email = "test@test.com";
        adm.Password = "teste";
        adm.Profile = "Adm";

        // Assert (validação dos dados)
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("teste", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
}