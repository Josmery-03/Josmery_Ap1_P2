using System.ComponentModel.DataAnnotations;
namespace Josmery_Ap1_P2.Models;
public class Productos
{
    [Key]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "La descripcion es requerida.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es requerido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no debe ser negativo.")]
    public int Stock { get; set; } = 10;
}