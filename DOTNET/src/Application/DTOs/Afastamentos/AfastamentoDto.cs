using System.Text.Json.Serialization;

namespace Application.DTOs.Afastamentos;

public class AfastamentoDto
{
    public int Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFinal { get; set; }
    public string? CnpjUnidade { get; set; }
    public string? CodigoSituacao { get; set; }
    [JsonPropertyName("datamovimento")]
    public DateTime DataMovimento { get; set; }
}
