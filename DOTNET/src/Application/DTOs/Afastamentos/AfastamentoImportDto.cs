namespace Application.DTOs.Afastamentos;

public class AfastamentoImportDto
{
    public int? Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFinal { get; set; }
    public string? CnpjUnidade { get; set; }
    public string? CodigoSituacao { get; set; }
    public long? EmpregadoId { get; set; }
    public string? Controle { get; set; }
}
