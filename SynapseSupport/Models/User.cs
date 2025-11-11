using System.ComponentModel.DataAnnotations;

namespace SynapseSupport.Models;

public class User
{
    public int Id { get; set; }
    [MaxLength(120)] public string Nome { get; set; } = null!;
    [MaxLength(160)] public string Email { get; set; } = null!;
    [MaxLength(255)] public string SenhaHash { get; set; } = null!;
    public Role Perfil { get; set; } = Role.USUARIO;
    [MaxLength(80)] public string? Setor { get; set; }
    public bool Ativo { get; set; } = true;
}