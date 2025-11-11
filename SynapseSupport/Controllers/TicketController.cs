using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynapseSupport.Data;
using SynapseSupport.DTOs;
using SynapseSupport.Models;
using SynapseSupport.Services;

namespace SynapseSupport.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AIService _ai;
    private int UserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
    private Role UserRole => Enum.Parse<Role>(User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value);

    public TicketController(AppDbContext db, AIService ai)
    {
        _db = db;
        _ai = ai;
    }

    [HttpPost]
    public async Task<IActionResult> Create(TicketDto dto)
    {
        var ticket = new Ticket
        {
            Descricao = dto.Descricao,
            CategoriaId = dto.CategoriaId,
            Prioridade = dto.Prioridade,
            UsuarioId = UserId
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        // Gera sugestão IA em background
        _ = Task.Run(async () =>
        {
            try { await _ai.GetSuggestionAsync(dto.Descricao, ticket.Id); }
            catch { /* log silencioso */ }
        });

        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _db.Tickets
            .Include(t => t.Usuario)
            .Include(t => t.Tecnico)
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();
        if (ticket.UsuarioId != UserId && UserRole != Role.TECNICO && UserRole != Role.ADMIN)
            return Forbid();

        return Ok(ticket);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTickets()
    {
        var query = _db.Tickets.Include(t => t.Categoria).AsQueryable();

        if (UserRole == Role.USUARIO)
            query = query.Where(t => t.UsuarioId == UserId);
        else if (UserRole == Role.TECNICO)
            query = query.Where(t => t.TecnicoId == UserId || t.TecnicoId == null);

        var tickets = await query.ToListAsync();
        return Ok(tickets);
    }

    [HttpPatch("{id}/assign")]
    [Authorize(Roles = "TECNICO,ADMIN")]
    public async Task<IActionResult> Assign(int id)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        ticket.TecnicoId = UserId;
        await _db.SaveChangesAsync();

        return Ok(ticket);
    }

    [HttpPost("{id}/solution")]
    [Authorize(Roles = "TECNICO,ADMIN")]
    public async Task<IActionResult> AddSolution(int id, [FromBody] string descricao)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        var solution = new Solution
        {
            Descricao = descricao,
            TicketId = id,
            TecnicoId = UserId
        };

        _db.Solutions.Add(solution);
        ticket.Status = "Resolvido";
        await _db.SaveChangesAsync();

        return Ok(solution);
    }

    [HttpGet("{id}/ai-suggestion")]
    public async Task<IActionResult> GetAISuggestion(int id)
    {
        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        var log = await _db.AILogs
            .OrderByDescending(l => l.Data)
            .FirstOrDefaultAsync(l => l.TicketId == id);

        if (log == null)
        {
            var suggestion = await _ai.GetSuggestionAsync(ticket.Descricao, id);
            return Ok(new { suggestion });
        }

        return Ok(new { suggestion = log.Resposta });
    }
}
