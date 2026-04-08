using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Afastamentos;

public class AfastamentoCreateDto
{
    [Required, MaxLength(13)]
    public string Matricula { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFinal { get; set; }

    [MaxLength(14)]
    public string? CnpjUnidade { get; set; }

    [MaxLength(10)]
    public string? CodigoSituacao { get; set; }

    [Required]
    public long EmpregadoId { get; set; }

    [Required, MaxLength(40)]
    public string Controle { get; set; } = string.Empty;
}
