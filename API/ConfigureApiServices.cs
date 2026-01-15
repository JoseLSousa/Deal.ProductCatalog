using API.HealthChecks;
using Domain.Constants;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

namespace API
{
    public static class ConfigureApiServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Adicionar FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Adicionar Health Checks
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("postgres")
                .AddCheck<MongoDbHealthCheck>("mongodb");

            // Configurar Swagger com suporte a JWT
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Product Catalog API",
                    Version = "v1",
                    Description = "API REST para gerenciamento de catálogo de produtos com JWT, MongoDB e PostgreSQL",
                    Contact = new OpenApiContact
                    {
                        Name = "José Lucas Sousa Ferreira",
                        Url = new Uri("https://github.com/JoseLSousa")
                    }
                });

                // Adicionar definição de segurança JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira apenas o token JWT (sem 'Bearer')"
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
                        Array.Empty<string>()
                    }
                });
            });

            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey não configurada");

            Console.WriteLine($"[CONFIG] Issuer: {jwtSettings["Issuer"]}");
            Console.WriteLine($"[CONFIG] Audience: {jwtSettings["Audience"]}");
            Console.WriteLine($"[CONFIG] SecretKey: {secretKey.Substring(0, Math.Min(10, secretKey.Length))}...");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Policies.RequireAdminRole, policy =>
                    policy.RequireRole(Roles.Admin))
                .AddPolicy(Policies.RequireEditorRole, policy =>
                    policy.RequireRole(Roles.Editor))
                .AddPolicy(Policies.RequireViewerRole, policy =>
                    policy.RequireRole(Roles.Viewer))
                .AddPolicy(Policies.CanWrite, policy =>
                    policy.RequireRole(Roles.Admin, Roles.Editor))
                .AddPolicy(Policies.CanDelete, policy =>
                    policy.RequireRole(Roles.Admin));

            return services;
        }

        public static WebApplication ConfigureApiMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            // Middleware de tratamento global de exceções
            app.UseMiddleware<API.Middleware.GlobalExceptionHandlerMiddleware>();

            // Health Check endpoint
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.TotalMilliseconds
                        }),
                        totalDuration = report.TotalDuration.TotalMilliseconds
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
