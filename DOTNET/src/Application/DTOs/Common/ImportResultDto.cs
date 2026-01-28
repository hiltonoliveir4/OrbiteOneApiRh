namespace Application.DTOs.Common;

public class ImportResultDto
{
    public int Total { get; set; }
    public int Criados { get; set; }
    public int Atualizados { get; set; }
    public int Sucesso { get; set; }
    public int Erros { get; set; }
    public List<ImportErrorLineDto> LinhasComErro { get; set; } = new();
}

public class ImportErrorLineDto
{
    public int Linha { get; set; }
    public string Erro { get; set; } = string.Empty;
}
