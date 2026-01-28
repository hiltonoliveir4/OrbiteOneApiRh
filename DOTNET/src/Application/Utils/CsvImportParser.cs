using Application.DTOs.Afastamentos;
using Application.DTOs.Colaboradores;
using Application.Exceptions;

namespace Application.Utils;

public static class CsvImportParser
{
    public static List<(int linha, ColaboradorImportDto data)> ParseColaboradores(string body)
    {
        var lines = NormalizeLines(body);
        var headers = ParseHeaders(lines);

        var result = new List<(int linha, ColaboradorImportDto data)>();
        for (var index = 1; index < lines.Count; index++)
        {
            var values = lines[index].Split('|');
            if (values.Length != headers.Count)
            {
                throw new OrbiteOneException("CSV inválido");
            }

            var item = new ColaboradorImportDto();
            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                var raw = values[i].Trim();
                if (string.IsNullOrEmpty(raw))
                {
                    continue;
                }

                switch (header)
                {
                    case "matricula":
                        item.Matricula = raw;
                        break;
                    case "nome":
                        item.Nome = raw;
                        break;
                    case "pis":
                        item.Pis = raw;
                        break;
                    case "cpf":
                        item.Cpf = raw;
                        break;
                    case "data_admissao":
                        item.DataAdmissao = DateOnly.Parse(raw);
                        break;
                    case "data_demissao":
                        item.DataDemissao = DateOnly.Parse(raw);
                        break;
                    case "nome_setor":
                        item.NomeSetor = raw;
                        break;
                    case "nome_cargo":
                        item.NomeCargo = raw;
                        break;
                    case "nome_departamento":
                        item.NomeDepartamento = raw;
                        break;
                    case "nome_unidade":
                        item.NomeUnidade = raw;
                        break;
                    case "nome_lotacao":
                        item.NomeLotacao = raw;
                        break;
                    case "situacao":
                        item.Situacao = raw;
                        break;
                    case "cnpj_unidade":
                        item.CnpjUnidade = raw;
                        break;
                    case "ctps":
                        item.Ctps = raw;
                        break;
                    case "serie":
                        item.Serie = raw;
                        break;
                }
            }

            result.Add((index + 1, item));
        }

        return result;
    }

    public static List<(int linha, AfastamentoImportDto data)> ParseAfastamentos(string body)
    {
        var lines = NormalizeLines(body);
        var headers = ParseHeaders(lines);

        var result = new List<(int linha, AfastamentoImportDto data)>();
        for (var index = 1; index < lines.Count; index++)
        {
            var values = lines[index].Split('|');
            if (values.Length != headers.Count)
            {
                throw new OrbiteOneException("CSV inválido");
            }

            var item = new AfastamentoImportDto();
            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                var raw = values[i].Trim();
                if (string.IsNullOrEmpty(raw))
                {
                    continue;
                }

                switch (header)
                {
                    case "matricula":
                        item.Matricula = raw;
                        break;
                    case "descricao":
                        item.Descricao = raw;
                        break;
                    case "data_inicio":
                        item.DataInicio = DateTime.Parse(raw);
                        break;
                    case "data_final":
                        item.DataFinal = DateTime.Parse(raw);
                        break;
                    case "cnpj_unidade":
                        item.CnpjUnidade = raw;
                        break;
                    case "codigo_situacao":
                        item.CodigoSituacao = raw;
                        break;
                }
            }

            result.Add((index + 1, item));
        }

        return result;
    }

    private static List<string> NormalizeLines(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new OrbiteOneException("CSV vazio");
        }

        var lines = body
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line))
            .ToList();

        if (lines.Count < 2)
        {
            throw new OrbiteOneException("CSV inválido");
        }

        return lines;
    }

    private static List<string> ParseHeaders(List<string> lines)
    {
        var headers = lines[0]
            .Split('|')
            .Select(header => header.Trim())
            .ToList();

        if (headers.Any(string.IsNullOrEmpty))
        {
            throw new OrbiteOneException("CSV inválido");
        }

        return headers;
    }
}
