using Moq;
using Questao5.Application.Services;
using Questao5.Domain.Interfaces.Repository;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;
using Questao5.Infrastructure.Sqlite;
using Xunit;
using System.Threading.Tasks;
using Questao5.Domain.Entities;

namespace Questao5.Tests
{
    public class ContaCorrenteServiceTests
    {
        private readonly Mock<IContaCorrenteRepository> _mockRepository;
        private readonly ContaCorrenteService _service;

        public ContaCorrenteServiceTests()
        {
            var databaseConfig = new DatabaseConfig { Name = "Data Source=:memory:" };
            _mockRepository = new Mock<IContaCorrenteRepository>();
            _service = new ContaCorrenteService(databaseConfig, _mockRepository.Object);
        }

        [Fact]
        public async Task MovimentarConta_ContaValida_RetornaIdMovimento()
        {
            // Arrange
            var request = new MovimentacaoRequest
            {
                Id = "request-id-1",
                IdContaCorrente = "conta-id-1",
                Valor = 100.0,
                TipoMovimento = "C"
            };

            var idMovimento = Guid.NewGuid().ToString();

            _mockRepository
                .Setup(repo => repo.InserirMovimentoAsync(request, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(repo => repo.SalvarIdempotenciaAsync(request.Id, request, It.IsAny<MovimentacaoResponse>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.MovimentarConta(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(Guid.TryParse(result.IdMovimento, out _));
        }

        [Fact]
        public async Task ConsultarSaldo_ContaValida_RetornaSaldoCorreto()
        {
            // Arrange
            var idContaCorrente = "conta-id-1";
            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = idContaCorrente,
                Numero = 123456,
                Nome = "Titular Teste",
                Ativo = true
            };

            var saldoEsperado = 150.0;

            _mockRepository
                .Setup(repo => repo.GetContaCorrenteByIdAsync(idContaCorrente))
                .ReturnsAsync(contaCorrente);

            _mockRepository
                .Setup(repo => repo.GetSaldoAsync(idContaCorrente))
                .ReturnsAsync(saldoEsperado);

            // Act
            var result = await _service.ConsultarSaldo(idContaCorrente);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(123456, result.NumeroContaCorrente);
            Assert.Equal("Titular Teste", result.NomeTitular);
            Assert.Equal(saldoEsperado, result.Saldo);
            Assert.False(string.IsNullOrWhiteSpace(result.DataConsulta));
        }

    }

}
