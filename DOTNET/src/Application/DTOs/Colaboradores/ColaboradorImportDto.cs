namespace Application.DTOs.Colaboradores;

public class ColaboradorImportDto
{
    public string? Matricula { get; set; }
    public string? Nome { get; set; }
    public string? Pis { get; set; }
    public string? Cpf { get; set; }
    public DateOnly? DataAdmissao { get; set; }
    public DateOnly? DataDemissao { get; set; }
    public string? NomeSetor { get; set; }
    public string? NomeCargo { get; set; }
    public string? NomeDepartamento { get; set; }
    public string? NomeUnidade { get; set; }
    public string? NomeLotacao { get; set; }
    public string? Situacao { get; set; }
    public string? CnpjUnidade { get; set; }
    public string? Ctps { get; set; }
    public string? Serie { get; set; }
}
