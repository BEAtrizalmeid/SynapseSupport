using System.ComponentModel.DataAnnotations;

namespace SynapseSupport.DTOs;

public class TicketDto
{
    [Required] [MaxLength(500)] public string Descricao { get; set; } = null!;
    public int? CategoriaId { get; set; }
    [MaxLength(20)] public string? Prioridade { get; set; } = "Normal";
}