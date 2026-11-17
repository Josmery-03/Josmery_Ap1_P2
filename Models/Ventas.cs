using System.ComponentModel.DataAnnotations;
namespace Josmery_Ap1_P2.Models;
public class Ventas
{
    [Key]
    public int VentaId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El detalle es obligatorio.")]
    public string Observacion { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public ICollection<VentasDetalle> VentasDetalle { get; set; } = new List<VentasDetalle>();
}