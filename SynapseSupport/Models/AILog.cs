using System.ComponentModel.DataAnnotations;

namespace SynapseSupport.Models;

public class AILog
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    [MaxLength(500)] public string Prompt { get; set; } = null!;
    [MaxLength(1000)] public string Resposta { get; set; } = null!;
    public int? TicketId { get; set; }
}