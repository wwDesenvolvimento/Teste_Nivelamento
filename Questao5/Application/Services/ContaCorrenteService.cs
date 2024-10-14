using Questao5.Domain.Interfaces.Repository;
using Questao5.Domain.Interfaces.Services;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Application.Services
{
    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public ContaCorrenteService(DatabaseConfig databaseConfig, IContaCorrenteRepository contaCorrenteRepository)
        {
            _databaseConfig = databaseConfig;
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<MovimentacaoResponse> MovimentarConta(MovimentacaoRequest request)
        {
            var idMovimento = Guid.NewGuid().ToString();

            await _contaCorrenteRepository.InserirMovimentoAsync(request, idMovimento);

            var resultado = new MovimentacaoResponse { IdMovimento = idMovimento };

            await _contaCorrenteRepository.SalvarIdempotenciaAsync(request.Id, request, resultado);

            return resultado;
        }

        public async Task<SaldoResponse> ConsultarSaldo(string idContaCorrente)
        {
            var conta = await _contaCorrenteRepository.GetContaCorrenteByIdAsync(idContaCorrente);
            var saldo = await _contaCorrenteRepository.GetSaldoAsync(idContaCorrente);

            return new SaldoResponse
            {
                NumeroContaCorrente = conta.Numero,
                NomeTitular = conta.Nome,
                DataConsulta = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Saldo = saldo
            };
        }

    }

}
