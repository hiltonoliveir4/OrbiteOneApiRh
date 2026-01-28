using Application.DTOs.Afastamentos;
using Application.Exceptions;
using Application.Services;
using Api.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Api.Tests;

[TestClass]
public class AfastamentoServiceTests
{
    [TestMethod]
    public async Task Criar_DeveCriarAfastamento()
    {
        var repo = new InMemoryAfastamentoRepository();
        var service = new AfastamentoService(repo);

        var dto = new AfastamentoCreateDto
        {
            Matricula = "123",
            Descricao = "Licenca",
            DataInicio = new DateTime(2024, 1, 10),
        };

        var created = await service.Criar(dto);

        Assert.AreEqual("123", created.Matricula);
        Assert.AreEqual("Licenca", created.Descricao);
    }

    [TestMethod]
    public async Task BuscarPorId_DeveRetornar404QuandoNaoEncontrado()
    {
        var repo = new InMemoryAfastamentoRepository();
        var service = new AfastamentoService(repo);

        try
        {
            await service.BuscarPorId(999);
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (OrbiteOneException ex)
        {
            Assert.AreEqual(404, ex.StatusCode);
        }
    }

    [TestMethod]
    public async Task CriarEmLote_DeveAtualizarOuCriar()
    {
        var repo = new InMemoryAfastamentoRepository();
        var service = new AfastamentoService(repo);

        var items = new List<(int linha, AfastamentoImportDto data)>
        {
            (1, new AfastamentoImportDto { Matricula = "123", Descricao = "Licenca", DataInicio = new DateTime(2024, 1, 10) }),
            (2, new AfastamentoImportDto { Matricula = "124", Descricao = "Ferias", DataInicio = new DateTime(2024, 2, 10) })
        };

        var result = await service.CriarEmLote(items);

        Assert.AreEqual(2, result.Total);
        Assert.AreEqual(2, result.Sucesso);
        Assert.AreEqual(2, result.Criados);
    }
}
