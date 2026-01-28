using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Colaboradores;

public class ColaboradorCreateDto
{
    [Required, MaxLength(13)]
    public string Matricula { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(11)]
    public string Pis { get; set; } = string.Empty;

    [Required, MaxLength(11)]
    public string Cpf { get; set; } = string.Empty;

    [Required]
    public DateOnly DataAdmissao { get; set; }

    public DateOnly? DataDemissao { get; set; }

    [MaxLength(40)]
    public string? NomeSetor { get; set; }

    [MaxLength(40)]
    public string? NomeCargo { get; set; }

    [MaxLength(40)]
    public string? NomeDepartamento { get; set; }

    [Required, MaxLength(40)]
    public string NomeUnidade { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string NomeLotacao { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Situacao { get; set; } = string.Empty;

    [Required, MaxLength(14)]
    public string CnpjUnidade { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Ctps { get; set; }

    [MaxLength(15)]
    public string? Serie { get; set; }
}
