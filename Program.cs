using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using YourExpo.Helper;
using YourExpo.Models;
using YourExpo.Persistence;
using YourExpo.Persistence.SeedingData;
using YourExpo.Services;

namespace YourExpo;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddDistributedMemoryCache(); // Add in-memory cache
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout (optional)
            options.Cookie.HttpOnly = true; // Set cookie settings
            options.Cookie.IsEssential = true; // Essential for the session to work
        });
        // Add services to the container.
        builder.Services.AddControllersWithViews();



        builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 


        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders(); 


        builder.Services.AddTransient<IOrderService,OrderService>(); 
        builder.Services.AddTransient<IShoppingCartService,ShoppingCartService>();


        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        // Set Stripe API Key
        var stripeSettings = builder.Configuration.GetSection("Stripe").Get<StripeSettings>();
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;

        var app = builder.Build();


        // Seed roles and admin user
        var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedingRoles.Initialize(scope.ServiceProvider, userManager, roleManager);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}
