using SynapseSupport.Data; // necessário para AppDbContext
using SynapseSupport.Models;
using System.Text;
using System.Text.Json;

namespace SynapseSupport.Services;

public class AIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public AIService(HttpClient httpClient, IConfiguration config, AppDbContext db)
    {
        _httpClient = httpClient;
        _config = config;
        _db = db;
    }

    public async Task<string> GetSuggestionAsync(string descricao, int? ticketId = null)
    {
        var apiKey = _config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey não configurada");
        var model = _config["OpenAI:Model"] ?? "gpt-3.5-turbo";

        var prompt = $"""
            Você é um técnico de suporte experiente.
            Analise o problema descrito e sugira uma solução clara e objetiva em até 3 passos.

            Problema: {descricao}

            Responda apenas a solução, sem introdução.
            """;

        var requestBody = new
        {
            model,
            messages = new[] { new { role = "user", content = prompt } },
            max_tokens = 300,
            temperature = 0.5
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", apiKey);

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<JsonElement>(json);
        var suggestion = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "Sem sugestão.";

        // Salva log
        _db.AILogs.Add(new AILog
        {
            Prompt = prompt,
            Resposta = suggestion,
            TicketId = ticketId
        });
        await _db.SaveChangesAsync();

        return suggestion;
    }
}
