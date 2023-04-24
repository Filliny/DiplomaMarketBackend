
using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Seeder;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using DiplomaMarketBackend.Services;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using System.Reflection;
using System.Text;
using WebShopApp.Abstract;

namespace DiplomaMarketBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<EmailSettings>
                                (options => builder.Configuration.GetSection("EmailSettings").Bind(options));

            string currConnectionString = "DMarketNpgsql";

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                currConnectionString = "DMarketNpgsqlLocal";
            }

            var connectionString = builder.Configuration.GetConnectionString(currConnectionString) ?? throw new InvalidOperationException("Connection string  not found.");

            // Add services to the container.
            builder.Services.AddDbContext<BaseContext>(options =>
               options.UseNpgsql(connectionString));//, new MySqlServerVersion(new Version(8, 0, 32))));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerHandler);

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddSingleton<IFileService, FileService>();

            builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
            builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();

            builder.Services.AddSingleton<IDeliveryCasher, DeliveryCasher>();
            //or start as service 
            builder.Services.AddHostedService<DeliveryCasher>();

            builder.Services.AddResponseCaching();


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

                await DbInitializer.Initialize(services);
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
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

            app.UseCors();//CORS with default policy

            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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