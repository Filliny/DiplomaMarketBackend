
using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using DiplomaMarketBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //string currConnectionString = "LocalDMarketMySQL";
            string currConnectionString = "LocalDMarketNpgsql";

            if (Environment.GetEnvironmentVariable("LOCAL_DB") == null)
            {
                currConnectionString = "LocalDMarketNpgsql";
            }

            var connectionString = builder.Configuration.GetConnectionString(currConnectionString) ?? throw new InvalidOperationException("Connection string  not found.");

            // Add services to the container.
            builder.Services.AddDbContext<BaseContext>(options =>
               options.UseNpgsql(connectionString));//, new MySqlServerVersion(new Version(8, 0, 32))));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerHandler);
            builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
            builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();


            //Allow CORS default policy
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                                 
                });
            });


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<BaseContext>();
                context.Database.Migrate();
                // requires using Microsoft.Extensions.Configuration;
                // Set password with the Secret Manager tool.
                // dotnet user-secrets set SeedUserPW <pw> //Not working for me - leave here for future

                var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");

                //await DbInitializer.Initialize(services);
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "files")),
                RequestPath = "/files",
                EnableDefaultFiles = true
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();//CORS with default policy

            app.MapControllers();

            app.Run();
        }

        private static void JwtBearerHandler(JwtBearerOptions option)
        {
            option.RequireHttpsMetadata = false;

            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtOptionConstant.Issuer,
                ValidAudience = JwtOptionConstant.Audience,
                IssuerSigningKey = JwtOptionConstant.GetSymmetric()
            };
        }
    }
}