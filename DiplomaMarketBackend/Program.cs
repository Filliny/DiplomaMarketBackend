
using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Seeder;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using DiplomaMarketBackend.Services;
using Google;
using Lessons3.Entity.Models;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using System.Reflection;
using System.Text;
using WebShopApp.Abstract;
using AutoMapper;
using DiplomaMarketBackend.Mappings;
using Microsoft.AspNetCore.Hosting;

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
            var varb = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                currConnectionString = "DMarketNpgsqlLocal";
            }else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing")
            {
                currConnectionString = "DMarketNpgsqlTesting";
            }


            var connectionString = builder.Configuration.GetConnectionString(currConnectionString) ?? throw new InvalidOperationException("Connection string  not found.");

            // Add services to the container.
            builder.Services.AddDbContext<BaseContext>(options =>
               options.UseNpgsql(connectionString));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            // For Identity
            builder.Services.AddIdentity<UserModel, IdentityRole>()
                .AddEntityFrameworkStores<BaseContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            });
           
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerHandler);

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddSingleton<IFileService, FileService>();

            builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
            builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();

            builder.Services.AddSingleton<IDeliveryCasher, DeliveryCasher>();
            //or start as service 
            builder.Services.AddHostedService<DeliveryCasher>();

            builder.Services.AddResponseCaching();

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                //config.AddProfile(new AssemblyMappingProfile(typeof(INotesDbContext).Assembly));
                
            });

            builder.Services.AddTransient(typeof(SetImgFullURL));


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
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction()||app.Environment.EnvironmentName.Equals("Testing"))
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