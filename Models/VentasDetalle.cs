using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Josmery_Ap1_P2.Models;
public class VentasDetalle
{
    [Key]
    public int DetalleId { get; set; }
    public int VentaId { get; set; }

    [ForeignKey("VentaId")]
    public Ventas? Ventas { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un producto.")]
    public int ProductoId { get; set; }

    [ForeignKey("ProductoId")]
    public Productos? Producto { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
}