namespace Application.DTOs.Afastamentos;

public class AfastamentoUpdateDto
{
    public string? Matricula { get; set; }
    public string? Descricao { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFinal { get; set; }
    public string? CnpjUnidade { get; set; }
    public string? CodigoSituacao { get; set; }
    public long? EmpregadoId { get; set; }
    public string? Controle { get; set; }
}
