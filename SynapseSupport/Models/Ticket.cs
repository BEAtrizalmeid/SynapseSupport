using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SynapseSupport.Models;

public class Ticket
{
    public int Id { get; set; }
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    [MaxLength(20)] public string Status { get; set; } = "Aberto";
    [MaxLength(20)] public string? Prioridade { get; set; } = "Normal";
    [MaxLength(500)] public string Descricao { get; set; } = null!;
    public int? CategoriaId { get; set; }
    public Category? Categoria { get; set; }
    public int UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))] public User Usuario { get; set; } = null!;
    public int? TecnicoId { get; set; }
    [ForeignKey(nameof(TecnicoId))] public User? Tecnico { get; set; }
}