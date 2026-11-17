using Microsoft.EntityFrameworkCore;
using Josmery_Ap1_P2.Models;
namespace Josmery_Ap1_P2.Context;
public class Contexto : DbContext
{
    public Contexto(DbContextOptions<Contexto> options) : base(options) { }
    public DbSet<Productos> Productos {  get; set; }
    public DbSet<Ventas> Ventas { get; set; }
    public DbSet<VentasDetalle> VentasDetalles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Productos>().HasData(
            new Productos
            {
                ProductoId = 1,
                Descripcion = "Café Capuchino Helado 250ml",
                Precio = 120.00m,
                Stock = 10
            },
            new Productos
            {
                ProductoId = 2,
                Descripcion = "Café Latte Vainilla Embotellado 300ml",
                Precio = 140.00m,
                Stock = 10
            },
            new Productos
            {
                ProductoId = 3,
                Descripcion = "Café Moca Frío 280ml",
                Precio = 155.00m,
                Stock = 10
            },
            new Productos
            {
                ProductoId = 4,
                Descripcion = "Café Expreso Americano 250ml",
                Precio = 110.00m,
                Stock = 10
            },
            new Productos
            {
                ProductoId = 5,
                Descripcion = "Café Caramelo Macchiato 300ml",
                Precio = 165.00m,
                Stock = 10
            }
        );
    }
}