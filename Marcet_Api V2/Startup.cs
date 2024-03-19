using FluentValidation;
using Marcet_Api_V2.Models;
using Marcet_Api_V2.Repository;
using Microsoft.EntityFrameworkCore;
using Marcet_Api.Repository;
using Marcet_Api.Services;
using Marcet_Api.Services.IServices;
using Marcet_Api.Services.IServices.ITokens;
using Marcet_Api.Validators;
using System.Text.Json.Serialization;
using Repository.IRepository;
using Services.IServices;
using Models.Dto.Token;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Marcet_Api_V2.Services.IServices;
using Marcet_Api_V2.Services;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<MarcetDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultSqlConnection"));
        });

        services.AddScoped<IRepository<Customer>, Repository<Customer>>();
        services.AddScoped<IRepository<Product>, Repository<Product>>();

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAccessTokenService, TokenService>();
        services.AddScoped<IRefreshTokenService, TokenService>();
        services.AddScoped<IHashService, HashService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();


        services.AddScoped<IRepository<RefreshToken>, Repository<RefreshToken>>();

        services.AddDistributedMemoryCache();

        services.AddScoped<IValidator<TokensDTO>, TokensDTOValidator>();

        services.AddAutoMapper(typeof(Startup));

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        })
         .AddNewtonsoftJson();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("User", policy => policy.RequireRole("User"));
        });

        // Добавление аутентификации с использованием JWT
        var publicKey = Configuration.GetValue<string>("JWT:PublicKey");
        var issuer = Configuration.GetValue<string>("JWT:Issuer");

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            RSA rsa = RSA.Create();
            rsa.FromXmlString(publicKey);

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = issuer,
                ValidateLifetime = true,
                ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // если запрос направлен хабу
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                    {
                        // получаем токен из строки запроса
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");

                c.OAuthClientId("swagger-ui");
                c.OAuthClientSecret("swagger-ui-secret");
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}


