using System.ComponentModel.DataAnnotations;

namespace SynapseSupport.Models;

public class Category
{
    public int Id { get; set; }
    [MaxLength(80)] public string Nome { get; set; } = null!;
}