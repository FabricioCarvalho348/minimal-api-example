using System.Net;
using System.Text;
using System.Text.Json;
using minimal_api.domain.DTOs;
using minimal_api.domain.ModelViews;
using Test.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Test.Request;

[TestClass]
public class AdministradorRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }
    
    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }
    
    [TestMethod]
    public async Task TestTestarRequests()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "adm@test.com",
            Password = "123456"
        };
        
        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.Client.PostAsync("administradores/login", content);

        // Assert
        Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        
        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.IsNotNull(admLogado?.Email);
        Assert.IsNotNull(admLogado.Profile);
        Assert.IsNotNull(admLogado.Token);
    }
    
}