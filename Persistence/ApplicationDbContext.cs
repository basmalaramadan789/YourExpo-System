using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourExpo.Models;

namespace YourExpo.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<ProductTargetCountry> ProductTargetCountries { get; set; }
    public DbSet<TargetCountry> TargetCountries { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
      new Category { Id = 1, Name = "Fruits" },
           new Category { Id = 2, Name = "Vegetables" });

        modelBuilder.Entity<TargetCountry>().HasData(
            new TargetCountry { Id = 1, Name = "Egypt" },
            new TargetCountry { Id = 2, Name = "Saudi Arabia" },
            new TargetCountry { Id = 3, Name = "United Arab Emirates" },
            new TargetCountry { Id = 4, Name = "Jordan" },
            new TargetCountry { Id = 5, Name = "Lebanon" }
        );
        modelBuilder.Entity<Supplier>().HasData(
       new Supplier
       {
           Id = 1,
           SupplierName = "Supplier 1",
           ContactInfo = "contact1@supplier.com",
           IsApproved = true
       },
       new Supplier
       {
           Id = 2,
           SupplierName = "Supplier 2",
           ContactInfo = "contact2@supplier.com",
           IsApproved = false
       },
       new Supplier
       {
           Id = 3,
           SupplierName = "Supplier 3",
           ContactInfo = "contact3@supplier.com",
           IsApproved = true
       }
   );

        
        modelBuilder.Entity<ProductTargetCountry>()
            .HasKey(ptc => new { ptc.ProductId, ptc.TargetCountryId });

        modelBuilder.Entity<ProductTargetCountry>()
            .HasOne(ptc => ptc.Product)
            .WithMany(p => p.ProductTargetCountries)
            .HasForeignKey(ptc => ptc.ProductId);

        modelBuilder.Entity<ProductTargetCountry>()
            .HasOne(ptc => ptc.TargetCountry)
            .WithMany(tc => tc.ProductTargetCountries)
            .HasForeignKey(ptc => ptc.TargetCountryId);

        modelBuilder.Entity<ApplicationUser>()
        .HasOne(u => u.Supplier)  
        .WithMany(s => s.Users)  
        .HasForeignKey(u => u.SupplierId);
    }

}