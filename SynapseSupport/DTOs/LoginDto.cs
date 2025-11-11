using System.ComponentModel.DataAnnotations;

namespace SynapseSupport.DTOs;

public class LoginDto
{
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Senha { get; set; } = null!;
}