using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using N_Shop.API.Data;
using N_Shop.API.Migrations;
using N_Shop.API.Models;
using N_Shop.API.Services;
using N_Shop.API.Utility;
using N_Shop.API.Utility.DBInitializer;
using Scalar.AspNetCore;
using Stripe;
using OrderService = N_Shop.API.Services.OrderService;
using ProductService = N_Shop.API.Services.ProductService;

namespace N_Shop.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.AllowAnyOrigin();
                });
        });

        // Add services to the container.
       // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       //     .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));
       
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<ApplicationDbContext>(optoins => optoins.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<ICategoryService,CategoryService>();
        builder.Services.AddScoped<IBrandService,BrandService>();
        builder.Services.AddScoped<IProductService,ProductService>();
        builder.Services.AddScoped<ICartService,CartService>();
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.AddScoped<IUserService,UserService>();
        builder.Services.AddScoped<IOrderService,OrderService>();
        
        builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        
        builder.Services.AddScoped<IDBInitializer,DBInitializer>();
        
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9itzSNVJRJHesEX6mevgGCjltu79tbCj"))
            };
        });

        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
        
        var app = builder.Build();
        app.UseCors(MyAllowSpecificOrigins);

        var scope = app.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        dbInitializer.Initialize();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()||app.Environment.IsProduction() )
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        // var context = new ApplicationDbContext();
        // try
        // {
        //     context.Database.CanConnect();
        //     Console.WriteLine("done");
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine("error");
        //     throw;
        // }
        //
        
        app.MapControllers();

        app.Run();
    }
}