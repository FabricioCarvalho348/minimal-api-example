using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.domain.DTOs;
using minimal_api.domain.entities;
using minimal_api.domain.Enums;
using minimal_api.domain.interfaces;
using minimal_api.domain.ModelViews;
using minimal_api.domain.services;
using minimal_api.infra.Db;

namespace minimal_api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        _key = Configuration.GetSection("Jwt").ToString();
    }

    private string? _key;

    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorService, AdministradorService>();
        services.AddScoped<IVeiculoService, VeiculoService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Input your Bearer token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            #region Home

            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

            #endregion

            #region Administrador

            string GerarTokenJwt(Administrador administrador)
            {
                if (string.IsNullOrEmpty(_key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Profile", administrador.Profile),
                    new Claim(ClaimTypes.Role, administrador.Profile),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("administradores/login",
                ([FromBody] LoginDto loginDto, IAdministradorService administradorService) =>
                {
                    var adm = administradorService.Login(loginDto);
                    if (adm != null)
                    {
                        string token = GerarTokenJwt(adm);
                        return Results.Ok(new AdministradorLogado
                        {
                            Email = adm.Email,
                            Profile = adm.Profile,
                            Token = token
                        });
                    }

                    return Results.Unauthorized();
                }).AllowAnonymous().WithTags("Administradores");

            endpoints.MapGet("/administradores",
                    ([FromQuery] int? pagina, IAdministradorService administradorService) =>
                    {
                        var adms = new List<AdministardorModelView>();
                        var administradores = administradorService.Todos(pagina);

                        foreach (var adm in administradores)
                        {
                            adms.Add(new AdministardorModelView
                            {
                                Id = adm.Id,
                                Email = adm.Email,
                                Profile = adm.Profile
                            });
                        }

                        return Results.Ok(adms);
                    }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administradores");

            endpoints.MapGet("/administradores/{id}",
                    ([FromRoute] int id, IAdministradorService administradorService) =>
                    {
                        var administrador = administradorService.BuscarPorId(id);

                        if (administrador == null)
                        {
                            return Results.NotFound();
                        }

                        return Results.Ok(administrador);
                    }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administradores");


            endpoints.MapPost("/administradores",
                    ([FromBody] AdministradorDto administradorDto, IAdministradorService administradorService) =>
                    {
                        var validation = new ValidationsErrors
                        {
                            Mensagens = new List<string>()
                        };

                        if (string.IsNullOrEmpty(administradorDto.Email))
                        {
                            validation.Mensagens.Add("Informe o email do administrador!");
                        }

                        if (string.IsNullOrEmpty(administradorDto.Password))
                        {
                            validation.Mensagens.Add("Informe a senha do administrador!");
                        }

                        if (administradorDto.Profile == null)
                        {
                            validation.Mensagens.Add("o perfil do administrador não pode ser nulo ou vazio.");
                        }

                        if (validation.Mensagens.Count > 0)
                        {
                            return Results.BadRequest(validation);
                        }

                        var administrador = new Administrador
                        {
                            Email = administradorDto.Email,
                            Password = administradorDto.Password,
                            Profile = administradorDto.Profile.ToString() ?? Profile.Editor.ToString()
                        };

                        administradorService.Incluir(administrador);

                        return Results.Created($"/administradores/{administrador.Id}", administrador);
                    }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Administradores");

            #endregion

            #region Veiculo

            ValidationsErrors ValidationsDto(VeiculoDto veiculoDto)
            {
                var validation = new ValidationsErrors
                {
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrEmpty(veiculoDto.Nome))
                {
                    validation.Mensagens.Add("O nome não pode ser vazio ou nulo.");
                }

                if (string.IsNullOrEmpty(veiculoDto.Marca))
                {
                    validation.Mensagens.Add("A marca não pode ser vazia ou nula.");
                }

                if (string.IsNullOrEmpty(veiculoDto.Ano))
                {
                    validation.Mensagens.Add("O Ano do veiculo não pode ser vazio ou nulo.");
                }

                return validation;
            }

            endpoints.MapPost("/veiculos", ([FromBody] VeiculoDto veiculoDto, IVeiculoService veiculoService) =>
                {
                    var validation = ValidationsDto(veiculoDto);
                    if (validation.Mensagens.Count > 0)
                    {
                        return Results.BadRequest(validation);
                    }

                    var veiculo = new Veiculo
                    {
                        Nome = veiculoDto.Nome,
                        Marca = veiculoDto.Marca,
                        Ano = veiculoDto.Ano
                    };
                    veiculoService.Incluir(veiculo);

                    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                .WithTags("Veiculos");

            endpoints.MapGet("/veiculos/", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
                {
                    var veiculos = veiculoService.Todos(pagina);

                    return Results.Ok(veiculos);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                .WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
                {
                    var veiculo = veiculoService.BuscarPorId(id);

                    if (veiculo == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(veiculo);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                .WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}",
                    ([FromRoute] int id, VeiculoDto veiculoDto, IVeiculoService veiculoService) =>
                    {
                        var veiculo = veiculoService.BuscarPorId(id);
                        if (veiculo == null)
                        {
                            return Results.NotFound();
                        }

                        var validation = ValidationsDto(veiculoDto);
                        if (validation.Mensagens.Count > 0)
                        {
                            return Results.BadRequest(validation);
                        }

                        veiculo.Nome = veiculoDto.Nome;
                        veiculo.Marca = veiculoDto.Marca;
                        veiculo.Ano = veiculoDto.Ano;

                        veiculoService.Atualizar(veiculo);

                        return Results.Ok(veiculo);
                    }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
                {
                    var veiculo = veiculoService.BuscarPorId(id);

                    if (veiculo == null)
                    {
                        return Results.NotFound();
                    }

                    veiculoService.Excluir(veiculo);

                    return Results.NoContent();
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Veiculos");
        });

        #endregion
    }
}