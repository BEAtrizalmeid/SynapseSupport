using System.ComponentModel.DataAnnotations;
using SynapseSupport.Models;

namespace SynapseSupport.DTOs;

public class RegisterDto
{
    [Required] [MaxLength(120)] public string Nome { get; set; } = null!;
    [Required] [EmailAddress] [MaxLength(160)] public string Email { get; set; } = null!;
    [Required] [MinLength(6)] public string Senha { get; set; } = null!;
    public Role Perfil { get; set; } = Role.USUARIO;
    [MaxLength(80)] public string? Setor { get; set; }
}