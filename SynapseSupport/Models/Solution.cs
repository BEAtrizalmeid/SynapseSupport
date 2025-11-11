using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SynapseSupport.Models;

public class Solution
{
    public int Id { get; set; }

    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    [MaxLength(1000)]
    public string Descricao { get; set; } = null!; // aqui será a "resposta" do ticket

    public int TicketId { get; set; }

    [ForeignKey(nameof(TicketId))]
    public Ticket Ticket { get; set; } = null!;

    public int TecnicoId { get; set; }

    [ForeignKey(nameof(TecnicoId))]
    public User Tecnico { get; set; } = null!;
}
