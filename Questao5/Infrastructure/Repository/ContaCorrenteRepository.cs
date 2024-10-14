using Dapper;
using Newtonsoft.Json;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repository;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;
using System.Data; 
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Repository
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly IDbConnection _dbConnection;

        public ContaCorrenteRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<bool> ExisteIdempotenciaAsync(string chaveIdempotencia)
        {
            var idempotencia = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia",
                new { ChaveIdempotencia = chaveIdempotencia });

            return !string.IsNullOrEmpty(idempotencia);
        }

        public async Task SalvarIdempotenciaAsync(string chaveIdempotencia, MovimentacaoRequest request, MovimentacaoResponse response)
        {
            await _dbConnection.ExecuteAsync(
                "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)",
                new
                {
                    ChaveIdempotencia = chaveIdempotencia,
                    Requisicao = JsonConvert.SerializeObject(request),
                    Resultado = JsonConvert.SerializeObject(response)
                });
        }

        public async Task InserirMovimentoAsync(MovimentacaoRequest request, string idMovimento)
        {
            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                new
                {
                    IdMovimento = idMovimento,
                    request.IdContaCorrente,
                    DataMovimento = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    request.TipoMovimento,
                    request.Valor
                });
        }

        public async Task<ContaCorrente> GetContaCorrenteByIdAsync(string idContaCorrente)
        {
            var sql = "SELECT * FROM ContaCorrente WHERE idcontacorrente = @idcontacorrente";
            return await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { idcontacorrente = idContaCorrente });
        }

        public async Task<double> GetSaldoAsync(string idContaCorrente)
        {
            var debitos = await _dbConnection.QueryFirstOrDefaultAsync<double>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'D'",
                new { IdContaCorrente = idContaCorrente });

            var creditos = await _dbConnection.QueryFirstOrDefaultAsync<double>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'C'",
                new { IdContaCorrente = idContaCorrente });

            return creditos - debitos;
        }

    }
}
