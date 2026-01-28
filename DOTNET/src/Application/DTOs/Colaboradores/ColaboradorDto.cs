namespace Application.DTOs.Colaboradores;

public class ColaboradorDto
{
    public string Matricula { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Pis { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateOnly DataAdmissao { get; set; }
    public DateOnly? DataDemissao { get; set; }
    public string? NomeSetor { get; set; }
    public string? NomeCargo { get; set; }
    public string? NomeDepartamento { get; set; }
    public string NomeUnidade { get; set; } = string.Empty;
    public string NomeLotacao { get; set; } = string.Empty;
    public string Situacao { get; set; } = string.Empty;
    public string CnpjUnidade { get; set; } = string.Empty;
    public string? Ctps { get; set; }
    public string? Serie { get; set; }
}
