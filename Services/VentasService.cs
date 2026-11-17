using Aplicada1.Core;
using Josmery_Ap1_P2.Context;
using Josmery_Ap1_P2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Josmery_Ap1_P2.Services;
public class VentasService(IDbContextFactory<Contexto> contextFactory) : IServiceResult<Ventas, int>
{
    public async Task<Result> Guardar(Ventas venta)
    {
        if (venta.VentasDetalle == null || !venta.VentasDetalle.Any())
            return Result.Failure(new Error("Ventas.SinDetalle", "Debe agregar al menos un producto al detalle."));

        var guardado = await Existe(venta.VentaId)
            ? await Modificar(venta)
            : await Insertar(venta);

        return guardado
            ? Result.Success(true)
            : Result.Failure(new Error("Ventas.ErrorGuardar", "No se pudo guardar la venta. Verifique que haya stock suficiente."));
    }
    private async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Ventas.AnyAsync(v => v.VentaId == id);
    }
    private async Task<bool> Insertar(Ventas venta)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        foreach (var detalle in venta.VentasDetalle)
        {
            var producto = await contexto.Productos.FindAsync(detalle.ProductoId);
            if (producto == null || producto.Stock < detalle.Cantidad)
                return false;

            if (detalle.Precio <= 0)
                detalle.Precio = producto.Precio;

            producto.Stock -= detalle.Cantidad;
        }

        venta.Total = venta.VentasDetalle.Sum(d => d.Cantidad * d.Precio);
        contexto.Ventas.Add(venta);

        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Ventas venta)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var original = await contexto.Ventas
            .Include(v => v.VentasDetalle)
            .FirstOrDefaultAsync(v => v.VentaId == venta.VentaId);

        if (original == null)
            return false;

        foreach (var detOriginal in original.VentasDetalle)
        {
            var prodOriginal = await contexto.Productos.FindAsync(detOriginal.ProductoId);
            if (prodOriginal != null)
            {
                prodOriginal.Stock += detOriginal.Cantidad;
            }
        }

        contexto.VentasDetalles.RemoveRange(original.VentasDetalle);

        var nuevosDetalles = new List<VentasDetalle>();
        foreach (var detNuevo in venta.VentasDetalle)
        {
            var prodNuevo = await contexto.Productos.FindAsync(detNuevo.ProductoId);
            if (prodNuevo == null || prodNuevo.Stock < detNuevo.Cantidad)
                return false;

            if (detNuevo.Precio <= 0)
                detNuevo.Precio = prodNuevo.Precio;

            prodNuevo.Stock -= detNuevo.Cantidad;

            nuevosDetalles.Add(new VentasDetalle
            {
                VentaId = original.VentaId,
                ProductoId = detNuevo.ProductoId,
                Cantidad = detNuevo.Cantidad,
                Precio = detNuevo.Precio
            });
        }

        contexto.VentasDetalles.AddRange(nuevosDetalles);

        original.Fecha = venta.Fecha;
        original.Observacion = venta.Observacion;
        original.Total = nuevosDetalles.Sum(d => d.Cantidad * d.Precio);

        return await contexto.SaveChangesAsync() > 0;
    }
    public async Task<Result<Ventas?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(v => v.VentasDetalle)
                .ThenInclude(d => d.Producto)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VentaId == id);
    }
    public async Task<Result<List<Ventas>>> GetList(Expression<Func<Ventas, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(v => v.VentasDetalle)
                .ThenInclude(d => d.Producto)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var venta = await contexto.Ventas
            .Include(v => v.VentasDetalle)
            .FirstOrDefaultAsync(v => v.VentaId == id);

        if (venta == null)
            return Result.Failure(new Error("Ventas.NoEncontrada", "Venta no encontrada."));

        foreach (var detalle in venta.VentasDetalle)
        {
            var producto = await contexto.Productos.FindAsync(detalle.ProductoId);
            if (producto != null)
            {
                producto.Stock += detalle.Cantidad;
            }
        }

        contexto.VentasDetalles.RemoveRange(venta.VentasDetalle);
        contexto.Ventas.Remove(venta);

        return await contexto.SaveChangesAsync() > 0
            ? Result.Success(true)
            : Result.Failure(new Error("Ventas.ErrorEliminar", "No se pudo eliminar la venta."));
    }
    public async Task<List<Productos>> ListarProductos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Productos
            .AsNoTracking()
            .ToListAsync();
    }
}