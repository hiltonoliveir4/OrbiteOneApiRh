namespace Domain.Entities;

public class Afastamento
{
    public int Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFinal { get; set; }
    public string? CnpjUnidade { get; set; }
    public string? CodigoSituacao { get; set; }

    public Colaborador? Colaborador { get; set; }
}
