using BoardGamesStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BoardGamesStore.Data;

public class BoardGamesStoreContext : IdentityDbContext<ApplicationUser>
{
    public BoardGamesStoreContext(DbContextOptions<BoardGamesStoreContext> options) : base(options) {}

    public DbSet<BoardGame> BoardGames { get; set; }

    public DbSet<BoardGameImage> BoardGameImages { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<CartBoardGame> ShoppingCarts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>()
            .HasMany(c => c.BoardGames)
            .WithOne(b => b.Category)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderID, od.BoardGameID });

        builder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderID);

        builder.Entity<OrderDetail>()
            .HasOne(od => od.BoardGame)
            .WithMany(b => b.OrderDetails)
            .HasForeignKey(od => od.BoardGameID);


        builder.Entity<CartBoardGame>()
           .HasKey(sc => new { sc.UserID, sc.BoardGameID });

        builder.Entity<CartBoardGame>()
            .HasOne(sc => sc.User)
            .WithMany(u => u.CartBoardGames)
            .HasForeignKey(sc => sc.UserID);

        builder.Entity<CartBoardGame>()
            .HasOne(sc => sc.BoardGame)
            .WithMany(bg => bg.CartBoardGames)
            .HasForeignKey(sc => sc.BoardGameID);
    }
}
