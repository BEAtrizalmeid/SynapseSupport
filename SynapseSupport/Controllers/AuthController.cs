using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynapseSupport.Data;
using SynapseSupport.DTOs;
using SynapseSupport.Models;
using SynapseSupport.Services;

namespace SynapseSupport.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // Verifica se o e-mail já está cadastrado
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("E-mail já cadastrado.");

        // Cria o usuário com senha criptografada
        var user = new User
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = dto.Perfil,
            Setor = dto.Setor
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Gera token JWT
        var token = _jwt.GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Nome, user.Email, user.Perfil } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, user.SenhaHash))
            return Unauthorized("Credenciais inválidas.");

        // Gera token JWT
        var token = _jwt.GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Nome, user.Email, user.Perfil } });
    }
}
