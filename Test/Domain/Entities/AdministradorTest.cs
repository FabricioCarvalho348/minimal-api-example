using minimal_api.domain.entities;

namespace Test.Domain;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetProperties()
    {
        // Arrange (todas as variaveis que devemos criar para fazer a validação)
        var adm = new Administrador();
        
        // Act (Ação que vamos executar)
        adm.Id = 1;
        adm.Email = "test@test.com";
        adm.Password = "123456";
        adm.Profile = "Adm";
        // Assert (validação dos dados)
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("teste", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
}