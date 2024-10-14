using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Questao5.Controller;
using Questao5.Application.Exceptions;
using Xunit;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;

namespace Questao5.Tests
{
    public class ContaCorrenteControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ContaCorrenteController _controller;

        public ContaCorrenteControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ContaCorrenteController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Movimentacao_ContaValida_Retorna200ComId()
        {
            // Arrange
            var request = new MovimentacaoRequest
            {
                Id = "request-id-1",
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                Valor = 100.00,
                TipoMovimento = "C"
            };

            var mockMediator = new Mock<IMediator>();

            var response = new MovimentacaoResponse
            {
                IdMovimento = "movimento-id-1"
            };

            // Configura o mock para o método específico que será chamado
            mockMediator
                .Setup(m => m.Send(It.IsAny<MovimentacaoRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Substitui o _mediator pelo mock criado
            var _controller = new ContaCorrenteController(mockMediator.Object);

            // Act
            var result = await _controller.MovimentarContaCorrente(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Extrai o objeto de resposta e verifica o IdMovimento
            var resultValue = result.Value as MovimentacaoResponse;
            Assert.NotNull(resultValue);
            Assert.Equal("movimento-id-1", resultValue.IdMovimento);
        }

        [Fact]
        public async Task Movimentacao_ContaInativa_Retorna400()
        {
            // Arrange
            var request = new MovimentacaoRequest
            {
                Id = "request-id-2",
                IdContaCorrente = "FA99D033-7067-ED11-96C6-7C5DFA4A16C9",
                Valor = 50.00,
                TipoMovimento = "D"
            };

            var mockMediator = new Mock<IMediator>();

            // Configura o mock para lançar uma exceção de BadRequest ao enviar a solicitação
            mockMediator
                .Setup(m => m.Send(It.IsAny<MovimentacaoRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BadRequestException("Conta inativa.", "INACTIVE_ACCOUNT"));

            // Substitui o _mediator pelo mock criado
            var _controller = new ContaCorrenteController(mockMediator.Object);

            // Act
            var result = await _controller.MovimentarContaCorrente(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);

            if (result.Value is not null)
            {
                var mensagem = result.Value.GetType().GetProperty("message")?.GetValue(result.Value)?.ToString();
                Assert.Equal("Conta inativa.", mensagem);
            }
            else
            {
                Assert.True(false, "O valor do resultado não contém a mensagem esperada.");
            }
        }


        [Fact]
        public async Task ConsultaSaldo_ContaValida_Retorna200ComSaldo()
        {
            // Arrange
            var idContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9";
            var expectedResponse = new ConsultaSaldoResult
            {
                Numero = 123,
                NomeTitular = "Katherine Sanchez",
                DataHoraResposta = DateTime.Now,
                SaldoAtual = 100.00
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ConsultaSaldoQuery>(), default))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.ConsultarSaldo(idContaCorrente);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualResponse = Assert.IsType<ConsultaSaldoResult>(okResult.Value);
            Assert.Equal(expectedResponse.Numero, actualResponse.Numero);
            Assert.Equal(expectedResponse.NomeTitular, actualResponse.NomeTitular);
            Assert.Equal(expectedResponse.SaldoAtual, actualResponse.SaldoAtual);
        }

        [Fact]
        public async Task ConsultaSaldo_ContaInativa_Retorna400()
        {
            // Arrange
            var request = "FA99D033-7067-ED11-96C6-7C5DFA4A16C9";

            // Configura o mock para lançar uma exceção ao enviar a consulta
            _mediatorMock.Setup(m => m.Send(It.IsAny<ConsultaSaldoQuery>(), default))
                .ThrowsAsync(new BadRequestException("Conta inativa.", "INACTIVE_ACCOUNT"));

            // Act
            var result = await _controller.ConsultarSaldo(request) as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Conta inativa.", result.Value?.GetType().GetProperty("mensagem")?.GetValue(result.Value));

        }
    }
}

