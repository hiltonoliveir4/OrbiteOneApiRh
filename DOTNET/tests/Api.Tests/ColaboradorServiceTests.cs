using Application.DTOs.Colaboradores;
using Application.Exceptions;
using Application.Services;
using Api.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Api.Tests;

[TestClass]
public class ColaboradorServiceTests
{
    [TestMethod]
    public async Task Criar_DeveCriarColaborador()
    {
        var repo = new InMemoryColaboradorRepository();
        var service = new ColaboradorService(repo);

        var dto = new ColaboradorCreateDto
        {
            Matricula = "123",
            Nome = "Ana",
            Pis = "111",
            Cpf = "222",
            DataAdmissao = new DateOnly(2024, 1, 1),
            NomeUnidade = "Unidade A",
            NomeLotacao = "Lotacao A",
            Situacao = "ATIVO",
            CnpjUnidade = "12345678901234",
        };

        var created = await service.Criar(dto);

        Assert.AreEqual("123", created.Matricula);
        Assert.AreEqual("Ana", created.Nome);
    }

    [TestMethod]
    public async Task Criar_DeveRetornar409QuandoDuplicado()
    {
        var repo = new InMemoryColaboradorRepository();
        var service = new ColaboradorService(repo);

        var dto = new ColaboradorCreateDto
        {
            Matricula = "123",
            Nome = "Ana",
            Pis = "111",
            Cpf = "222",
            DataAdmissao = new DateOnly(2024, 1, 1),
            NomeUnidade = "Unidade A",
            NomeLotacao = "Lotacao A",
            Situacao = "ATIVO",
            CnpjUnidade = "12345678901234",
        };

        await service.Criar(dto);

        try
        {
            await service.Criar(dto);
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (OrbiteOneException ex)
        {
            Assert.AreEqual(409, ex.StatusCode);
        }
    }

    [TestMethod]
    public async Task BuscarPorMatricula_DeveRetornarErroQuandoNaoEncontrado()
    {
        var repo = new InMemoryColaboradorRepository();
        var service = new ColaboradorService(repo);

        try
        {
            await service.BuscarPorMatricula("999");
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (OrbiteOneException ex)
        {
            Assert.AreEqual("Colaborador não encontrado", ex.Message);
        }
    }
}
