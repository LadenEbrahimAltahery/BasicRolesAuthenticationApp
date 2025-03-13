
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RoleBasedBasicAuthentication.Data;
using RoleBasedBasicAuthentication.Handler;
using RoleBasedBasicAuthentication.Interfaces;
using RoleBasedBasicAuthentication.Repositories;
using RoleBasedBasicAuthentication.Services;
using RoleBasedBasicAuthenticationDemo.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add controller services and configure JSON options.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Disable automatic camel casing so property names remain as defined.
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            // Configure Entity Framework Core to use SQL Server.
            // We retrieve the connection string from appsettings.json (key: "EFCoreDBConnection").
            // Use SQL Server with the connection string from the configuration file.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("EfCoreDbConnection")));


            // Configure authentication to use our custom BasicAuthenticationHandler.
            // "BasicAuthentication" is the scheme name; the handler is specified as BasicAuthenticationHandler.
            builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            // Configure authorization and define role-based policies:
            //   - "AdminOnly": Requires the "Admin" role.
            //   - "UserOnly": Requires the "User" role.
            //   - "AdminOrUser": Requires either "Admin" or "User" role.
            //   - "AdminAndUser": Requires both "Admin" AND "User" roles (via a custom assertion).
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
                options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
                options.AddPolicy("AdminAndUser", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") && context.User.IsInRole("User")));
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            
            // Register Application Services
            builder.Services.AddScoped<IUserService, UserService>(); // Add user service
            builder.Services.AddScoped<IRoleService, RoleService>(); // Add role service
            builder.Services.AddScoped<IProductService, ProductService>(); // Add Product service

            // Register Application Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>(); // Add user repository
            builder.Services.AddScoped<IRoleRepository, RoleRepository>(); // Add role repository
            builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Add Product repository

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
