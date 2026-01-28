using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Afastamentos;

public class AfastamentoCreateDto
{
    [Required, MaxLength(13)]
    public string Matricula { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTime DataInicio { get; set; }

    public DateTime? DataFinal { get; set; }

    [MaxLength(14)]
    public string? CnpjUnidade { get; set; }

    [MaxLength(10)]
    public string? CodigoSituacao { get; set; }
}
